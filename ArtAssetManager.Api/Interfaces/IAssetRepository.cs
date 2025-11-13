using ArtAssetManager.Api.Data.Helpers;
using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
    public interface IAssetRepository
    {

        Task<Asset?> GetAssetByIdAsync(int id);
        Task<Asset?> GetAssetByPathAsync(string path);
        Task<Asset> AddAssetAsync(Asset asset);
        Task UpdateAssetTagsAsync(int assetId, IEnumerable<Tag> tags);
        Task<PagedResult<Asset>> GetPagedAssetsAsync(
       int pageNumber,
       int numOfItems,
       string? fileName,
       List<string>? fileType,
       List<string>? tags,
       bool matchAll = false,
       string? sortBy = null,
       bool sortDesc = false,
       DateTime? dateFrom = null,
       DateTime? dateTo = null
   );


    }
}