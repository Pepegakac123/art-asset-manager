using System.Globalization;
using ArtAssetManager.Api.Data.Helpers;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using ArtAssetManager.Api.Errors;

namespace ArtAssetManager.Api.Controllers
{
    [ApiController]
    [Route("api/assets")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetRepository _assetRepo;
        private readonly ITagRepository _tagRepo;
        private readonly IMapper _mapper;

        public AssetsController(IAssetRepository assetRepo, ITagRepository tagRepo, IMapper mapper)
        {
            _assetRepo = assetRepo;
            _tagRepo = tagRepo;
            _mapper = mapper;
        }



        [HttpGet] // GET /api/assets
        public async Task<ActionResult<PagedResponse<AssetDto>>> GetAssets(
             [FromQuery] AssetQueryParameters queryParams
         )
        {
            if (queryParams.PageNumber <= 0) queryParams.PageNumber = AssetQueryParameters.DefaultPage;
            if (queryParams.PageSize <= 0) queryParams.PageSize = AssetQueryParameters.DefaultPageSize;
            if (queryParams.PageSize > AssetQueryParameters.MaxPageSize) queryParams.PageSize = AssetQueryParameters.MaxPageSize;

            var pagedResult = await _assetRepo.GetPagedAssetsAsync(queryParams);

            var assetsDto = _mapper.Map<IEnumerable<AssetDto>>(pagedResult.Items);

            var totalPages = (int)Math.Ceiling(pagedResult.TotalItems / (double)queryParams.PageSize);
            var hasNext = queryParams.PageNumber < totalPages;
            var hasPrevious = queryParams.PageNumber > 1;

            var response = new PagedResponse<AssetDto>
            {
                Items = assetsDto.ToList(),
                TotalItems = pagedResult.TotalItems,
                PageSize = queryParams.PageSize,
                CurrentPage = queryParams.PageNumber,
                TotalPages = totalPages,
                HasNextPage = hasNext,
                HasPreviousPage = hasPrevious
            };

            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetDetailsDto>> GetAssetById(int id)
        {

            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }

            var asset = await _assetRepo.GetAssetByIdAsync(id);
            if (asset == null)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Asset o ID {id} nie został znaleziony.", HttpContext.Request.Path));
            }
            var assetDto = _mapper.Map<AssetDetailsDto>(asset);
            return Ok(assetDto);
        }

        [HttpGet("{id}/versions")]
        public async Task<ActionResult<IEnumerable<AssetDto>>> GetAssetVersions(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi byćwiększe od 0.", HttpContext.Request.Path));
            }
            try
            {
                var versions = await _assetRepo.GetAssetVersionAsync(id);

                var versionsDto = _mapper.Map<IEnumerable<AssetDto>>(versions);
                return Ok(versionsDto);
            }
            catch (KeyNotFoundException)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Asset o ID {id} nie został znaleziony.", HttpContext.Request.Path));
            }

        }
        [HttpPost("{childId}/link-to/{parentId}")]
        public async Task<ActionResult> LinkAssetToParentAsync(int childId, int parentId)
        {
            if (childId <= 0 || parentId <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            if (childId == parentId)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Nie można powiązać ze sobą takich samych assetów", HttpContext.Request.Path));
            }
            try
            {
                await _assetRepo.LinkAssetToParentAsync(childId, parentId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
            catch (Exception ex)
            {

                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }

        [HttpPost("{id}/tags")]
        public async Task<ActionResult> UpdateAssetTagsAsync(
            [FromRoute] int id,
            [FromBody] UpdateTagsRequest body
        )
        {

            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }

            var asset = await _assetRepo.GetAssetByIdAsync(id);


            if (asset == null)
            {
                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Asset o ID {id} nie został znaleziony.", HttpContext.Request.Path));
            }

            var tagsResult = await _tagRepo.GetOrCreateTagsAsync(body.TagsNames);
            if (!tagsResult.IsSuccess)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, tagsResult?.Error ?? "Nieprawidłowe tagi.", HttpContext.Request.Path));
            }

            await _assetRepo.UpdateAssetTagsAsync(id, tagsResult.Value!);
            return NoContent();
        }
        [HttpPost("{id}/rating")]
        public async Task<ActionResult> SetAssetRatingAsync([FromRoute] int id, [FromBody] int rating)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            if (rating < 0 || rating > 5)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Ocena musi znajdować sie w przedziale od 0 do 5.", HttpContext.Request.Path));
            }
            try
            {
                await _assetRepo.SetAssetRatingAsync(id, rating);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }

        }

    }
}