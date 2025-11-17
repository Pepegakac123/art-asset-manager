using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
    public interface ISettingsRepository
    {
        Task<IEnumerable<ScanFolder>> GetScanFoldersAsync(CancellationToken cancellationToken);
        Task<ScanFolder> AddScanFolderAsync(ScanFolder folder, CancellationToken cancellationToken);
        Task DeleteScanFolderAsync(int id, CancellationToken cancellationToken);
        Task<ScanFolder?> GetScanFolderByIdAsync(int id, CancellationToken cancellationToken);

        Task<ScanFolder> ToggleScanFolderActiveAsync(int id, CancellationToken cancellationToken);

    }
}