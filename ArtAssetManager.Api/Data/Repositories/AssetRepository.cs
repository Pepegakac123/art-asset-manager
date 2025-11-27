using System.Drawing;
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
        public async Task<Asset?> GetAssetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Assets
            .Include(a => a.Tags)
            .Include(a => a.Children)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }
        public async Task<Asset?> GetAssetByPathAsync(string path, CancellationToken cancellationToken)
        {
            return await _context.Assets.FirstOrDefaultAsync(x => x.FilePath == path, cancellationToken);
        }
        public async Task<Asset?> GetAssetByFileHashAsync(string fileHash, CancellationToken cancellationToken)
        {
            return await _context.Assets
          .FirstOrDefaultAsync(x => x.FileHash == fileHash && x.FileHash != null, cancellationToken);
        }
        public async Task<Asset> AddAssetAsync(Asset asset, CancellationToken cancellationToken)
        {
            _context.Add(asset);
            await _context.SaveChangesAsync(cancellationToken);
            return asset;
        }
        public async Task<Asset?> UpdateAssetMetadataAsync(int id, PatchAssetRequest request, CancellationToken cancellationToken)
        {
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (asset == null) return null;

            if (request.FileName != null) asset.FileName = request.FileName;
            if (request.FileType != null) asset.FileType = request.FileType;
            if (request.Rating.HasValue) asset.Rating = request.Rating.Value;
            if (request.IsFavorite.HasValue) asset.IsFavorite = request.IsFavorite.Value;
            if (request.Description != null) asset.Description = request.Description;
            await _context.SaveChangesAsync(cancellationToken);
            return asset;
        }
        public async Task UpdateAssetTagsAsync(int assetId, IEnumerable<Tag> tags, CancellationToken cancellationToken)
        {

            var asset = await _context.Assets
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == assetId, cancellationToken);
            if (asset == null) throw new KeyNotFoundException($"Asset {assetId} not found");
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                asset.Tags.Clear();
                foreach (var tag in tags)
                {
                    asset.Tags.Add(tag);
                }
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

        }
        public async Task BulkUpdateAssetTagsAsync(List<int> assetIds, IEnumerable<Tag> tags, CancellationToken cancellationToken)
        {

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
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
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {

                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        public async Task UpdateAssetAsync(Asset asset, CancellationToken ct = default)
        {
            _context.Assets.Update(asset); // To jest bezpieczne - jeśli obiekt jest już śledzony, EF tylko potwierdzi stan.
            await _context.SaveChangesAsync(ct);
        }
        public async Task<PagedResult<Asset>> GetPagedAssetsAsync(
            AssetQueryParameters queryParams, CancellationToken cancellationToken
        )
        {
            IQueryable<Asset> query = _context.Assets;

            query = query.ApplyFilteringAndSorting(queryParams);

            var totalItems = await query.CountAsync(cancellationToken);
            return new PagedResult<Asset>
            {
                Items = await query.Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
        .Take(queryParams.PageSize).ToListAsync(cancellationToken),
                TotalItems = totalItems
            };
        }
        public async Task<IEnumerable<Asset>> GetAssetVersionAsync(int id, CancellationToken cancellationToken)
        {
            var rootId = await _context.Assets.Where(a => a.Id == id).Select(a => a.ParentAssetId ?? (int?)a.Id).FirstOrDefaultAsync(cancellationToken);
            if (rootId == null)
            {
                throw new KeyNotFoundException($"Asset o ID {id} nie został znaleziony.");
            }
            var versions = await _context.Assets
        .Where(a => a.Id == rootId || a.ParentAssetId == rootId)
        .OrderByDescending(a => a.LastModified)
        .ToListAsync(cancellationToken);

            return versions;

        }
        public async Task LinkAssetToParentAsync(int childId, int parentId, CancellationToken cancellationToken)
        {
            var parentAsset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == parentId, cancellationToken);
            if (parentAsset == null)
            {
                throw new KeyNotFoundException($"Asset 'Rodzic'  {parentId} nie istnieje");
            }
            if (parentAsset.ParentAssetId != null)
            {
                throw new Exception($"{parentAsset.FileName} jest już dzieckiem");
            }
            var childAsset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == childId, cancellationToken);
            if (childAsset == null)
            {
                throw new KeyNotFoundException($"Asset 'dziecko' {childId} nie istnieje");
            }
            if (childAsset.ParentAssetId != null)
            {
                throw new Exception("Asset jest już połączony, rozłącz go najpierw");
            }
            childAsset.ParentAssetId = parentId;
            await _context.SaveChangesAsync(cancellationToken);

        }
        public async Task SetAssetRatingAsync(int id, int rating, CancellationToken cancellationToken)
        {
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (asset == null) throw new KeyNotFoundException($"Asset {id} nie istnieje");
            asset.Rating = rating;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ToggleAssetFavoriteAsync(int assetId, CancellationToken cancellationToken)
        {
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == assetId, cancellationToken);
            if (asset == null) throw new KeyNotFoundException($"Asset {assetId} nie istnieje");
            asset.IsFavorite = !(asset.IsFavorite ?? false);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PagedResult<Asset>> GetFavoritesAssetsAsync(
           AssetQueryParameters queryParams, CancellationToken cancellationToken
       )
        {
            IQueryable<Asset> query = _context.Assets.Where(a => a.IsFavorite == true);

            query = query.ApplyFilteringAndSorting(queryParams);

            var totalItems = await query.CountAsync(cancellationToken);
            return new PagedResult<Asset>
            {
                Items = await query.Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
        .Take(queryParams.PageSize).ToListAsync(cancellationToken),
                TotalItems = totalItems
            };
        }

        public async Task<PagedResult<Asset>> GetUncategorizedAssetsAsync(
           AssetQueryParameters queryParams, CancellationToken cancellationToken
       )
        {
            IQueryable<Asset> query = _context.Assets.IgnoreQueryFilters().Where(a => a.Tags.Count == 0);

            query = query.ApplyFilteringAndSorting(queryParams);

            var totalItems = await query.CountAsync(cancellationToken);
            return new PagedResult<Asset>
            {
                Items = await query.Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
        .Take(queryParams.PageSize).ToListAsync(cancellationToken),
                TotalItems = totalItems
            };
        }


        private async Task<Asset> GetAssetIgnoreFilters(int assetId, CancellationToken cancellationToken)
        {

            var markedAsset = await _context.Assets
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.Id == assetId, cancellationToken);

            if (markedAsset == null)
            {
                throw new KeyNotFoundException($"Asset {assetId} nie istnieje");
            }
            return markedAsset;
        }

        public async Task SoftDeleteAssetAsync(int assetId, CancellationToken cancellationToken)
        {
            var markedAsset = await GetAssetIgnoreFilters(assetId, cancellationToken);
            markedAsset.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
        public async Task BulkSoftDeleteAssetsAsync(List<int> assetIds, CancellationToken cancellationToken)
        {
            if (assetIds == null || assetIds.Count == 0)
            {
                throw new ArgumentException("Lista ID nie może być pusta.");
            }
            await _context.Assets
        .IgnoreQueryFilters()
        .Where(a => assetIds.Contains(a.Id))
        .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDeleted, true), cancellationToken);
        }
        public async Task RestoreAssetAsync(int assetId, CancellationToken cancellationToken)
        {
            var markedAsset = await GetAssetIgnoreFilters(assetId, cancellationToken);
            markedAsset.IsDeleted = false;
            await _context.SaveChangesAsync();
        }
        public async Task BulkRestoreAssetsAsync(List<int> assetIds, CancellationToken cancellationToken)
        {
            if (assetIds == null || assetIds.Count == 0)
            {
                throw new ArgumentException("Lista ID nie może być pusta.");
            }
            await _context.Assets
         .IgnoreQueryFilters()
         .Where(a => assetIds.Contains(a.Id))
         .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDeleted, false), cancellationToken);
        }
        public async Task<PagedResult<Asset>> GetDeletedAssetsAsync(AssetQueryParameters queryParams, CancellationToken cancellationToken)
        {
            IQueryable<Asset> query = _context.Assets.IgnoreQueryFilters().Where(a => a.IsDeleted == true);

            query = query.ApplyFilteringAndSorting(queryParams);

            var totalItems = await query.CountAsync(cancellationToken);
            return new PagedResult<Asset>
            {
                Items = await query.Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
        .Take(queryParams.PageSize).ToListAsync(cancellationToken),
                TotalItems = totalItems
            };
        }
        public async Task PermanentDeleteAssetAsync(int assetId, CancellationToken cancellationToken)
        {
            var markedAsset = await GetAssetIgnoreFilters(assetId, cancellationToken);
            _context.Assets.Remove(markedAsset);
            await _context.SaveChangesAsync();
        }
        public async Task BulkPermanentDeleteAssetsAsync(List<int> assetIds, CancellationToken cancellationToken)
        {
            if (assetIds == null || assetIds.Count == 0)
            {
                throw new ArgumentException("Lista ID nie może być pusta.");
            }
            await _context.Assets
.IgnoreQueryFilters()
.Where(a => assetIds.Contains(a.Id))
.ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<LibraryStatsDto> GetStatsAsync(CancellationToken cancellationToken)
        {
            var totalAssets = await _context.Assets.CountAsync(cancellationToken);

            if (totalAssets == 0)
            {
                return new LibraryStatsDto
                {
                    TotalAssets = 0,
                    TotalSize = 0,
                    LastScan = null
                };
            }
            var totalSize = await _context.Assets.SumAsync(a => a.FileSize, cancellationToken);

            var lastScan = await _context.Assets.MaxAsync(a => a.LastModified, cancellationToken);

            return new LibraryStatsDto
            {
                TotalAssets = totalAssets,
                TotalSize = totalSize,
                LastScan = lastScan
            };
        }
        public async Task<List<string>> GetColorsListAsync(CancellationToken cancellationToken)
        {
            var colorList = await _context.Assets.Select(a => a.DominantColor).Where(color => color != null).Select(c => c!.ToLower()).Distinct().ToListAsync(cancellationToken);
            return colorList!;
        }
    };





}
