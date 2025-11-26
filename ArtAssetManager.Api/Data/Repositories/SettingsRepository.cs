using System.Text.Json;
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
            var exists = await _context.ScanFolders.AnyAsync(f => f.Path == folder.Path, cancellationToken);
            if (exists)
            {
                throw new InvalidOperationException("Folder already exists in the scan list.");
            }
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

        List<string> DefaultAllowedExtensions = new() { ".jpg", ".jpeg", ".gif", ".png", ".webp", ".blend", ".fbx", ".obj", ".ztl", ".zpr", ".exr", ".hdr", ".tif", ".tiff", ".max", ".ma", ".mb", ".zbr", ".spp", ".sbs", ".sbsar", ".hip", ".hipnc", ".hiplc", ".psd", ".psb", ".ai", ".eps", ".uasset", ".umap", ".unity", ".prefab", ".mat", ".asset" };
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

            if (extensions == null || extensions.Count == 0)
            {
                extensions = DefaultAllowedExtensions;
            }

            var dangerousExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".dll", ".bat", ".cmd", ".sh", ".vbs", ".msi", ".com", ".scr", ".js", ".ps1", ".bin"
    };

            var detectedThreat = extensions.FirstOrDefault(ext => dangerousExtensions.Contains(ext.Trim()));

            if (detectedThreat != null)
            {
                throw new ArgumentException($"Security Alert: Adding executable files ({detectedThreat}) is forbidden.");
            }
            var valueToSave = string.Join(";", extensions);
            await SetValueAsync("Scanner_AllowedExtensions", valueToSave, ct);
        }



    }
}