using ArtAssetManager.Api.Data.Helpers;
using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
    public interface IMaterialSetRepository
    {
        Task<IEnumerable<MaterialSet>> GetAllAsync(CancellationToken cancellationToken);
        Task<MaterialSet> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<MaterialSet> AddAsync(MaterialSet materialSet, CancellationToken cancellationToken);
        Task<MaterialSet> UpdateAsync(int id, MaterialSet updateData, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);

        Task AddAssetToSetAsync(int assetId, int materialSetId, CancellationToken cancellationToken);
        Task RemoveAssetFromSetAsync(int assetId, int materialSetId, CancellationToken cancellationToken);

        Task<PagedResult<Asset>> GetAssetsForSetAsync(int setId, AssetQueryParameters queryParams, CancellationToken cancellationToken);

        Task<int> CountByMaterialSetIdAsync(int materialSetId, CancellationToken cancellationToken);
    }
}
