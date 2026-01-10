namespace ArtAssetManager.Api.Entities
{
    // Reprezentuje folder na dysku monitorowany przez aplikację
    public class ScanFolder
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty; // Pełna ścieżka do katalogu
        public DateTime DateAdded { get; set; }
        public bool IsActive { get; set; } // Czy skaner ma aktualnie sprawdzać ten folder
        
        // Soft delete - folder oznaczony jako usunięty przestaje być skanowany,
        // ale historia może zostać zachowana (zależnie od logiki czyszczenia)
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;

        // Relacja jeden-do-wielu: Folder zawiera wiele assetów
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();

        private ScanFolder() { }

        public static ScanFolder Create(string path)
        {
            var newScanFolder = new ScanFolder
            {
                Path = path,
                DateAdded = DateTime.UtcNow,
                IsActive = true,
            };
            return newScanFolder;
        }
    }
}
