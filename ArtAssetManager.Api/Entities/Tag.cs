namespace ArtAssetManager.Api.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}