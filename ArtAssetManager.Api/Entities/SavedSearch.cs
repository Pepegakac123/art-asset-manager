namespace ArtAssetManager.Api.Entities
{
    // Encja "Smart Collection" - zapisane kryteria wyszukiwania
    public class SavedSearch
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        // Przechowuje stan filtrów w formacie JSON (np. { "tags": ["nature"], "minRating": 4 })
        // Pozwala odtworzyć widok galerii z konkretnymi ustawieniami
        public string FilterJson { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }

        private SavedSearch() { }

        public static SavedSearch Create(string name, string filterJson)
        {
            var newSavedSearch = new SavedSearch
            {
                Name = name,
                FilterJson = filterJson,
                DateAdded = DateTime.UtcNow
            };
            return newSavedSearch;
        }
    }
}
