using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArtAssetManager.Api.Controllers
{
    [ApiController]
    [Route("api/assets")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetRepository _assetRepo;
        private readonly IMapper _mapper;

        public AssetsController(IAssetRepository assetRepo, IMapper mapper)
        {
            _assetRepo = assetRepo;
            _mapper = mapper;
        }

        private const int DefaultPage = 1;
        private const int DefaultPageSize = 20;
        private const int MaxPageSize = 60;

        //TODO: Pagination Metadata 
        [HttpGet] // GET /api/assets
        public async Task<ActionResult<IEnumerable<AssetDto>>> GetAssets(
    [FromQuery] int page = DefaultPage,
    [FromQuery] int pageSize = DefaultPageSize
        )
        {
            if (page <= 0) page = DefaultPage;
            if (pageSize <= 0) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;
            var assets = await _assetRepo.GetPagedAssetsAsync(page, pageSize);

            var assetsDto = _mapper.Map<IEnumerable<AssetDto>>(assets);

            return Ok(assetsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssetDetailsDto>> GetAssetById(int id)
        {
            var asset = await _assetRepo.GetAssetByIdAsync(id);
            if (asset == null)
            {
                return NotFound();
            }
            var assetDto = _mapper.Map<AssetDetailsDto>(asset);
            return Ok(assetDto);
        }

    }
}