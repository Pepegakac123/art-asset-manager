using Microsoft.Extensions.Hosting;

namespace ArtAssetManager.Api.Services
{
    public class ScannerService : BackgroundService
    {
        private readonly ILogger<ScannerService> _logger;
        public ScannerService(ILogger<ScannerService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Scanner Service started!");
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("🔍 Scanner iteration at {Time}", DateTime.UtcNow);
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