using ArtAssetManager.Api.Data.Helpers;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        // TODO (Phase 4): Refactor to return PagedResult<Asset> with metadata
        // - TotalItems, TotalPages, HasNext, HasPrevious
        public async Task<PagedResult<Asset>> GetPagedAssetsAsync(
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
    )
        {

            IQueryable<Asset> query = _context.Assets;


            if (!string.IsNullOrEmpty(fileName))
            {
                var keyword = $"%{fileName}%";
                query = query.Where(a => EF.Functions.Like(a.FileName, keyword));
            }
            if (tags?.Count > 0)
            {

                if (matchAll)
                {
                    query = query.Where(a => tags.All(tagName => a.Tags.Any(t => t.Name == tagName)));
                }
                else
                {
                    query = query.Where(a => a.Tags.Any(t => tags.Contains(t.Name)));
                }
            }
            if (fileType?.Count > 0)
            {
                query = query.Where(a => fileType.Any(type => a.FileType == type));
            }
            if (dateFrom != null)
            {
                query = query.Where(a => a.DateAdded >= dateFrom);
            }
            if (dateTo != null)
            {
                query = query.Where(a => a.DateAdded <= dateTo);
            }
            if (!string.IsNullOrEmpty(sortBy))
            {

                var normalizedSortBy = sortBy.ToLowerInvariant();

                switch (normalizedSortBy)
                {
                    case "filename":
                        query = sortDesc
                            ? query.OrderByDescending(a => a.FileName)
                            : query.OrderBy(a => a.FileName);
                        break;

                    case "filesize":
                        query = sortDesc
                            ? query.OrderByDescending(a => a.FileSize)
                            : query.OrderBy(a => a.FileSize);
                        break;

                    case "lastmodified":
                        query = sortDesc
                            ? query.OrderByDescending(a => a.LastModified)
                            : query.OrderBy(a => a.LastModified);
                        break;

                    case "dateadded":
                    default:
                        query = sortDesc
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
                Items = await query.Skip((pageNumber - 1) * numOfItems)
        .Take(numOfItems).ToListAsync(),
                TotalItems = totalItems
            };
        }
    }
}