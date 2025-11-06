using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtAssetManager.Api.Data.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AssetDbContext _context;

        public TagRepository(AssetDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames)
        {
            var existingTags = await _context.Tags
            .Where(t => tagNames.Contains(t.Name))
            .ToListAsync();

            var existingNames = existingTags.Select(t => t.Name).ToHashSet();
            var missingNames = tagNames.Where(n => !existingNames.Contains(n));

            var newTags = missingNames.Select(n => new Tag
            {
                Name = n,
                DateCreated = DateTime.UtcNow
            }).ToList();
            if (newTags.Any())
            {
                _context.Tags.AddRange(newTags);
                await _context.SaveChangesAsync();
            }
            return existingTags.Concat(newTags);

        }
        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }
    }
}