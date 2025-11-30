namespace ArtAssetManager.Api.DTOs
{
    public class AssetDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public float FileSize { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public string FileExtension { get; set; } = string.Empty;
        public string ThumbnailPath { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
    }
}
