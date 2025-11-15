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


            if (!string.IsNullOrEmpty(queryParams.FileName))
            {
                var keyword = $"%{queryParams.FileName}%";
                query = query.Where(a => EF.Functions.Like(a.FileName, keyword));
            }
            if (queryParams.Tags?.Count > 0)
            {

                if (queryParams.MatchAll)
                {
                    foreach (var tagName in queryParams.Tags)
                    {
                        query = query.Where(a => a.Tags.Any(t => tagName == t.Name));
                    }

                }
                else
                {
                    query = query.Where(a => a.Tags.Any(t => queryParams.Tags.Contains(t.Name)));
                }
            }
            if (queryParams.FileType?.Count > 0)
            {
                query = query.Where(a => queryParams.FileType.Any(type => a.FileType == type));
            }
            if (queryParams.DateFrom != null)
            {
                query = query.Where(a => a.DateAdded >= queryParams.DateFrom);
            }
            if (queryParams.DateTo != null)
            {
                query = query.Where(a => a.DateAdded <= queryParams.DateTo);
            }
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {

                var normalizedSortBy = queryParams.SortBy.ToLowerInvariant();

                switch (normalizedSortBy)
                {
                    case "filename":
                        query = queryParams.SortDesc
                            ? query.OrderByDescending(a => a.FileName)
                            : query.OrderBy(a => a.FileName);
                        break;

                    case "filesize":
                        query = queryParams.SortDesc
                            ? query.OrderByDescending(a => a.FileSize)
                            : query.OrderBy(a => a.FileSize);
                        break;

                    case "lastmodified":
                        query = queryParams.SortDesc
                            ? query.OrderByDescending(a => a.LastModified)
                            : query.OrderBy(a => a.LastModified);
                        break;

                    case "dateadded":
                    default:
                        query = queryParams.SortDesc
                            ? query.OrderByDescending(a => a.DateAdded)
                            : query.OrderBy(a => a.DateAdded);
                        break;
                }
            }

            else
            {
                query = query.OrderByDescending(a => a.DateAdded);
            }
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
    };

}