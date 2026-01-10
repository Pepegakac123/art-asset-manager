namespace ArtAssetManager.Api.Services.Helpers
{
    // Prosta klasa pomocnicza (DTO wewnątrz-serwisowe)
    // Służy do przekazywania wyników analizy obrazu (ImageSharp) do logiki tworzenia Assetu
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
