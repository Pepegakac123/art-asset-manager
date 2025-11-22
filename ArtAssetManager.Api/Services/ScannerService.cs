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
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

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
                _logger.LogInformation("📂 Retrieved {Count} folders", scannedFolders.Count());
                var totalFilesToScan = 0;
                foreach (var folder in scannedFolders)
                {
                    if (!folder.IsActive) continue;
                    if (!Directory.Exists(folder.Path))
                    {
                        _logger.LogWarning("⚠️ Folder not found: {Path}", folder.Path);
                        continue;
                    }
                    totalFilesToScan += Directory.GetFiles(folder.Path).Length;
                }
                _logger.LogInformation($"Znaleziono łącznie {totalFilesToScan} plików do przetworzenia");
                int globalProcessedCount = 0;
                foreach (var folder in scannedFolders)
                {
                    if (!folder.IsActive) continue;
                    if (!Directory.Exists(folder.Path))
                    {
                        _logger.LogWarning("⚠️ Folder not found: {Path}", folder.Path);
                        continue;
                    }
                    var files = Directory.EnumerateFiles(
                        path: folder.Path,
                        searchPattern: "*.*",
                        searchOption: SearchOption.AllDirectories
                    );
                    foreach (var filePath in files)
                    {

                        try
                        {

                            var extension = Path.GetExtension(filePath).ToLower();
                            if (!allowedExtensions.Contains(extension))
                            {
                                continue;
                            }
                            var existingAssetByPath = await assetRepo.GetAssetByPathAsync(filePath, stoppingToken);

                            if (existingAssetByPath != null)
                            {
                                continue;
                            }

                            var (thumbnailPath, metadata) = await GenerateThumbnailAsync(filePath, extension);
                            var (fileSize, lastModified) = GetFileSizeAndLastModifiedDate(filePath);
                            var fileHash = await ComputeFileHashAsync(filePath, fileSize, stoppingToken);
                            Asset newAsset = Asset.Create(folder.Id, filePath, fileSize, DetermineFileType(extension), thumbnailPath, lastModified, fileHash, metadata?.Width, metadata?.Height, metadata?.DominantColor, metadata?.BitDepth, metadata?.HasAlphaChannel);
                            if (fileHash != null)
                            {
                                var existingAssetByHash = await assetRepo.GetAssetByFileHashAsync(fileHash ?? "", stoppingToken);
                                if (existingAssetByHash != null)
                                {
                                    _logger.LogInformation($"⏭️ Found duplicate asset: {newAsset.FileName} on Path: {newAsset.FilePath}.\nAssigning to parent: ){existingAssetByHash.FileName} on Path: {existingAssetByHash.FilePath}");
                                    var rootId = existingAssetByHash.ParentAssetId ?? existingAssetByHash.Id;
                                    newAsset.ParentAssetId = rootId;
                                }
                            }
                            await assetRepo.AddAssetAsync(newAsset, stoppingToken);
                            _logger.LogInformation("✅ Added new asset: {FileName}", newAsset.FileName);
                            globalProcessedCount++;
                            if (globalProcessedCount % 50 == 0)
                            {
                                await _hubContext.Clients.All.ReceiveProgress($"Scanning folder 📂{folder.Path}: Total files scanned ({globalProcessedCount}/{totalFilesToScan})", totalFilesToScan, globalProcessedCount);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            _logger.LogInformation("Scanner loop was cancelled.");
                            break;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to process file {FilePath}. Skipping.", filePath);
                        }

                    }

                }
            }
        }

        private string DetermineFileType(string extension)
        {
            return extension switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" => FileTypes.Image,
                ".blend" or ".fbx" or ".obj" or ".ztl" or ".zpr" => FileTypes.Model,
                ".psd" or ".ai" or ".svg" => FileTypes.Texture,
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
                        Directory.CreateDirectory(thumbsFolder);

                        var fullSavePath = Path.Combine(thumbsFolder, uniqueFileName);

                        await image.SaveAsWebpAsync(fullSavePath);

                        // Zwracamy ścieżkę relatywną dla frontendu (np. "/thumbnails/abc.webp")
                        var webPath = Path.Combine("/", _scannerSettings.ThumbnailsFolder, uniqueFileName).Replace("\\", "/");

                        return (webPath, metadata);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to generate thumbnail for image {Path}: {Message}", filePath, ex.Message);
                }
            }

            if (extension is ".blend")
            {
                // TODO: Wywołaj Blender w trybie headless
                return (_scannerSettings.PlaceholderThumbnail, null);
            }

            return (_scannerSettings.PlaceholderThumbnail, null);
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


