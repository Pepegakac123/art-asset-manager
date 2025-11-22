using ArtAssetManager.Api.Config;
using Microsoft.Extensions.Options;

namespace ArtAssetManager.Api.Services
{
    public class StartupInitializationService : IHostedService
    {
        private readonly ILogger<StartupInitializationService> _logger;
        private readonly ScannerSettings _settings;
        private readonly IWebHostEnvironment _env;

        public StartupInitializationService(
            ILogger<StartupInitializationService> logger,
            IOptions<ScannerSettings> settings,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _settings = settings.Value;
            _env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔧 Running startup initialization...");

            try
            {
                var thumbsPath = Path.Combine(_env.WebRootPath, _settings.ThumbnailsFolder);

                if (!Directory.Exists(thumbsPath))
                {
                    Directory.CreateDirectory(thumbsPath);
                    _logger.LogInformation("📁 Created thumbnails directory: {Path}", thumbsPath);
                }
                var placeholderPath = Path.Combine(_env.WebRootPath, _settings.PlaceholderThumbnail.TrimStart('/', '\\'));
                if (!File.Exists(placeholderPath))
                {
                    _logger.LogWarning("⚠️ Placeholder not found at: {Path} - Make sure to put 'placeholder.png' in wwwroot/thumbnails!", placeholderPath);
                }
                else
                {
                    _logger.LogInformation("✅ Placeholder image found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during startup initialization");
            }

            _logger.LogInformation("✅ Initialization complete!");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}