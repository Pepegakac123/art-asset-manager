namespace ArtAssetManager.Api.Entities
{
    public class ScanFolder
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;

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