namespace ArtAssetManager.Api.DTOs
{
    public class AssetDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;

        public string ThumbnailUrl { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
    }
}