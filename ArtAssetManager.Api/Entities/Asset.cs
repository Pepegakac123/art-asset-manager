namespace ArtAssetManager.Api.Entities
{
    // Główna encja reprezentująca pojedynczy plik (grafikę, model 3D) w systemie
    public class Asset
    {
        public int Id { get; set; }
        
        // Relacja do fizycznego folderu na dysku, z którego pochodzi plik
        public int? ScanFolderId { get; set; }
        public ScanFolder? ScanFolder { get; set; }
        
        // Relacja "Rodzic-Dziecko" (np. plik źródłowy PSD -> wyeksportowany JPG)
        // Pozwala grupować wersje tego samego assetu
        public int? ParentAssetId { get; set; }
        public Asset? Parent { get; set; }
        
        public string FileName { get; set; } = string.Empty;
        // Właściwość wyliczana (nie zapisywana w bazie), pomocnicza
        public string FileExtension => Path.GetExtension(FileName).ToLower();
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty; // np. "image", "texture", "model"
        public long FileSize { get; set; } = 0;
        public string ThumbnailPath { get; set; } = string.Empty; // Ścieżka do wygenerowanej miniatury
        
        // Metadane użytkownika
        public int Rating { get; set; } // Ocena 0-5
        public string? Description { get; set; }
        public bool? IsFavorite { get; set; }
        
        // Metadane techniczne pliku
        public int? ImageWidth { get; set; }
        public int? ImageHeight { get; set; }
        public string? DominantColor { get; set; } // Dominujący kolor (Hex)
        public int? BitDepth { get; set; }
        public bool? HasAlphaChannel { get; set; }
        
        // Daty i audyt
        public DateTime DateAdded { get; set; }
        public DateTime LastScanned { get; set; }
        public DateTime LastModified { get; set; } // Data modyfikacji pliku na dysku
        public string? FileHash { get; set; } // Hash do wykrywania duplikatów
        
        // Mechanizm Soft Delete (plik nie znika z bazy, tylko jest oznaczony jako usunięty)
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Relacja wiele-do-wielu z Tagami.
        // EF Core automatycznie tworzy tabelę łączącą (AssetTags), ponieważ
        // obie encje (Asset i Tag) mają kolekcje wskazujące na siebie nawzajem.
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        
        // Kolekcja dzieci (wersji) tego assetu
        public ICollection<Asset> Children { get; set; } = new List<Asset>();
        
        // Relacja wiele-do-wielu z Zestawami Materiałów
        public ICollection<MaterialSet> MaterialSets { get; set; } = new List<MaterialSet>();

        private Asset() { } // Konstruktor prywatny wymagany przez EF Core

        public static Asset Create(
            int scanFolderId,
            string filePath,
            long fileSize,
            string fileType,
            string thumbnailPath,
            DateTime lastModified,
            string? FileHash = null,
            int? FileWidth = null,
            int? FileHeight = null,
            string? DominantColor = null,
            int? BitDepth = null,
            bool? HasAlphaChannel = null
        )
        {
            var newAsset = new Asset
            {
                ScanFolderId = scanFolderId,
                FilePath = filePath,
                FileName = Path.GetFileName(filePath),
                FileType = fileType,
                FileSize = fileSize,
                FileHash = FileHash,
                ImageWidth = FileWidth,
                ImageHeight = FileHeight,
                DominantColor = DominantColor,
                BitDepth = BitDepth,
                HasAlphaChannel = HasAlphaChannel,
                ThumbnailPath = thumbnailPath,
                DateAdded = DateTime.UtcNow,
                LastScanned = DateTime.UtcNow,
                LastModified = lastModified,
                IsDeleted = false,
                DeletedAt = null,
                Rating = 0,
                IsFavorite = false

            };
            return newAsset;
        }


    }
}
