namespace ArtAssetManager.Api.Entities
{
    public class Asset
    {
        public int Id { get; set; }
        public int? ScanFolderId { get; set; }
        public ScanFolder? ScanFolder { get; set; }
        public int? ParentAssetId { get; set; }
        public Asset? Parent { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileExtension => Path.GetExtension(FileName).ToLower();
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; } = 0;
        public string ThumbnailPath { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Description { get; set; }
        public bool? IsFavorite { get; set; }
        public int? ImageWidth { get; set; }
        public int? ImageHeight { get; set; }

        public string? DominantColor { get; set; }
        public int? BitDepth { get; set; }
        public bool? HasAlphaChannel { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastScanned { get; set; }
        public DateTime LastModified { get; set; }
        public string? FileHash { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<Asset> Children { get; set; } = new List<Asset>();
        public ICollection<MaterialSet> MaterialSets { get; set; } = new List<MaterialSet>();

        private Asset() { }

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