using System.Runtime.CompilerServices;
using System.Text.Json;
using ArtAssetManager.Api.Config;
using ArtAssetManager.Api.Data;
using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
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
        public ScannerService(ILogger<ScannerService> logger, IServiceScopeFactory scopeFactory, IOptions<ScannerSettings> scannerSettings)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _scannerSettings = scannerSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Scanner Service started!");
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("🔍 Scanner iteration at {Time}", DateTime.UtcNow);
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var assetRepo = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
                        var settingsRepo = scope.ServiceProvider.GetRequiredService<ISettingsRepository>();

                        var scannedFolders = await settingsRepo.GetScanFoldersAsync();
                        _logger.LogInformation("📂 Retrieved {Count} folders", scannedFolders.Count());

                        foreach (var folder in scannedFolders)
                        {
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
                                var extension = Path.GetExtension(filePath).ToLower();
                                if (!_scannerSettings.AllowedExtensions.Contains(extension))
                                {
                                    continue;
                                }
                                var existingAsset = await assetRepo.GetAssetByPathAsync(filePath);
                                if (existingAsset != null)
                                {
                                    continue;
                                }
                                var (thumbnailPath, metadataJson) = await GenerateThumbnailAsync(filePath, extension);
                                var (fileSize, lastModified) = GetFileSizeAndLastModifiedDate(filePath);
                                var fileHash = await ComputeFileHashAsync(filePath, fileSize);
                                Asset newAsset = Asset.Create(folder.Id, filePath, fileSize, DetermineFileType(extension), thumbnailPath, lastModified, fileHash);
                                newAsset.MetadataJson = metadataJson;
                                await assetRepo.AddAssetAsync(newAsset);
                                _logger.LogInformation("✅ Added new asset: {FileName}", newAsset.FileName);
                            }

                        }
                    }
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {

                _logger.LogInformation("Scanner iteration cancelled - shutting down.");
            }

            _logger.LogInformation("⛔ Scanner Service stopped.");
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

        private async Task<(string ThumbnailPath, string? MetadataJson)> GenerateThumbnailAsync(string filePath, string extension)
        {
            if (extension is ".jpg" or ".jpeg" or ".png" or ".webp")
            {

                try
                {
                    await using var stream = File.OpenRead(filePath);
                    using (var image = await Image.LoadAsync(stream))
                    {
                        bool hasAlphaChannel = image.PixelType.AlphaRepresentation.HasValue;
                        int bitDepth = image.PixelType.BitsPerPixel;
                        var metadata = new
                        {
                            Width = image.Width,
                            Height = image.Height,
                            DominantColor = GetDominantColor(image),
                            BitDepth = bitDepth,
                            HasAlphaChannel = hasAlphaChannel
                        };
                        string metadataJson = JsonSerializer.Serialize(metadata);
                        image.Mutate(x => x.Resize(400, 0));
                        var uniqueFileName = $"{Guid.NewGuid()}.webp";
                        var fullSavePath = Path.Combine(Directory.GetCurrentDirectory(), _scannerSettings.ThumbnailsFolder, uniqueFileName);
                        var relativeDir = Path.GetDirectoryName(_scannerSettings.PlaceholderThumbnail).Replace("\\", "/");
                        var relativePath = $"{relativeDir}/{uniqueFileName}";
                        await image.SaveAsWebpAsync(fullSavePath);
                        return (relativePath, metadataJson);
                    }
                }
                catch (Exception ex)
                {

                    _logger.LogInformation("The image was not loaded: {Message}", ex.Message);
                    return (_scannerSettings.PlaceholderThumbnail, null);
                }
            }

            if (extension is ".blend")
            {
                // TODO: Wywołaj Blender w trybie headless
                return (_scannerSettings.PlaceholderThumbnail, null);
            }

            return (_scannerSettings.PlaceholderThumbnail, null);
        }

        private async Task<string?> ComputeFileHashAsync(string filePath, long fileSizeBytes)
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
            var hash = await sha256.ComputeHashAsync(stream);
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


