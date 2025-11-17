namespace ArtAssetManager.Api.Services.Helpers
{
    public class AssetMetadata
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string DominantColor { get; set; } = string.Empty;
        public int BitDepth { get; set; }
        public bool HasAlphaChannel { get; set; }

        public AssetMetadata(int width, int height, string dominantColor, int bitDepth, bool hasAlphaChannel)
        {
            Width = width;
            Height = height;
            DominantColor = dominantColor;
            BitDepth = bitDepth;
            HasAlphaChannel = hasAlphaChannel;
        }
    }
}