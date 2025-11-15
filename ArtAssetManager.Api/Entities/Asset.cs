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
        public DateTime DateAdded { get; set; }
        public DateTime LastScanned { get; set; }
        public DateTime LastModified { get; set; }
        public string? FileHash { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<Asset> Children { get; set; } = new List<Asset>();

        private Asset() { }

        public static Asset Create(
            int scanFolderId,
            string filePath,
            long fileSize,
            string fileType,
            string thumbnailPath,
            DateTime lastModified,
            string? FileHash
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
                ThumbnailPath = thumbnailPath,
                DateAdded = DateTime.UtcNow,
                LastScanned = DateTime.UtcNow,
                LastModified = lastModified,
                IsDeleted = false,
                DeletedAt = null
            };
            return newAsset;
        }


    }
}