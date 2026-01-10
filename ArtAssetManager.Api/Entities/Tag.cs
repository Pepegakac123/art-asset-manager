namespace ArtAssetManager.Api.Entities
{
    // Prosta encja Tagu służąca do kategoryzacji assetów
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        
        // Relacja wiele-do-wielu z Assetami.
        // Dzięki obecności kolekcji ICollection<Asset> tutaj oraz ICollection<Tag> w Asset.cs,
        // Entity Framework Core domyślnie obsłuży tabelę łączącą (Join Table).
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
