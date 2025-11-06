using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtAssetManager.Api.Data.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly AssetDbContext _context;
        public SettingsRepository(AssetDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ScanFolder>> GetScanFoldersAsync()
        {
            return await _context.ScanFolders.ToListAsync();
        }
        public async Task<ScanFolder> AddScanFolderAsync(ScanFolder folder)
        {
            _context.ScanFolders.Add(folder);
            await _context.SaveChangesAsync();
            return folder;


        }
        public async Task DeleteScanFolderAsync(int id)
        {
            var folder = await _context.ScanFolders.FindAsync(id);
            if (folder == null) throw new KeyNotFoundException($"Folder {id} not found");
            _context.ScanFolders.Remove(folder);
            await _context.SaveChangesAsync();
        }
        public async Task<ScanFolder?> GetScanFolderByIdAsync(int id)
        {
            return await _context.ScanFolders.FindAsync(id);
        }


    }
}