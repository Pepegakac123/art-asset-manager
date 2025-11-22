namespace ArtAssetManager.Api.DTOs
{
    public class LibraryStatsDto
    {
        public int TotalAssets { get; set; }
        public long TotalSize { get; set; }
        public DateTime? LastScan { get; set; }
    }
}