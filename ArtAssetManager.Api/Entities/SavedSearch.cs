namespace ArtAssetManager.Api.Entities
{
    public class SavedSearch
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
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