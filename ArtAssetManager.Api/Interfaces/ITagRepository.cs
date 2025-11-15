using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Shared;

namespace ArtAssetManager.Api.Interfaces
{
    public interface ITagRepository
    {
        Task<Result<IEnumerable<Tag>>> GetOrCreateTagsAsync(IEnumerable<string> tagNames);
        Task<IEnumerable<Tag>> GetAllTagsAsync();
    }
}