namespace ArtAssetManager.Api.DTOs
{
    public class CreateMaterialSetRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? CoverAssetId { get; set; }
        public string? CustomCoverUrl { get; set; }
        public string? CustomColor { get; set; }
        public List<int>? InitialAssetIds { get; set; }
    }
}
