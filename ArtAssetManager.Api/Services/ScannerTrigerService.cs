using System.Threading.Channels;
using ArtAssetManager.Api.Enums;
using ArtAssetManager.Api.Interfaces;

namespace ArtAssetManager.Api.Services
{
    // Serwis (Singleton) zarządzający kolejką żądań skanowania.
    // Działa jako pośrednik między API (Controller) a Workerem (ScannerService).
    // Używa wzorca Producer-Consumer opartego na System.Threading.Channels.
    public class ScannerTriggerService : IScannerTrigger // Rejestrowany jako SINGLETON
    {
        private readonly Channel<ScanMode> _channel;
        private readonly ILogger<ScannerTriggerService> _logger;
        public bool IsScanning { get; private set; } = false;
        
        public ScannerTriggerService(ILogger<ScannerTriggerService> logger)
        {
            _logger = logger;
            {
                // Ograniczamy kolejkę do 1 elementu. 
                // Jeśli skaner pracuje, nowe żądanie typu "Skanuj" zostanie odrzucone (DropWrite),
                // aby nie kolejkować nieskończonej liczby skanów.
                var options = new BoundedChannelOptions(1)
                {
                    FullMode = BoundedChannelFullMode.DropWrite
                }; 
                _channel = Channel.CreateBounded<ScanMode>(options);
            }
        }

        // Metoda producenta: Wrzuca żądanie do kanału (wywoływana przez kontroler)
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

        // Metoda konsumenta: Zwraca strumień, na którym nasłuchuje ScannerService
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
