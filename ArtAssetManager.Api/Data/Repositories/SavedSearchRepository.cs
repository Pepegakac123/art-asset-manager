using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtAssetManager.Api.Data.Repositories
{
    public class SavedSearchRepository : ISavedSearchRepository
    {
        private readonly AssetDbContext _context;

        public SavedSearchRepository(AssetDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<SavedSearch>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.SavedSearches.ToListAsync(cancellationToken);
        }
        public async Task<SavedSearch> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var savedSearch = await _context.SavedSearches.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new KeyNotFoundException($"Nie znaleziono wyszukiwania o ID:{id} ");
            return savedSearch;
        }
        public async Task<SavedSearch> AddAsync(SavedSearch savedSearch, CancellationToken cancellationToken)
        {
            _context.SavedSearches.Add(savedSearch);
            await _context.SaveChangesAsync(cancellationToken);
            return savedSearch;
        }
        public async Task<SavedSearch> UpdateAsync(int id, SavedSearch updateData, CancellationToken cancellationToken)
        {
            var existingSearch = await _context.SavedSearches.FindAsync(id, cancellationToken);
            if (existingSearch == null)
            {
                throw new KeyNotFoundException($"Nie znaleziono wyszukiwania o ID: {id}");
            }
            existingSearch.Name = updateData.Name;
            existingSearch.FilterJson = updateData.FilterJson;
            await _context.SaveChangesAsync(cancellationToken);
            return existingSearch;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _context.SavedSearches.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
        }
    }
}