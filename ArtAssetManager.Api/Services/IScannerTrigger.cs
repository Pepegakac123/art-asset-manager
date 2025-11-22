using ArtAssetManager.Api.Enums;

namespace ArtAssetManager.Api.Services
{
    public interface IScannerTrigger
    {
        Task TriggerScanAsync(ScanMode mode);
        IAsyncEnumerable<ScanMode> WaitForTriggersAsync(CancellationToken cancellationToken);
        bool IsScanning { get; }
        void SetScanningStatus(bool isScanning);
    }
}