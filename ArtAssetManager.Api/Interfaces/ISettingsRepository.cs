using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
    public interface ISettingsRepository
    {
        Task<IEnumerable<ScanFolder>> GetScanFoldersAsync(CancellationToken cancellationToken);
        Task<ScanFolder> AddScanFolderAsync(ScanFolder folder, CancellationToken cancellationToken);
        Task DeleteScanFolderAsync(int id, CancellationToken cancellationToken);
        Task<ScanFolder?> GetScanFolderByIdAsync(int id, CancellationToken cancellationToken);
        Task<ScanFolder> UpdateScanFolderStatusAsync(int id, bool isActive, CancellationToken cancellationToken);
        Task<string?> GetValueAsync(string key, CancellationToken ct = default);
        Task SetValueAsync(string key, string value, CancellationToken ct = default);
        Task<List<string>> GetAllowedExtensionsAsync(CancellationToken ct = default);
        Task SetAllowedExtensionsAsync(List<string> extensions, CancellationToken ct = default);

    }
}