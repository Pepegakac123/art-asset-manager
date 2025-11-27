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
        public string? CustomColor { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }
        public DateTime LastModified { get; set; }

        public ICollection<Asset> Assets { get; set; } = new List<Asset>();

        private MaterialSet() { }

        public static MaterialSet Create(string Name, string? Description, int? CoverAssetId, string? CustomCoverUrl, string? CustomColor)
        {
            var newMaterialSet = new MaterialSet
            {
                Name = Name,
                Description = Description,
                CoverAssetId = CoverAssetId,
                CustomCoverUrl = CustomCoverUrl,
                CustomColor = CustomColor,
                DateAdded = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };
            return newMaterialSet;
        }


    }
}
