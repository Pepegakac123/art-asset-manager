using ArtAssetManager.Api.Data.Helpers;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
    public interface IAssetRepository
    {

        Task<Asset?> GetAssetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Asset?> GetAssetByPathAsync(string path, CancellationToken cancellationToken);
        Task<Asset?> GetAssetByFileHashAsync(string fileHash, CancellationToken cancellationToken);
        Task<Asset> AddAssetAsync(Asset asset, CancellationToken cancellationToken);
        Task<Asset?> UpdateAssetMetadataAsync(int id, PatchAssetRequest request, CancellationToken cancellationToken);
        Task UpdateAssetTagsAsync(int assetId, IEnumerable<Tag> tags, CancellationToken cancellationToken);
        Task BulkUpdateAssetTagsAsync(List<int> assetIds, IEnumerable<Tag> tags, CancellationToken cancellationToken);
        Task UpdateAssetAsync(Asset asset, CancellationToken ct = default);
        Task<PagedResult<Asset>> GetPagedAssetsAsync(
  AssetQueryParameters queryParams, CancellationToken cancellationToken);
        Task<IEnumerable<Asset>> GetAssetVersionAsync(int assetId, CancellationToken cancellationToken);
        Task LinkAssetToParentAsync(int assetId, int parentId, CancellationToken cancellationToken);
        Task SetAssetRatingAsync(int assetId, int rating, CancellationToken cancellationToken);
        Task ToggleAssetFavoriteAsync(int assetId, CancellationToken cancellationToken);
        Task<PagedResult<Asset>> GetFavoritesAssetsAsync(AssetQueryParameters queryParams, CancellationToken cancellationToken);
        Task<PagedResult<Asset>> GetUncategorizedAssetsAsync(AssetQueryParameters queryParams, CancellationToken cancellationToken);

        Task SoftDeleteAssetAsync(int assetId, CancellationToken cancellationToken);
        Task BulkSoftDeleteAssetsAsync(List<int> assetIds, CancellationToken cancellationToken);
        Task RestoreAssetAsync(int assetId, CancellationToken cancellationToken);
        Task BulkRestoreAssetsAsync(List<int> assetIds, CancellationToken cancellationToken);
        Task<PagedResult<Asset>> GetDeletedAssetsAsync(AssetQueryParameters queryParams, CancellationToken cancellationToken);
        Task PermanentDeleteAssetAsync(int assetId, CancellationToken cancellationToken);
        Task BulkPermanentDeleteAssetsAsync(List<int> assetIds, CancellationToken cancellationToken);

        Task<LibraryStatsDto> GetStatsAsync(CancellationToken cancellationToken);
        Task<SidebarStatsDto> GetSidebarStatsAsync(CancellationToken cancellationToken);

        Task<List<string>> GetColorsListAsync(CancellationToken cancellationToken);



    }
}
