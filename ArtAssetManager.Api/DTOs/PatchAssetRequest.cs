namespace ArtAssetManager.Api.DTOs
{
    public class PatchAssetRequest
    {
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? Description { get; set; }
        public int? Rating { get; set; }
        public bool? IsFavorite { get; set; }
        public string? ThumbnailPath { get; set; }
    }
}
