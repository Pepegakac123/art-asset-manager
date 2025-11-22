using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
    public interface ISavedSearchRepository
    {
        Task<IEnumerable<SavedSearch>> GetAllAsync(CancellationToken cancellationToken);
        Task<SavedSearch> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<SavedSearch> AddAsync(SavedSearch savedSearch, CancellationToken cancellationToken);
        Task<SavedSearch> UpdateAsync(int id, SavedSearch updateData, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}