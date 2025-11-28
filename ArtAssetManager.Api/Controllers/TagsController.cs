using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
namespace ArtAssetManager.Api.Controllers
{
    [ApiController]
    [Route("api/tags")]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _tagRepo;
        private readonly IMapper _mapper;

        public TagsController(ITagRepository tagRepo, IMapper mapper)
        {
            _tagRepo = tagRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags(CancellationToken cancellationToken)
        {
            var tags = await _tagRepo.GetAllTagsAsync(cancellationToken);

            var tagsDto = _mapper.Map<IEnumerable<TagDto>>(tags);

            return Ok(tagsDto);
        }
    }
}
