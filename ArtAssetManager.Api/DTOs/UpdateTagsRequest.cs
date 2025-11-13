namespace ArtAssetManager.Api.DTOs
{
    public class UpdateTagsRequest
    {
        public List<string> TagsNames { get; set; } = new List<string>();
    }
}