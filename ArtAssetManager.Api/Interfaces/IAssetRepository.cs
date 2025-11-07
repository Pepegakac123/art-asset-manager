using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Interfaces
{
    public interface IAssetRepository
    {

        Task<Asset?> GetAssetByIdAsync(int id);
        Task<Asset?> GetAssetByPathAsync(string path);
        Task<Asset> AddAssetAsync(Asset asset);
        Task UpdateAssetTagsAsync(int assetId, Tag[] tags);
        Task<IEnumerable<Asset>> GetPagedAssetsAsync(int pageNumber, int numOfItems);
        // TODO: Paginacja z metadanymi
        // Todo: Dodanie opcjonalnego parametru w getPagedAssetsSync do filtrowania po FileType
        Task<IEnumerable<Asset>> SearchAssetsByTagsAsync(Tag[] tags, bool matchAll = false);


    }
}