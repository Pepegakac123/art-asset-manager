using System.Threading.Channels;
using ArtAssetManager.Api.Enums;
using ArtAssetManager.Api.Interfaces;

namespace ArtAssetManager.Api.Services
{
    public class ScannerTriggerService : IScannerTrigger // SINGLETONH
    {
        private readonly Channel<ScanMode> _channel;
        private readonly ILogger<ScannerTriggerService> _logger;
        public bool IsScanning { get; private set; } = false;
        public ScannerTriggerService(ILogger<ScannerTriggerService> logger)
        {
            _logger = logger;
            {
                var options = new BoundedChannelOptions(1)
                {
                    FullMode = BoundedChannelFullMode.DropWrite
                }; // 1 prośba nar az
                _channel = Channel.CreateBounded<ScanMode>(options);
            }
        }

        // Wywołanie kontrolera
        public async Task TriggerScanAsync(ScanMode mode)
        {
            if (_channel.Writer.TryWrite(mode))
            {
                _logger.LogInformation("Triggered scan request: {Mode}", mode);
            }
            else
            {
                _logger.LogWarning("Scan request ignored - scanner is busy or queue full.");
            }
            await Task.CompletedTask;
        }

        public IAsyncEnumerable<ScanMode> WaitForTriggersAsync(CancellationToken cancellationToken)
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }

        public void SetScanningStatus(bool isScanning)
        {
            IsScanning = isScanning;
        }

    }

}