namespace ArtAssetManager.Api.DTOs
{
    public class BulkUpdateAssetTagsRequest
    {
        public List<int> AssetIds { get; set; } = new List<int>();
        public List<string> TagNames { get; set; } = new List<string>();
    }
}