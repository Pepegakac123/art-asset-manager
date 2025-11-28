namespace ArtAssetManager.Api.DTOs
{
    public class MaterialSetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TotalAssets { get; set; }
        public int? CoverAssetId { get; set; }
        public string? CustomCoverUrl { get; set; }
        public string? CustomColor { get; set; }
        public int? Count { get; set; }
    }
}
