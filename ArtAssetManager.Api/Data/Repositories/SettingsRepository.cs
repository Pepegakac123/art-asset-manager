using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtAssetManager.Api.Data.Repositories
{
    // Repozytorium zarządzające ustawieniami aplikacji (foldery, rozszerzenia plików)
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
            // Sprawdzenie czy folder był kiedyś dodany ale został usunięty (soft delete)
            var oldFolder = await _context.ScanFolders.IgnoreQueryFilters().FirstOrDefaultAsync(f => f.Path == folder.Path, cancellationToken);
            if (oldFolder != null)
            {
                // Przywrócenie starego folderu zamiast tworzenia nowego
                oldFolder.IsDeleted = false;
                await _context.SaveChangesAsync(cancellationToken);
                return oldFolder;
            }
            _context.ScanFolders.Add(folder);
            await _context.SaveChangesAsync(cancellationToken);
            return folder;


        }
        
        // Usunięcie folderu z listy skanowania
        public async Task DeleteScanFolderAsync(int id, CancellationToken cancellationToken)
        {

            var folderToDelete = await _context.ScanFolders.FindAsync(new object[] { id }, cancellationToken);

            if (folderToDelete == null)
                throw new KeyNotFoundException($"Folder with ID {id} not found");

            // Logika: Jeśli usuwamy podfolder, a jego rodzic też jest skanowany,
            // to przenosimy assety do rodzica zamiast je "gubić".
            var parentFolder = await _context.ScanFolders
                .Where(f => f.Id != id && !f.IsDeleted)
                .Where(f => folderToDelete.Path.StartsWith(f.Path))
                .OrderByDescending(f => f.Path.Length)
                .FirstOrDefaultAsync(cancellationToken);

            if (parentFolder != null)
            {
                await _context.Assets
                    .Where(a => a.ScanFolderId == id)
                    .ExecuteUpdateAsync(s => s.SetProperty(a => a.ScanFolderId, parentFolder.Id), cancellationToken);
            }
            folderToDelete.IsDeleted = true;
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
        
        // Generyczne pobieranie ustawień z tabeli SystemSettings (klucz-wartość)
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
        
        // Zapis listy dozwolonych rozszerzeń z walidacją bezpieczeństwa
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

            // Blokada dodawania plików wykonywalnych
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