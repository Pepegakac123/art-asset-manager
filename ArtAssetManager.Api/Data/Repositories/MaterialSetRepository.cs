using ArtAssetManager.Api.Data.Helpers;
using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtAssetManager.Api.Data.Repositories
{
    public class MaterialSetRepository : IMaterialSetRepository
    {
        private readonly AssetDbContext _context;
        public MaterialSetRepository(AssetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaterialSet>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.MaterialSets.ToListAsync(cancellationToken);
        }
        public async Task<MaterialSet> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var set = await _context.MaterialSets.Include(ms => ms.Assets).FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new KeyNotFoundException($"Nie znaleziono setu o ID:{id} ");
            return set;
        }
        public async Task<MaterialSet> AddAsync(MaterialSet materialSet, CancellationToken cancellationToken)
        {
            _context.MaterialSets.Add(materialSet);
            await _context.SaveChangesAsync(cancellationToken);
            return materialSet;
        }
        public async Task<MaterialSet> UpdateAsync(int id, MaterialSet updateData, CancellationToken cancellationToken)
        {
            var existingSet = await _context.MaterialSets.FindAsync(id, cancellationToken);
            if (existingSet == null)
            {
                throw new KeyNotFoundException($"Nie znaleziono setu o ID: {id}");
            }
            existingSet.Name = updateData.Name;
            existingSet.Description = updateData.Description;
            existingSet.CoverAssetId = updateData.CoverAssetId;
            existingSet.CustomCoverUrl = updateData.CustomCoverUrl;
            existingSet.LastModified = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return existingSet;
        }
        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var material = await _context.MaterialSets.FindAsync(id);
            if (material == null) throw new KeyNotFoundException($"Nie znaleziono setu o ID:{id} ");
            _context.MaterialSets.Remove(material);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task AddAssetToSetAsync(int assetId, int materialSetId, CancellationToken cancellationToken)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset == null) throw new KeyNotFoundException($"Nie znaleziono assetu o ID:{assetId} ");
            var materialSet = await _context.MaterialSets
            .Include(ms => ms.Assets)
            .FirstOrDefaultAsync(ms => ms.Id == materialSetId, cancellationToken) ?? throw new KeyNotFoundException($"Nie znaleziono setu o ID:{materialSetId} ");
            if (materialSet.Assets.Any(a => a.Id == asset.Id))
            {
                throw new InvalidOperationException($"Asset o Nazwie {asset.FileName} jest już w zestawie.");
            }
            if (materialSet.CoverAssetId == null && materialSet.CustomCoverUrl == null)
            {
                materialSet.CoverAssetId = asset.Id;
            }
            materialSet.Assets.Add(asset);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task RemoveAssetFromSetAsync(int assetId, int materialSetId, CancellationToken cancellationToken)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset == null) throw new KeyNotFoundException($"Nie znaleziono assetu o ID:{assetId} ");
            var materialSet = await _context.MaterialSets
            .Include(ms => ms.Assets)
            .FirstOrDefaultAsync(ms => ms.Id == materialSetId, cancellationToken) ?? throw new KeyNotFoundException($"Nie znaleziono setu o ID:{materialSetId} ");
            if (!materialSet.Assets.Any(a => a.Id == asset.Id))
            {
                throw new InvalidOperationException($"Asset o Nazwie {asset.FileName} nie istnieje już w zestawie.");
            }
            if (materialSet.CoverAssetId == asset.Id)
            {
                materialSet.CoverAssetId = null;
            }
            materialSet.Assets.Remove(asset);
            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task<PagedResult<Asset>> GetAssetsForSetAsync(int setId, AssetQueryParameters queryParams, CancellationToken cancellationToken)
        {
            if (!await _context.MaterialSets.AnyAsync(ms => ms.Id == setId, cancellationToken))
            {
                return new PagedResult<Asset> { Items = new List<Asset>(), TotalItems = 0 };
            }

            IQueryable<Asset> query = _context.Assets
                .Where(a => a.MaterialSets.Any(ms => ms.Id == setId));


            query = query.ApplyFilteringAndSorting(queryParams);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Asset>
            {
                Items = items,
                TotalItems = totalItems
            };
        }
    }
}