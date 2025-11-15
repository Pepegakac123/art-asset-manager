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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

            modelBuilder.Entity<ScanFolder>()
                .HasIndex(s => s.Path)
                .IsUnique();


            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.FilePath)
                .IsUnique();
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Parent)
                .WithMany(a => a.Children)
                .HasForeignKey(a => a.ParentAssetId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.ParentAssetId);
            modelBuilder.Entity<Asset>().HasQueryFilter(a => a.IsDeleted == false);

        }
    }
}