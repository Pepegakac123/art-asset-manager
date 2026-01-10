using ArtAssetManager.Api.Enums;

namespace ArtAssetManager.Api.Services
{
    // Interfejs do sterowania procesem skanowania.
    // Oddziela logikę "kiedy skanować" (Trigger) od "jak skanować" (ScannerService).
    public interface IScannerTrigger
    {
        // Zgłasza żądanie rozpoczęcia skanowania (np. z API lub harmonogramu)
        Task TriggerScanAsync(ScanMode mode);
        
        // Strumień zdarzeń, na który oczekuje BackgroundService
        IAsyncEnumerable<ScanMode> WaitForTriggersAsync(CancellationToken cancellationToken);
        
        // Aktualny status (Thread-safe flaga)
        bool IsScanning { get; }
        void SetScanningStatus(bool isScanning);
    }
}
