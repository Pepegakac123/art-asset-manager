namespace ArtAssetManager.Api.Data.Helpers
{
    public class AssetQueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool MatchAll { get; set; } = false;
        public bool SortDesc { get; set; } = false;
        public string? FileName { get; set; } = null;
        public List<string>? FileType { get; set; } = null;
        public List<string>? Tags { get; set; } = null;
        public DateTime? DateFrom { get; set; } = null;
        public DateTime? DateTo { get; set; } = null;
        public string? SortBy { get; set; } = null;

        public const int DefaultPage = 1;
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 60;
    }
}