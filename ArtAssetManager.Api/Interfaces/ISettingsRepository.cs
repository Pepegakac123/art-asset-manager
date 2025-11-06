using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
    public interface ISettingsRepository
    {
        Task<IEnumerable<ScanFolder>> GetScanFoldersAsync();
        Task<ScanFolder> AddScanFolderAsync(ScanFolder folder);
        Task DeleteScanFolderAsync(int id);
        Task<ScanFolder?> GetScanFolderByIdAsync(int id);

        // TODO: Updatowanie Statusu aktywnosci folderow zamiast ich usuwania kompletnego
        // Todo: Task MarkAsDeletedAsync(int id)

    }
}