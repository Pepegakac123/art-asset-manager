using ArtAssetManager.Api.Config;
using Microsoft.Extensions.Options;

namespace ArtAssetManager.Api.Services
{
    public class StartupInitializationService : IHostedService
    {
        private readonly ILogger<StartupInitializationService> _logger;
        private readonly ScannerSettings _settings;

        public StartupInitializationService(
            ILogger<StartupInitializationService> logger,
            IOptions<ScannerSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔧 Running startup initialization...");

            var thumbnailsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                _settings.ThumbnailsFolder
            );

            if (!Directory.Exists(thumbnailsPath))
            {
                Directory.CreateDirectory(thumbnailsPath);
                _logger.LogInformation("📁 Created: {Path}", thumbnailsPath);
            }
            var placeholderPath = Path.Combine(thumbnailsPath, "placeholder.png");
            // TODO: Skopiuj placeholder
            if (!File.Exists(placeholderPath))
            {
                _logger.LogWarning("⚠️ Placeholder not found - thumbnails will use relative path");

            }

            _logger.LogInformation("✅ Initialization complete!");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
