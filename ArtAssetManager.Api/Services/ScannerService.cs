using ArtAssetManager.Api.Config;
using ArtAssetManager.Api.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ArtAssetManager.Api.Services
{
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
                    }
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {

                _logger.LogInformation("Scanner iteration cancelled - shutting down.");
            }

            _logger.LogInformation("⛔ Scanner Service stopped.");
        }
    }
}