using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace ArtAssetManager.Api.Data.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AssetDbContext _context;
        public AssetRepository(AssetDbContext context)
        {
            _context = context;
        }
        public async Task<Asset?> GetAssetByIdAsync(int id)
        {
            return await _context.Assets.FindAsync(id);
        }
        public async Task<Asset?> GetAssetByPathAsync(string path)
        {
            return await _context.Assets.FirstOrDefaultAsync(x => x.FilePath == path);
        }
        public async Task<Asset> AddAssetAsync(Asset asset)
        {
            _context.Add(asset);
            await _context.SaveChangesAsync();
            return asset;
        }
        public async Task UpdateAssetTagsAsync(int assetId, Tag[] tags)
        {
            var asset = await _context.Assets
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == assetId);
            if (asset == null) throw new KeyNotFoundException($"Asset {assetId} not found");
            asset.Tags.Clear();
            foreach (var tag in tags)
            {
                asset.Tags.Add(tag);
            }
            await _context.SaveChangesAsync();

        }
        // TODO (Phase 4): Refactor to return PagedResult<Asset> with metadata
        // - TotalItems, TotalPages, HasNext, HasPrevious
        public async Task<IEnumerable<Asset>> GetPagedAssetsAsync(int pageNumber, int numOfItems)
        {
            return await _context.Assets
        .OrderBy(a => a.DateAdded)
       .Skip((pageNumber - 1) * numOfItems)
       .Take(numOfItems)
       .ToListAsync();
        }

        public async Task<IEnumerable<Asset>> SearchAssetsByTagsAsync(
     Tag[] tags,
     bool matchAll = false)
        {
            var tagIds = tags.Select(t => t.Id).ToList();


            IQueryable<Asset> query = _context.Assets.Include(a => a.Tags);


            if (matchAll)
            {
                query = query.Where(a => tagIds.All(id => a.Tags.Any(t => t.Id == id)));
            }
            else
            {
                query = query.Where(a => a.Tags.Any(t => tagIds.Contains(t.Id)));
            }

            return await query.ToListAsync();
        }
    }
}