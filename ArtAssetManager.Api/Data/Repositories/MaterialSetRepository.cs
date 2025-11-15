using ArtAssetManager.Api.Interfaces;

namespace ArtAssetManager.Api.Data.Repositories
{
    public class MaterialSetRepository : IMaterialSetRepository
    {
        private readonly AssetDbContext _context;
        public MaterialSetRepository(AssetDbContext context)
        {
            _context = context;
        }
    }
}