using ArtAssetManager.Api.Data.Helpers;
using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
    public interface IMaterialSetRepository
    {
        Task<IEnumerable<MaterialSet>> GetAllAsync();
        Task<MaterialSet> GetByIdAsync(int id);
        Task<MaterialSet> AddAsync(MaterialSet materialSet);
        Task<MaterialSet> UpdateAsync(int id, MaterialSet updateData);
        Task DeleteAsync(int id);

        Task AddAssetToSetAsync(int assetId, int materialSetId);
        Task RemoveAssetFromSetAsync(int assetId, int materialSetId);

        Task<PagedResult<Asset>> GetAssetsForSetAsync(int setId, AssetQueryParameters queryParams);
    }
}