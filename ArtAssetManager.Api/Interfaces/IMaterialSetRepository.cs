using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
    public interface IMaterialSetRepository
    {
        Task<IEnumerable<MaterialSet>> GetAllAsync();
        Task<MaterialSet> GetByIdAsync(int id);
        Task<MaterialSet> AddAsync(MaterialSet materialSet);
        Task UpdateAsync(MaterialSet materialSet);
        Task DeleteAsync(MaterialSet materialSet);

        Task AddAssetToSetAsync(int assetId, int materialSetId);
        Task RemoveAssetFromSetAsync(int assetId, int materialSetId);

        Task<Asset> GetAssetsForSetAsync(int setId, int pageNumber, int pageSize);
    }
}