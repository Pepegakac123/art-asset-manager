using System.Runtime.CompilerServices;
using ArtAssetManager.Api.Config;
using ArtAssetManager.Api.Data;
using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
                                var thumbnailPath = await GenerateThumbnailAsync(filePath, extension);
                                var (fileSize, lastModified) = GetFileSizeAndLastModifiedDate(filePath);
                                var fileHash = await ComputeFileHashAsync(filePath, fileSize);
                                Asset newAsset = Asset.Create(folder.Id, filePath, fileSize, DetermineFileType(extension), thumbnailPath, lastModified, fileHash);
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

        private async Task<string> GenerateThumbnailAsync(string filePath, string extension)
        {
            if (extension is ".jpg" or ".jpeg" or ".png")
            {
                // TODO: Użyj ImageSharp do generowania miniaturki
                return _scannerSettings.PlaceholderThumbnail;
            }

            if (extension is ".blend")
            {
                // TODO: Wywołaj Blender w trybie headless
                return _scannerSettings.PlaceholderThumbnail;
            }

            return _scannerSettings.PlaceholderThumbnail;
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
    }
}


