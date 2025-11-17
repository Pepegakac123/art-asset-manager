using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
using ArtAssetManager.Api.Shared;
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
        public async Task<Result<IEnumerable<Tag>>> GetOrCreateTagsAsync(IEnumerable<string> tagNames, CancellationToken cancellationToken)
        {
            var normalizedTagNames = tagNames.Select(n => n.Trim().ToLower()).Distinct().ToList();
            var existingTags = await _context.Tags
            .Where(t => normalizedTagNames.Contains(t.Name))
            .ToListAsync(cancellationToken);

            var existingNames = existingTags.Select(t => t.Name).ToHashSet();
            var missingNames = normalizedTagNames.Where(n => !existingNames.Contains(n));

            var newTags = missingNames.Select(n => Tag.Create(n)).ToList();
            if (newTags.Any())
            {
                _context.Tags.AddRange(newTags);
                await _context.SaveChangesAsync(cancellationToken);
            }
            return Result<IEnumerable<Tag>>.Success(existingTags.Concat(newTags));
        }
        public async Task<IEnumerable<Tag>> GetAllTagsAsync(CancellationToken cancellationToken)
        {
            return await _context.Tags.ToListAsync(cancellationToken);
        }
    }
}