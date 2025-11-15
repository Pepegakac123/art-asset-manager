namespace ArtAssetManager.Api.Entities
{
    public class MaterialSet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? CoverAssetId { get; set; }
        public Asset? CoverAsset { get; set; }
        public string? CustomCoverUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastModified { get; set; }

        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}