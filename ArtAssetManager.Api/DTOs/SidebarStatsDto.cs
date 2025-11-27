namespace ArtAssetManager.Api.DTOs;

public class SidebarStatsDto
{
    public int TotalAssets { get; set; }
    public int TotalFavorites { get; set; }
    public int TotalUncategorized { get; set; }
    public int TotalTrashed { get; set; }
}
