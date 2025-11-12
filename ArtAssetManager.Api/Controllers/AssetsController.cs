using ArtAssetManager.Api.Data.Helpers;
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

        [HttpGet] // GET /api/assets
        public async Task<ActionResult<PagedResponse<AssetDto>>> GetAssets(
    [FromQuery] int pageNumber = DefaultPage,
    [FromQuery] int pageSize = DefaultPageSize,
    [FromQuery] bool matchAll = false,
    [FromQuery] bool sortDesc = false,
    [FromQuery] string? fileName = null,
    [FromQuery] List<string>? fileType = null,
    [FromQuery] List<string>? tags = null,
    [FromQuery] DateTime? dateFrom = null,
    [FromQuery] DateTime? dateTo = null,
    [FromQuery] string? sortBy = null
)
        {

            if (pageNumber <= 0) pageNumber = DefaultPage;
            if (pageSize <= 0) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;

            var pagedResult = await _assetRepo.GetPagedAssetsAsync(
                pageNumber,
                pageSize,
                fileName,
                fileType,
                tags,
                matchAll,
                sortBy,
                sortDesc,
                dateFrom,
                dateTo
            );
            var assetsDto = _mapper.Map<IEnumerable<AssetDto>>(pagedResult.Items);

            var totalPages = (int)Math.Ceiling(pagedResult.TotalItems / (double)pageSize);
            var hasNext = pageNumber < totalPages;
            var hasPrevious = pageNumber > 1;


            var response = new PagedResponse<AssetDto>
            {
                Items = assetsDto.ToList(),
                TotalItems = pagedResult.TotalItems,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                HasNextPage = hasNext,
                HasPreviousPage = hasPrevious
            };

            return Ok(response);
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