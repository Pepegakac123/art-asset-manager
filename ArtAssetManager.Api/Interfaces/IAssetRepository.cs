using ArtAssetManager.Api.Data.Helpers;
using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
  public interface IAssetRepository
  {

    Task<Asset?> GetAssetByIdAsync(int id);
    Task<Asset?> GetAssetByPathAsync(string path);
    Task<Asset?> GetAssetByFileHashAsync(string fileHash);
    Task<Asset> AddAssetAsync(Asset asset);
    Task UpdateAssetTagsAsync(int assetId, IEnumerable<Tag> tags);
    Task<PagedResult<Asset>> GetPagedAssetsAsync(
  AssetQueryParameters queryParams);
    Task<IEnumerable<Asset>> GetAssetVersionAsync(int assetId);
    Task LinkAssetToParentAsync(int assetId, int parentId);
    Task SetAssetRatingAsync(int assetId, int rating);


  }
}