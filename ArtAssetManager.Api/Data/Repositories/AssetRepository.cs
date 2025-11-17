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
        public async Task<Asset?> GetAssetByFileHashAsync(string fileHash)
        {
            return await _context.Assets
          .FirstOrDefaultAsync(x => x.FileHash == fileHash && x.FileHash != null);
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
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                asset.Tags.Clear();
                foreach (var tag in tags)
                {
                    asset.Tags.Add(tag);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }



        }
        public async Task BulkUpdateAssetTagsAsync(List<int> assetIds, IEnumerable<Tag> tags)
        {

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var assets = await _context.Assets
                .Include(a => a.Tags)
                .Where(a => assetIds.Contains(a.Id))
                .ToListAsync();
                foreach (var asset in assets)
                {
                    asset.Tags.Clear();
                    foreach (var tag in tags)
                    {
                        asset.Tags.Add(tag);
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                throw;
            }
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

        public async Task ToggleAssetFavoriteAsync(int assetId)
        {
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == assetId);
            if (asset == null) throw new KeyNotFoundException($"Asset {assetId} nie istnieje");
            asset.IsFavorite = !(asset.IsFavorite ?? false);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<Asset>> GetFavoritesAssetsAsync(
           AssetQueryParameters queryParams
       )
        {
            IQueryable<Asset> query = _context.Assets.Where(a => a.IsFavorite == true);

            query = query.ApplyFilteringAndSorting(queryParams);

            var totalItems = await query.CountAsync();
            return new PagedResult<Asset>
            {
                Items = await query.Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
        .Take(queryParams.PageSize).ToListAsync(),
                TotalItems = totalItems
            };
        }


        private async Task<Asset> GetAssetIgnoreFilters(int assetId)
        {

            var markedAsset = await _context.Assets
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.Id == assetId);

            if (markedAsset == null)
            {
                throw new KeyNotFoundException($"Asset {assetId} nie istnieje");
            }
            return markedAsset;
        }

        public async Task SoftDeleteAssetAsync(int assetId)
        {
            var markedAsset = await GetAssetIgnoreFilters(assetId);
            markedAsset.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
        public async Task BulkSoftDeleteAssetsAsync(List<int> assetIds)
        {
            if (assetIds == null || assetIds.Count == 0)
            {
                throw new ArgumentException("Lista ID nie może być pusta.");
            }
            await _context.Assets
        .IgnoreQueryFilters()
        .Where(a => assetIds.Contains(a.Id))
        .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDeleted, true));
        }
        public async Task RestoreAssetAsync(int assetId)
        {
            var markedAsset = await GetAssetIgnoreFilters(assetId);
            markedAsset.IsDeleted = false;
            await _context.SaveChangesAsync();
        }
        public async Task BulkRestoreAssetsAsync(List<int> assetIds)
        {
            if (assetIds == null || assetIds.Count == 0)
            {
                throw new ArgumentException("Lista ID nie może być pusta.");
            }
            await _context.Assets
         .IgnoreQueryFilters()
         .Where(a => assetIds.Contains(a.Id))
         .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDeleted, false));
        }
        public async Task<PagedResult<Asset>> GetDeletedAssetsAsync(AssetQueryParameters queryParams)
        {
            IQueryable<Asset> query = _context.Assets.IgnoreQueryFilters().Where(a => a.IsDeleted == true);

            query = query.ApplyFilteringAndSorting(queryParams);

            var totalItems = await query.CountAsync();
            return new PagedResult<Asset>
            {
                Items = await query.Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
        .Take(queryParams.PageSize).ToListAsync(),
                TotalItems = totalItems
            };
        }
        public async Task PermanentDeleteAssetAsync(int assetId)
        {
            var markedAsset = await GetAssetIgnoreFilters(assetId);
            _context.Assets.Remove(markedAsset);
            await _context.SaveChangesAsync();
        }
        public async Task BulkPermanentDeleteAssetsAsync(List<int> assetIds)
        {
            if (assetIds == null || assetIds.Count == 0)
            {
                throw new ArgumentException("Lista ID nie może być pusta.");
            }
            await _context.Assets
.IgnoreQueryFilters()
.Where(a => assetIds.Contains(a.Id))
.ExecuteDeleteAsync();
        }
    };

}