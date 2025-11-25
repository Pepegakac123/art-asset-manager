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
        public async Task<IEnumerable<ScanFolder>> GetScanFoldersAsync(CancellationToken cancellationToken)
        {
            return await _context.ScanFolders.ToListAsync(cancellationToken);
        }
        public async Task<ScanFolder> AddScanFolderAsync(ScanFolder folder, CancellationToken cancellationToken)
        {
            _context.ScanFolders.Add(folder);
            await _context.SaveChangesAsync(cancellationToken);
            return folder;


        }
        public async Task DeleteScanFolderAsync(int id, CancellationToken cancellationToken)
        {
            var folder = await _context.ScanFolders.FindAsync(id);
            if (folder == null) throw new KeyNotFoundException($"Folder {id} not found");
            folder.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<ScanFolder?> GetScanFolderByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.ScanFolders.FindAsync(id);
        }

        public async Task<ScanFolder> UpdateScanFolderStatusAsync(int id, bool isActive, CancellationToken cancellationToken)
        {
            var folder = await _context.ScanFolders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (folder == null) throw new KeyNotFoundException($"Folder {id} not found");
            folder.IsActive = isActive;
            await _context.SaveChangesAsync(cancellationToken);
            return folder;
        }
        public async Task<string?> GetValueAsync(string key, CancellationToken ct = default)
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key, ct);
            return setting?.Value;
        }
        public async Task SetValueAsync(string key, string value, CancellationToken ct = default)
        {
            var existing = await _context.SystemSettings.FindAsync(new object[] { key }, ct);

            if (existing != null)
            {
                existing.Value = value;
            }
            else
            {
                _context.SystemSettings.Add(new SystemSetting { Key = key, Value = value });
            }

            await _context.SaveChangesAsync(ct);
        }

        List<string> DefaultAllowedExtensions = new() { ".jpg", ".jpeg", ".gif", ".png", ".webp", ".blend", ".fbx", ".obj", ".ztl", ".zpr" };
        public async Task<List<string>> GetAllowedExtensionsAsync(CancellationToken ct = default)
        {
            var extensions = await GetValueAsync("Scanner_AllowedExtensions");
            if (string.IsNullOrEmpty(extensions))
            {
                await SetAllowedExtensionsAsync(DefaultAllowedExtensions, ct);
                return DefaultAllowedExtensions;
            }
            return extensions.Split(';').ToList();
        }
        public async Task SetAllowedExtensionsAsync(List<string> extensions, CancellationToken ct = default)
        {
            if (extensions.Count == 0) extensions = DefaultAllowedExtensions;
            await SetValueAsync("Scanner_AllowedExtensions", string.Join(";", extensions), ct);
        }



    }
}