namespace ArtAssetManager.Api.Entities
{
    // Encja reprezentująca zestaw materiałów (Kolekcję)
    // Służy do grupowania powiązanych plików, np. mapy tekstur dla jednego materiału PBR (Albedo, Normal, Roughness)
    public class MaterialSet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        // Opcjonalne wskazanie assetu, który służy jako okładka tej kolekcji
        public int? CoverAssetId { get; set; }
        public Asset? CoverAsset { get; set; }
        
        public string? CustomCoverUrl { get; set; } // Zewnętrzny URL/ścieżka dla niestandardowej okładki
        public string? CustomColor { get; set; } = string.Empty; // Kolor wyróżniający folder w UI
        public DateTime DateAdded { get; set; }
        public DateTime LastModified { get; set; }

        // Relacja wiele-do-wielu: Zestaw zawiera wiele assetów.
        // Zdefiniowana przez konwencję (kolekcja Assets tutaj i kolekcja MaterialSets w Asset.cs).
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();

        private MaterialSet() { }

        public static MaterialSet Create(string Name, string? Description, int? CoverAssetId, string? CustomCoverUrl, string? CustomColor)
        {
            var newMaterialSet = new MaterialSet
            {
                Name = Name,
                Description = Description,
                CoverAssetId = CoverAssetId,
                CustomCoverUrl = CustomCoverUrl,
                CustomColor = CustomColor,
                DateAdded = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };
            return newMaterialSet;
        }


    }
}