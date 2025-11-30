namespace ArtAssetManager.Api.DTOs
{
    public class AssetDetailsDto
    {

        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileHash { get; set; } = string.Empty;
        public string ThumbnailPath { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Description { get; set; }
        public bool? IsFavorite { get; set; }
        public int? ImageWidth { get; set; }
        public int? ImageHeight { get; set; }
        public string DominantColor { get; set; } = string.Empty;
        public int? BitDepth { get; set; }
        public bool? HasAlphaChannel { get; set; }

        public List<TagDto> Tags { get; set; } = new List<TagDto>();
        public List<ChildDto> Children { get; set; } = new List<ChildDto>();
        public List<SimpleMaterialSetDto> MaterialSets { get; set; } = new();
    }

    public class SimpleMaterialSetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? CustomColor { get; set; }
    }
}
