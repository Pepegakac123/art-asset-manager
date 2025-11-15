using ArtAssetManager.Api.Data.Helpers;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
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
            return await _context.Assets
            .Include(a => a.Tags)
            .Include(a => a.Children)
            .FirstOrDefaultAsync(a => a.Id == id);
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
        public async Task UpdateAssetTagsAsync(int assetId, IEnumerable<Tag> tags)
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
        public async Task<PagedResult<Asset>> GetPagedAssetsAsync(
            AssetQueryParameters queryParams
        )
        {
            IQueryable<Asset> query = _context.Assets;

            query = query.ApplyFilteringAndSorting(queryParams);

            var totalItems = await query.CountAsync();
            return new PagedResult<Asset>
            {
                Items = await query.Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
        .Take(queryParams.PageSize).ToListAsync(),
                TotalItems = totalItems
            };
        }
        public async Task<IEnumerable<Asset>> GetAssetVersionAsync(int id)
        {
            List<Asset> assets = new List<Asset>();
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == id);
            if (asset == null) throw new KeyNotFoundException($"Asset {id} nie istnieje");
            var rootId = asset.ParentAssetId ?? asset.Id;
            var versions = await _context.Assets
        .Where(a => a.Id == rootId || a.ParentAssetId == rootId)
        .OrderByDescending(a => a.LastModified)
        .ToListAsync();

            return versions;

        }
        public async Task LinkAssetToParentAsync(int childId, int parentId)
        {
            var parentAsset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == parentId);
            if (parentAsset == null)
            {
                throw new KeyNotFoundException($"Asset 'Rodzic'  {parentId} nie istnieje");
            }
            if (parentAsset.ParentAssetId != null)
            {
                throw new Exception($"{parentAsset.FileName} jest już dzieckiem");
            }
            var childAsset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == childId);
            if (childAsset == null)
            {
                throw new KeyNotFoundException($"Asset 'dziecko' {childId} nie istnieje");
            }
            if (childAsset.ParentAssetId != null)
            {
                throw new Exception("Asset jest już połączony, rozłącz go najpierw");
            }
            childAsset.ParentAssetId = parentId;
            await _context.SaveChangesAsync();

        }
        public async Task SetAssetRatingAsync(int id, int rating)
        {
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == id);
            if (asset == null) throw new KeyNotFoundException($"Asset {id} nie istnieje");
            asset.Rating = rating;
            await _context.SaveChangesAsync();
        }
    };

}