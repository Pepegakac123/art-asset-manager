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
        public long? FileSizeMin { get; set; }
        public long? FileSizeMax { get; set; }
        public int? RatingMin { get; set; }
        public int? RatingMax { get; set; }
        public string? FileHash { get; set; }
        public List<string>? DominantColors { get; set; } = null;
        public int? MinWidth { get; set; }
        public int? MaxWidth { get; set; }
        public int? MinHeight { get; set; }
        public int? MaxHeight { get; set; }
        public bool? HasAlphaChannel { get; set; }
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 60;
    }
}