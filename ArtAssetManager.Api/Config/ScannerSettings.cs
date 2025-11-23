namespace ArtAssetManager.Api.Config
{
    public class ScannerSettings
    {
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
        public string ThumbnailsFolder { get; set; } = "wwwroot/thumbnails";
        public string PlaceholderThumbnail { get; set; } = "/thumbnails/placeholder.png";
        public bool EnableHashing { get; set; } = false;
        public int MaxHashFileSizeMB { get; set; } = 10;
        public Dictionary<string, string> PlaceholderMappings { get; set; } = new();
    }
}