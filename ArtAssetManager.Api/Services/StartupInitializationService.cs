using ArtAssetManager.Api.Config;
using Microsoft.Extensions.Options;

namespace ArtAssetManager.Api.Services
{
    // Serwis uruchamiany jednorazowo przy starcie aplikacji (HostedService)
    // Odpowiada za przygotowanie środowiska (tworzenie folderów, sprawdzanie plików konfiguracyjnych)
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
                // 1. Upewnij się, że folder na miniatury istnieje
                var thumbsPath = Path.Combine(_env.WebRootPath, _settings.ThumbnailsFolder);

                if (!Directory.Exists(thumbsPath))
                {
                    Directory.CreateDirectory(thumbsPath);
                    _logger.LogInformation("📁 Created thumbnails directory: {Path}", thumbsPath);
                }
                
                // 2. Sprawdź obecność domyślnego placeholdera (ważne dla UI)
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
