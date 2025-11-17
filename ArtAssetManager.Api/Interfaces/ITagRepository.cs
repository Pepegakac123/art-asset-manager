using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Shared;

namespace ArtAssetManager.Api.Interfaces
{
    public interface ITagRepository
    {
        Task<Result<IEnumerable<Tag>>> GetOrCreateTagsAsync(IEnumerable<string> tagNames, CancellationToken cancellationToken);
        Task<IEnumerable<Tag>> GetAllTagsAsync(CancellationToken cancellationToken);
    }
}