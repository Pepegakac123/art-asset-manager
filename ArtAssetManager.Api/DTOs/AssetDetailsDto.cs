namespace ArtAssetManager.Api.DTOs
{
    public class AssetDetailsDto
    {

        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}