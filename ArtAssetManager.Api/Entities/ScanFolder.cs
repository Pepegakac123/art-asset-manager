namespace ArtAssetManager.Api.Entities
{
    public class ScanFolder
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}