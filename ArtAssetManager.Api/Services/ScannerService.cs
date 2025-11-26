using System.Runtime.CompilerServices;
using System.Text.Json;
using ArtAssetManager.Api.Config;
using ArtAssetManager.Api.Data;
using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Enums;
using ArtAssetManager.Api.Hubs;
using ArtAssetManager.Api.Interfaces;
using ArtAssetManager.Api.Services.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace ArtAssetManager.Api.Services
{
    public static class FileTypes
    {
        public const string Image = "image";
        public const string Model = "model";
        public const string Texture = "texture";
        public const string Other = "other";
    }
    public class ScannerService : BackgroundService
    {
        private readonly ILogger<ScannerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ScannerSettings _scannerSettings;
        private readonly IHubContext<ScanHub, IScanClient> _hubContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IScannerTrigger _trigger;

        public ScannerService(ILogger<ScannerService> logger, IServiceScopeFactory scopeFactory, IOptions<ScannerSettings> scannerSettings, IHubContext<ScanHub, IScanClient> hubContext, IScannerTrigger trigger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _scannerSettings = scannerSettings.Value;
            _hubContext = hubContext;
            _trigger = trigger;
            _webHostEnvironment = webHostEnvironment;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Scanner Service started!");
            //  Uruchom to w tle i nie czekaj aż skończy. Gdyby było await to kod by sie zamroził w tym miejscu
            _ = StartSchedulerAsync(stoppingToken); // Scheduler działą w tle

            await foreach (var mode in _trigger.WaitForTriggersAsync(stoppingToken))
            {
                _logger.LogInformation("Trigger received: {Mode}", mode);
                try
                {
                    _trigger.SetScanningStatus(true);
                    await PerformFullScanAsync(stoppingToken);
                    await _hubContext.Clients.All.ReceiveProgress("Scanner iteration finished", 100, 100);
                }
                catch (OperationCanceledException)
                {

                    _logger.LogInformation("Scanner iteration cancelled - shutting down.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during scanning");
                }
                finally
                {
                    _trigger.SetScanningStatus(false);
                }
            }
            _logger.LogInformation("⛔ Scanner Service stopped.");
        }

        //Trigerrujemy taska
        private async Task StartSchedulerAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduler started.");
            _logger.LogInformation("Starting Initial Scan.");
            await _trigger.TriggerScanAsync(ScanMode.Initial);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(25), stoppingToken);

                _logger.LogDebug(" Scheduler: Sending Scanning Request at {Time}", DateTime.UtcNow);

                await _trigger.TriggerScanAsync(ScanMode.Scheduled);
            }
        }

        private async Task PerformFullScanAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🔍 Scanner iteration at {Time}", DateTime.UtcNow);

            using (var scope = _scopeFactory.CreateScope())
            {
                var assetRepo = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
                var settingsRepo = scope.ServiceProvider.GetRequiredService<ISettingsRepository>();


                var allowedExtensions = await settingsRepo.GetAllowedExtensionsAsync(stoppingToken);
                var scannedFolders = await settingsRepo.GetScanFoldersAsync(stoppingToken);

                var activeFolders = scannedFolders
                    .Where(f => f.IsActive && Directory.Exists(f.Path))
                    .ToList();

                _logger.LogInformation("📂 Active folders to scan: {Count}", activeFolders.Count);

                var allFilePaths = new List<string>();

                await _hubContext.Clients.All.ReceiveScanStatus(ScaningStatusEnumToString(ScanStatus.Scanning));
                await _hubContext.Clients.All.ReceiveProgress("Indexing files...", 0, 0);

                foreach (var folder in activeFolders)
                {
                    try
                    {
                        // SearchOption.AllDirectories = Skanuje podfoldery!
                        var filesInFolder = Directory.GetFiles(folder.Path, "*.*", SearchOption.AllDirectories);
                        allFilePaths.AddRange(filesInFolder);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        _logger.LogWarning("⛔ Brak dostępu do folderu: {Path}", folder.Path);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Błąd podczas indeksowania folderu: {Path}", folder.Path);
                    }
                }

                int totalFilesToScan = allFilePaths.Count;
                _logger.LogInformation($"📦 Znaleziono łącznie {totalFilesToScan} plików do przetworzenia (Flattened List)");

                // ==============================================================================
                // KROK 2: PROCESS PHASE (Jedna pętla po wszystkich plikach)
                // ==============================================================================

                int globalProcessedCount = 0;

                // Sygnał startowy dla UI (0%)
                await _hubContext.Clients.All.ReceiveProgress("Starting analysis...", totalFilesToScan, 0);

                foreach (var filePath in allFilePaths)
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    globalProcessedCount++; // Inkrementujemy ZAWSZE, nawet jak pominiemy plik (żeby progress szedł do przodu)

                    try
                    {
                        var extension = Path.GetExtension(filePath).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            if (globalProcessedCount % 50 == 0) await SendProgress(filePath, totalFilesToScan, globalProcessedCount);
                            continue;
                        }

                        // Sprawdzenie czy istnieje w bazie
                        var existingAssetByPath = await assetRepo.GetAssetByPathAsync(filePath, stoppingToken);
                        if (existingAssetByPath != null)
                        {
                            if (!allowedExtensions.Contains(existingAssetByPath.FileExtension))
                            {
                                // Plik zmienił rozszerzenie na niedozwolone - dajem do trasha
                                await assetRepo.SoftDeleteAssetAsync(existingAssetByPath.Id, stoppingToken);
                                _logger.LogInformation("🗑️ Moved to trash (extension no longer allowed): {FileName}", existingAssetByPath.FileName);
                            }
                            if (existingAssetByPath.IsDeleted && allowedExtensions.Contains(existingAssetByPath.FileExtension))
                            {
                                await assetRepo.RestoreAssetAsync(existingAssetByPath.Id, stoppingToken);
                                _logger.LogInformation("♻️ Restored from trash (extension now allowed): {FileName}", existingAssetByPath.FileName);
                            }
                            if (globalProcessedCount % 50 == 0) await SendProgress(filePath, totalFilesToScan, globalProcessedCount);
                            continue;
                        }


                        var (thumbnailPath, metadata) = await GenerateThumbnailAsync(filePath, extension);
                        var (fileSize, lastModified) = GetFileSizeAndLastModifiedDate(filePath);
                        var fileHash = await ComputeFileHashAsync(filePath, fileSize, stoppingToken);

                        var sourceFolder = activeFolders
                            .OrderByDescending(f => f.Path.Length) // Najdłuższa ścieżka wygrywa (w razie zagnieżdżonych bibliotek)
                            .FirstOrDefault(f => filePath.StartsWith(f.Path));

                        if (sourceFolder == null) continue; // Should not happen

                        Asset newAsset = Asset.Create(sourceFolder.Id, filePath, fileSize, DetermineFileType(extension), thumbnailPath, lastModified, fileHash, metadata?.Width, metadata?.Height, metadata?.DominantColor, metadata?.BitDepth, metadata?.HasAlphaChannel);

                        if (fileHash != null)
                        {
                            var existingAssetByHash = await assetRepo.GetAssetByFileHashAsync(fileHash, stoppingToken);
                            if (existingAssetByHash != null)
                            {
                                var rootId = existingAssetByHash.ParentAssetId ?? existingAssetByHash.Id;
                                newAsset.ParentAssetId = rootId;
                            }
                        }

                        await assetRepo.AddAssetAsync(newAsset, stoppingToken);
                        _logger.LogInformation("✅ Added: {FileName}", newAsset.FileName);
                        // await Task.Delay(TimeSpan.FromSeconds(2)); do testu
                        // --- PROGRESS REPORTING (Smart Update) ---
                        // Raportuj: 
                        // 1. Pierwszy plik
                        // 2. Co 10 plików (dla płynności)
                        // 3. Ostatni plik
                        if (globalProcessedCount == 1 || globalProcessedCount % 10 == 0 || globalProcessedCount == totalFilesToScan)
                        {
                            await SendProgress(filePath, totalFilesToScan, globalProcessedCount);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed file: {FilePath}", filePath);
                    }
                }

                // ZAWSZE raportuj 100% na koniec
                _logger.LogInformation("Scan Finished.");
                await _hubContext.Clients.All.ReceiveScanStatus(ScaningStatusEnumToString(ScanStatus.Idle));
            }
        }

        private async Task SendProgress(string filePath, int total, int current)
        {
            await _hubContext.Clients.All.ReceiveProgress(
                $"Scanning: {Path.GetFileName(filePath)} ({current}/{total})",
                total,
                current
            );
        }

        private string ScaningStatusEnumToString(ScanStatus status)
        {
            return status switch
            {
                ScanStatus.Scanning => "scanning",
                ScanStatus.Idle => "idle",
                _ => "unknown"
            };
        }

        private string DetermineFileType(string extension)
        {
            return extension switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" => FileTypes.Image,
                ".blend" or ".fbx" or ".obj" or ".ztl" or ".zpr" => FileTypes.Model,
                ".psd" or ".ai" or ".svg" or ".exr" or ".hdr" or ".tif" or ".tiff" => FileTypes.Texture,
                _ => FileTypes.Other
            };
        }
        private (long, DateTime) GetFileSizeAndLastModifiedDate(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return (fileInfo.Length, fileInfo.LastWriteTimeUtc);
        }

        private async Task<(string ThumbnailPath, AssetMetadata? Metadata)> GenerateThumbnailAsync(string filePath, string extension)
        {
            var ext = extension.ToLowerInvariant();

            // GENEROWANIE DLA OBRAZKÓW
            if (ext is ".jpg" or ".jpeg" or ".png" or ".webp" or ".bmp" or ".tga")
            {
                try
                {
                    await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

                    using (var image = await Image.LoadAsync(stream))
                    {
                        // Metadata extraction
                        bool hasAlpha = image.PixelType.AlphaRepresentation.HasValue;
                        int bitDepth = image.PixelType.BitsPerPixel;
                        var dominantColor = GetDominantColor(image);

                        AssetMetadata metadata = new(
                            image.Width,
                            image.Height,
                            dominantColor,
                            bitDepth,
                            hasAlpha
                        );

                        image.Mutate(x => x.Resize(400, 0));

                        var uniqueFileName = $"{Guid.NewGuid()}.webp";
                        var thumbsFolder = Path.Combine(_webHostEnvironment.WebRootPath, _scannerSettings.ThumbnailsFolder);

                        if (!Directory.Exists(thumbsFolder))
                        {
                            Directory.CreateDirectory(thumbsFolder);
                        }

                        var fullSavePath = Path.Combine(thumbsFolder, uniqueFileName);
                        await image.SaveAsWebpAsync(fullSavePath);

                        var webPath = Path.Combine("/", _scannerSettings.ThumbnailsFolder, uniqueFileName).Replace("\\", "/");

                        return (webPath, metadata);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to generate thumbnail for image {Path}: {Message}", filePath, ex.Message);
                    // Jeśli failnie generowanie obrazka, spadnie niżej do switcha i zwróci generic placeholder. 
                }
            }

            // LOGIKA PLACEHOLDERÓW (Switch Expression)
            const string placeholderDir = "placeholders";
            const string defaultPlaceholder = "generic_placeholder.webp";

            if (!_scannerSettings.PlaceholderMappings.TryGetValue(ext, out var placeholderFile))
            {
                // Jak nie znajdziemy klucza (np. .c4d), bierzemy default
                placeholderFile = defaultPlaceholder;
            }

            var finalPath = Path.Combine("/", _scannerSettings.ThumbnailsFolder, placeholderDir, placeholderFile)
                                .Replace("\\", "/");

            return (finalPath, null);
        }

        private async Task<string?> ComputeFileHashAsync(string filePath, long fileSizeBytes, CancellationToken cancellationToken)
        {
            if (!_scannerSettings.EnableHashing)
                return null;
            var maxBytes = _scannerSettings.MaxHashFileSizeMB * 1024 * 1024;
            if (fileSizeBytes > maxBytes)
            {
                _logger.LogDebug("⏭️ Skipping hash for large file: {FileName}", Path.GetFileName(filePath));
                return null;
            }
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            await using var stream = File.OpenRead(filePath);
            var hash = await sha256.ComputeHashAsync(stream, cancellationToken);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }


        private string GetDominantColor(Image image)
        {
            var palette = new List<Rgba32>
        {
            new Rgba32(0, 0, 0),       // Black
            new Rgba32(255, 255, 255), // White
            new Rgba32(128, 128, 128), // Gray
            new Rgba32(255, 0, 0),     // Red
            new Rgba32(0, 255, 0),     // Lime
            new Rgba32(0, 0, 255),     // Blue
            new Rgba32(255, 255, 0),   // Yellow
            new Rgba32(0, 255, 255),   // Cyan
            new Rgba32(255, 0, 255),   // Magenta
            new Rgba32(128, 0, 0),     // Maroon
            new Rgba32(0, 128, 0),     // Green
            new Rgba32(0, 0, 128),     // Navy
            new Rgba32(128, 128, 0),   // Olive
            new Rgba32(128, 0, 128),   // Purple
            new Rgba32(0, 128, 128),   // Teal
            new Rgba32(192, 192, 192)  // Silver
        };


            using var small = image.CloneAs<Rgba32>();
            small.Mutate(x => x.Resize(50, 50));


            var colorCount = new Dictionary<Rgba32, int>();

            small.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        var pixel = row[x];
                        if (colorCount.ContainsKey(pixel))
                            colorCount[pixel]++;
                        else
                            colorCount[pixel] = 1;
                    }
                }
            });

            // Dominujący prawdziwy kolor
            var dominant = colorCount.OrderByDescending(c => c.Value).First().Key;

            // Znalezienie najbliższego koloru z palety
            Rgba32 closest = palette
                .OrderBy(p => ColorDistance(p, dominant))
                .First();

            return $"#{closest.R:X2}{closest.G:X2}{closest.B:X2}";
        }

        private static double ColorDistance(Rgba32 a, Rgba32 b)
        {
            int dr = a.R - b.R;
            int dg = a.G - b.G;
            int db = a.B - b.B;
            return dr * dr + dg * dg + db * db;
        }

    }
}


