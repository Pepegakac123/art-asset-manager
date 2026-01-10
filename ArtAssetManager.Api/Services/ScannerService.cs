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
    
    // Główny serwis działający w tle (Worker Service)
    // Odpowiada za fizyczne przeszukiwanie dysku, analizę plików i aktualizację bazy danych
    public class ScannerService : BackgroundService
    {
        private readonly ILogger<ScannerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ScannerSettings _scannerSettings;
        private readonly IHubContext<ScanHub, IScanClient> _hubContext; // Do wysyłania powiadomień SignalR
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IScannerTrigger _trigger; // Kanał komunikacyjny z wyzwalaczem

        public ScannerService(ILogger<ScannerService> logger, IServiceScopeFactory scopeFactory, IOptions<ScannerSettings> scannerSettings, IHubContext<ScanHub, IScanClient> hubContext, IScannerTrigger trigger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _scannerSettings = scannerSettings.Value;
            _hubContext = hubContext;
            _trigger = trigger;
            _webHostEnvironment = webHostEnvironment;

        }

        // Metoda startowa serwisu
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Scanner Service started!");
            
            // Uruchomienie harmonogramu (Scheduler) w osobnym wątku ("Fire and forget")
            // Dzięki temu pętla główna (poniżej) nie jest blokowana przez Timer
            _ = StartSchedulerAsync(stoppingToken); 

            // Główna pętla nasłuchująca na sygnały skanowania (z API lub Schedulera)
            // Wykorzystuje IAsyncEnumerable (Channel) do efektywnego oczekiwania
            await foreach (var mode in _trigger.WaitForTriggersAsync(stoppingToken))
            {
                _logger.LogInformation("Trigger received: {Mode}", mode);
                try
                {
                    _trigger.SetScanningStatus(true);
                    await PerformFullScanAsync(stoppingToken); // Wykonanie właściwego skanowania
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

        // Harmonogram automatycznego skanowania
        private async Task StartSchedulerAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduler started.");
            _logger.LogInformation("Starting Initial Scan.");
            
            // Skan startowy przy uruchomieniu aplikacji
            await _trigger.TriggerScanAsync(ScanMode.Initial);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                // Pętla nieskończona z opóźnieniem (co 25 minut)
                await Task.Delay(TimeSpan.FromMinutes(25), stoppingToken);

                _logger.LogDebug(" Scheduler: Sending Scanning Request at {Time}", DateTime.UtcNow);

                await _trigger.TriggerScanAsync(ScanMode.Scheduled);
            }
        }

        // === CORE LOGIC: PEŁNY SKAN ===
        private async Task PerformFullScanAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🔍 Scanner iteration at {Time}", DateTime.UtcNow);

            // Tworzymy nowy Scope, ponieważ ScannerService jest Singletonem,
            // a Repozytoria (DbContext) są Scoped (żyją krócej).
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

                // Powiadomienie UI: Start indeksowania
                await _hubContext.Clients.All.ReceiveScanStatus(ScaningStatusEnumToString(ScanStatus.Scanning));
                await _hubContext.Clients.All.ReceiveProgress("Indexing files...", 0, 0);

                // KROK 1: Szybkie zebranie wszystkich ścieżek plików
                foreach (var folder in activeFolders)
                {
                    try
                    {
                        // SearchOption.AllDirectories = Rekurencyjne przeszukiwanie podfolderów
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
                // KROK 2: PRZETWARZANIE PLIKÓW (Jedna pętla po wszystkich zebranych plikach)
                // ==============================================================================

                int globalProcessedCount = 0;

                // Sygnał startowy dla UI (0%)
                await _hubContext.Clients.All.ReceiveProgress("Starting analysis...", totalFilesToScan, 0);

                foreach (var filePath in allFilePaths)
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    globalProcessedCount++; // Inkrementujemy ZAWSZE, aby pasek postępu był rzetelny

                    try
                    {
                        var extension = Path.GetExtension(filePath).ToLower();

                        // Pomijamy pliki z nieobsługiwanym rozszerzeniem
                        if (!allowedExtensions.Contains(extension))
                        {
                            // Raportuj postęp co 50 plików, żeby nie "zamulić" sieci
                            if (globalProcessedCount % 50 == 0) await SendProgress(filePath, totalFilesToScan, globalProcessedCount);
                            continue;
                        }

                        // Sprawdzenie czy plik już istnieje w bazie
                        var existingAssetByPath = await assetRepo.GetAssetByPathAsync(filePath, stoppingToken);
                        if (existingAssetByPath != null)
                        {
                            bool wasModified = false;
                            
                            // Logika "Self-Healing": Plik zmienił rozszerzenie na niedozwolone? -> Trash
                            if (!allowedExtensions.Contains(existingAssetByPath.FileExtension))
                            {
                                await assetRepo.SoftDeleteAssetAsync(existingAssetByPath.Id, stoppingToken);
                                _logger.LogInformation("🗑️ Moved to trash (extension no longer allowed): {FileName}", existingAssetByPath.FileName);
                            }
                            // Plik wrócił do łask (rozszerzenie znów dozwolone)? -> Restore
                            if (existingAssetByPath.IsDeleted && allowedExtensions.Contains(existingAssetByPath.FileExtension))
                            {
                                await assetRepo.RestoreAssetAsync(existingAssetByPath.Id, stoppingToken);
                                _logger.LogInformation("♻️ Restored from trash (extension now allowed): {FileName}", existingAssetByPath.FileName);
                            }
                            
                            // Logika przypisania do folderu (przydatne przy zagnieżdżonych bibliotekach)
                            var correctFolder = activeFolders
                                    .OrderByDescending(f => f.Path.Length) // Najbardziej specyficzna ścieżka wygrywa
                                    .FirstOrDefault(f => filePath.StartsWith(f.Path));
                            
                            if (correctFolder != null)
                            {
                                if (existingAssetByPath.ScanFolderId != correctFolder.Id)
                                {
                                    _logger.LogWarning("🩹 Self-Healing: Asset {FileName} re-adopted from FolderId {OldId} to {NewId}",
                                        existingAssetByPath.FileName, existingAssetByPath.ScanFolderId, correctFolder.Id);

                                    existingAssetByPath.ScanFolderId = correctFolder.Id;

                                    wasModified = true;
                                }
                            }
                            if (wasModified)
                            {
                                await assetRepo.UpdateAssetAsync(existingAssetByPath, stoppingToken);
                            }
                            if (globalProcessedCount % 50 == 0) await SendProgress(filePath, totalFilesToScan, globalProcessedCount);
                            continue;
                        }

                        // === NOWY ASSET ===
                        // 1. Generowanie miniatury i analiza obrazu (ImageSharp)
                        var (thumbnailPath, metadata) = await GenerateThumbnailAsync(filePath, extension);
                        // 2. Pobranie metadanych plikowych
                        var (fileSize, lastModified) = GetFileSizeAndLastModifiedDate(filePath);
                        // 3. Obliczenie hasha (dla wykrywania duplikatów)
                        var fileHash = await ComputeFileHashAsync(filePath, fileSize, stoppingToken);

                        var sourceFolder = activeFolders
                            .OrderByDescending(f => f.Path.Length)
                            .FirstOrDefault(f => filePath.StartsWith(f.Path));

                        if (sourceFolder == null) continue; // Should not happen

                        // 4. Utworzenie encji
                        Asset newAsset = Asset.Create(sourceFolder.Id, filePath, fileSize, DetermineFileType(extension), thumbnailPath, lastModified, fileHash, metadata?.Width, metadata?.Height, metadata?.DominantColor, metadata?.BitDepth, metadata?.HasAlphaChannel);

                        // 5. Automatyczne łączenie duplikatów (jeśli hash pasuje do istniejącego)
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
                        
                        // --- PROGRESS REPORTING (Smart Update) ---
                        // Raportuj rzadziej (co 10 plików), żeby nie obciążać SignalR
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

                // ZAWSZE raportuj koniec skanowania
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

        // Mapowanie rozszerzenia na ogólny typ pliku (używane do filtrowania w UI)
        private string DetermineFileType(string extension)
        {
            return extension switch
            {
                // Images
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" => FileTypes.Image,

                // 3D Models
                ".blend" or ".blend1" or ".fbx" or ".obj" or ".max" or ".ma" or ".mb" => FileTypes.Model,

                // Sculpting
                ".ztl" or ".zpr" or ".zbr" => FileTypes.Model,

                // Procedural/Houdini
                ".hip" or ".hipnc" or ".hiplc" => FileTypes.Model,

                // Game Engine Assets
                ".uasset" or ".umap" or ".unity" or ".prefab" or ".mat" or ".asset" => FileTypes.Model,

                // Textures/Materials
                ".psd" or ".psb" or ".ai" or ".eps" or ".exr" or ".hdr" or ".tif" or ".tiff" => FileTypes.Texture,

                // Substance (Textures/Materials)
                ".spp" or ".sbs" or ".sbsar" => FileTypes.Texture,

                _ => FileTypes.Other
            };
        }
        private (long, DateTime) GetFileSizeAndLastModifiedDate(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return (fileInfo.Length, fileInfo.LastWriteTimeUtc);
        }

        // Generowanie miniatur przy użyciu biblioteki ImageSharp
        private async Task<(string ThumbnailPath, AssetMetadata? Metadata)> GenerateThumbnailAsync(string filePath, string extension)
        {
            var ext = extension.ToLowerInvariant();

            // DLA OBRAZKÓW: Generujemy prawdziwą miniaturę
            if (ext is ".jpg" or ".jpeg" or ".png" or ".webp" or ".bmp" or ".tga")
            {
                try
                {
                    await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

                    using (var image = await Image.LoadAsync(stream))
                    {
                        // Ekstrakcja metadanych (rozmiar, głębia, alpha, kolor)
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

                        // Skalowanie do szerokości 400px (proporcjonalnie)
                        image.Mutate(x => x.Resize(400, 0));

                        var uniqueFileName = $"{Guid.NewGuid()}.webp";
                        var thumbsFolder = Path.Combine(_webHostEnvironment.WebRootPath, _scannerSettings.ThumbnailsFolder);

                        if (!Directory.Exists(thumbsFolder))
                        {
                            Directory.CreateDirectory(thumbsFolder);
                        }

                        var fullSavePath = Path.Combine(thumbsFolder, uniqueFileName);
                        await image.SaveAsWebpAsync(fullSavePath);

                        var webPath = Path.Combine("/", _scannerSettings.ThumbnailsFolder, uniqueFileName).Replace("\", "/");

                        return (webPath, metadata);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to generate thumbnail for image {Path}: {Message}", filePath, ex.Message);
                    // Fallback do placeholdera w razie błędu
                }
            }

            // DLA INNYCH PLIKÓW (3D, Tekstury): Używamy statycznych ikon (Placeholderów)
            const string placeholderDir = "placeholders";
            const string defaultPlaceholder = "generic_placeholder.webp";

            if (!_scannerSettings.PlaceholderMappings.TryGetValue(ext, out var placeholderFile))
            {
                // Jak nie znajdziemy dedykowanej ikony (np. dla .c4d), bierzemy domyślną
                placeholderFile = defaultPlaceholder;
            }

            var finalPath = Path.Combine("/", _scannerSettings.ThumbnailsFolder, placeholderDir, placeholderFile)
                                .Replace("\", "/");

            return (finalPath, null);
        }

        // Obliczanie hasha SHA256 w celu wykrywania duplikatów
        private async Task<string?> ComputeFileHashAsync(string filePath, long fileSizeBytes, CancellationToken cancellationToken)
        {
            if (!_scannerSettings.EnableHashing)
                return null;
            
            // Pomijamy bardzo duże pliki ze względu na wydajność
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

        // Algorytm wyznaczania koloru dominującego
        private string GetDominantColor(Image image)
        {
            // Zdefiniowana paleta 16 podstawowych kolorów, do których przyrównujemy wynik
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

            // Zmniejszamy obraz do 50x50, żeby przyspieszyć obliczenia
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

            // 1. Znajdź najczęściej występujący pixel
            var dominant = colorCount.OrderByDescending(c => c.Value).First().Key;

            // 2. Znajdź najbliższy kolor z naszej palety (euklidesowa odległość kolorów)
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