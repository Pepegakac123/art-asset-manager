namespace ArtAssetManager.Api.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();

        private Tag() { }

        public static Tag Create(string name)
        {
            var newTag = new Tag
            {
                Name = name,
                DateCreated = DateTime.UtcNow
            };
            return newTag;
        }
    }
}