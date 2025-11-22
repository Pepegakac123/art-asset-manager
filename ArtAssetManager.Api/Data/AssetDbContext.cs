using Microsoft.EntityFrameworkCore;
using ArtAssetManager.Api.Entities;
namespace ArtAssetManager.Api.Data
{
    public class AssetDbContext : DbContext
    {
        public AssetDbContext(DbContextOptions<AssetDbContext> options) : base(options)
        {

        }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ScanFolder> ScanFolders { get; set; }
        public DbSet<MaterialSet> MaterialSets { get; set; }
        public DbSet<SavedSearch> SavedSearches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

            modelBuilder.Entity<ScanFolder>()
                .HasIndex(s => s.Path)
                .IsUnique();
            modelBuilder.Entity<ScanFolder>().HasQueryFilter(a => a.IsDeleted == false);

            modelBuilder.Entity<SavedSearch>().HasIndex(s => s.Name).IsUnique();

            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.FilePath)
                .IsUnique();
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Parent)
                .WithMany(a => a.Children)
                .HasForeignKey(a => a.ParentAssetId)
                .OnDelete(DeleteBehavior.SetNull);

            // INDEKSY DLA WYDAJNOŚCI FILTROWANIA I SORTOWANIA
            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.ParentAssetId);
            modelBuilder.Entity<Asset>().HasIndex(a => a.FileType);
            modelBuilder.Entity<Asset>().HasIndex(a => a.Rating);
            modelBuilder.Entity<Asset>().HasIndex(a => a.FileSize);
            modelBuilder.Entity<Asset>().HasIndex(a => a.ImageWidth);
            modelBuilder.Entity<Asset>().HasIndex(a => a.ImageHeight);
            modelBuilder.Entity<Asset>().HasIndex(a => a.DominantColor);
            modelBuilder.Entity<Asset>().HasIndex(a => a.HasAlphaChannel);
            modelBuilder.Entity<Asset>().HasIndex(a => a.FileHash);
            modelBuilder.Entity<Asset>().HasIndex(a => a.DateAdded);
            modelBuilder.Entity<Asset>().HasIndex(a => a.LastModified);
            modelBuilder.Entity<Asset>().HasQueryFilter(a => a.IsDeleted == false);

            modelBuilder.Entity<MaterialSet>()
            .HasIndex(s => s.Name)
            .IsUnique();
            modelBuilder.Entity<MaterialSet>()
          .HasOne(m => m.CoverAsset)
          .WithMany()
          .HasForeignKey(m => m.CoverAssetId)
          .OnDelete(DeleteBehavior.SetNull);




        }
    }
}