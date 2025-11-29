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
             [FromQuery] AssetQueryParameters queryParams, CancellationToken cancellationToken
         )
        {
            if (queryParams.PageNumber <= 0) queryParams.PageNumber = AssetQueryParameters.DefaultPage;
            if (queryParams.PageSize <= 0) queryParams.PageSize = AssetQueryParameters.DefaultPageSize;
            if (queryParams.PageSize > AssetQueryParameters.MaxPageSize) queryParams.PageSize = AssetQueryParameters.MaxPageSize;

            var pagedResult = await _assetRepo.GetPagedAssetsAsync(queryParams, cancellationToken);

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
        public async Task<ActionResult<AssetDetailsDto>> GetAssetById([FromRoute] int id, CancellationToken cancellationToken) //
        {

            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }

            var asset = await _assetRepo.GetAssetByIdAsync(id, cancellationToken);
            if (asset == null)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Asset o ID {id} nie został znaleziony.", HttpContext.Request.Path));
            }
            var assetDto = _mapper.Map<AssetDetailsDto>(asset);
            return Ok(assetDto);
        }

        [HttpGet("{id}/versions")]
        public async Task<ActionResult<IEnumerable<AssetDto>>> GetAssetVersions([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi byćwiększe od 0.", HttpContext.Request.Path));
            }
            try
            {
                var versions = await _assetRepo.GetAssetVersionAsync(id, cancellationToken);

                var versionsDto = _mapper.Map<IEnumerable<AssetDto>>(versions);
                return Ok(versionsDto);
            }
            catch (KeyNotFoundException)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Asset o ID {id} nie został znaleziony.", HttpContext.Request.Path));
            }

        }

        [HttpPost("{childId}/link-to/{parentId}")]
        public async Task<ActionResult> LinkAssetToParentAsync(
            [FromRoute] int childId,
            [FromRoute] int parentId, CancellationToken cancellationToken
        )
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
                await _assetRepo.LinkAssetToParentAsync(childId, parentId, cancellationToken);
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
            [FromBody] UpdateTagsRequest body, CancellationToken cancellationToken
        )
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                var tagsResult = await _tagRepo.GetOrCreateTagsAsync(body.TagsNames, cancellationToken);
                if (!tagsResult.IsSuccess)
                {
                    return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, tagsResult?.Error ?? "Nieprawidłowe tagi.", HttpContext.Request.Path));
                }

                await _assetRepo.UpdateAssetTagsAsync(id, tagsResult.Value!, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }

        }
        [HttpPost("bulk/tags")]
        public async Task<ActionResult> BulkUpdateAssetTagsAsync(
           [FromBody] BulkUpdateAssetTagsRequest body, CancellationToken cancellationToken
       )
        {
            try
            {
                var tagsResult = await _tagRepo.GetOrCreateTagsAsync(body.TagNames, cancellationToken);
                if (!tagsResult.IsSuccess)
                {
                    return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, tagsResult.Error ?? "Nieprawidłowe tagi.", HttpContext.Request.Path));
                }
                if (tagsResult.Value == null) return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Brak tagów.", HttpContext.Request.Path));
                await _assetRepo.BulkUpdateAssetTagsAsync(body.AssetIds, tagsResult.Value, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<AssetDto>> PatchAsset(int id, [FromBody] PatchAssetRequest request, CancellationToken cancellationToken)
        {
            var updatedAsset = await _assetRepo.UpdateAssetMetadataAsync(id, request, cancellationToken);

            if (updatedAsset == null)
                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, "Brak Assetu", HttpContext.Request.Path));

            return Ok(_mapper.Map<AssetDto>(updatedAsset));
        }

        [HttpPatch("{id}/rating")]
        public async Task<ActionResult> SetAssetRatingAsync(
            [FromRoute] int id,
            [FromBody] int rating, CancellationToken cancellationToken
        )
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
                await _assetRepo.SetAssetRatingAsync(id, rating, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }

        }
        [HttpPatch("{id}/toggle-favorite")]
        public async Task<ActionResult> ToggleAssetFavoriteAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi byćwiększe od 0.", HttpContext.Request.Path));
            }
            try
            {
                await _assetRepo.ToggleAssetFavoriteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }
        [HttpGet("favorites")]
        public async Task<ActionResult<PagedResponse<AssetDto>>> GetFavoriteAssets([FromQuery] AssetQueryParameters queryParams, CancellationToken cancellationToken)
        {
            if (queryParams.PageNumber <= 0) queryParams.PageNumber = AssetQueryParameters.DefaultPage;
            if (queryParams.PageSize <= 0) queryParams.PageSize = AssetQueryParameters.DefaultPageSize;
            if (queryParams.PageSize > AssetQueryParameters.MaxPageSize) queryParams.PageSize = AssetQueryParameters.MaxPageSize;
            var pagedResult = await _assetRepo.GetFavoritesAssetsAsync(queryParams, cancellationToken);

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
        [HttpGet("uncategorized")]
        public async Task<ActionResult<PagedResponse<AssetDto>>> GetUncategorizedAssets([FromQuery] AssetQueryParameters queryParams, CancellationToken cancellationToken)
        {
            if (queryParams.PageNumber <= 0) queryParams.PageNumber = AssetQueryParameters.DefaultPage;
            if (queryParams.PageSize <= 0) queryParams.PageSize = AssetQueryParameters.DefaultPageSize;
            if (queryParams.PageSize > AssetQueryParameters.MaxPageSize) queryParams.PageSize = AssetQueryParameters.MaxPageSize;
            var pagedResult = await _assetRepo.GetUncategorizedAssetsAsync(queryParams, cancellationToken);

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

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftlyDeleteAsset([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                await _assetRepo.SoftDeleteAssetAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }
        [HttpGet("deleted")]
        public async Task<ActionResult<PagedResponse<AssetDto>>> GetDeletedAssets([FromQuery] AssetQueryParameters queryParams, CancellationToken cancellationToken)
        {
            if (queryParams.PageNumber <= 0) queryParams.PageNumber = AssetQueryParameters.DefaultPage;
            if (queryParams.PageSize <= 0) queryParams.PageSize = AssetQueryParameters.DefaultPageSize;
            if (queryParams.PageSize > AssetQueryParameters.MaxPageSize) queryParams.PageSize = AssetQueryParameters.MaxPageSize;

            var pagedResult = await _assetRepo.GetDeletedAssetsAsync(queryParams, cancellationToken);

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
        [HttpPost("{id}/restore")]
        public async Task<ActionResult> RestoreAsset([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi byćwiększe od 0.", HttpContext.Request.Path));
            }
            try
            {
                await _assetRepo.RestoreAssetAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }
        [HttpDelete("{id}/permanent")]
        public async Task<ActionResult> PermanentlyDeleteAsset([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi byćwiększe od 0.", HttpContext.Request.Path));
            }
            try
            {
                await _assetRepo.PermanentDeleteAssetAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }
        // --- BULK OPERACJE DLA KOSZA ---

        [HttpPost("bulk/delete")]
        public async Task<ActionResult> BulkSoftlyDeleteAssets([FromBody] List<int> assetIds, CancellationToken cancellationToken)
        {
            if (assetIds == null || assetIds.Count == 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Lista ID nie może być pusta.", HttpContext.Request.Path));
            }
            try
            {
                await _assetRepo.BulkSoftDeleteAssetsAsync(assetIds, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, ex.Message, HttpContext.Request.Path));
            }
        }

        [HttpPost("bulk/restore")]
        public async Task<ActionResult> BulkRestoreAssets([FromBody] List<int> assetIds, CancellationToken cancellationToken)
        {
            if (assetIds == null || assetIds.Count == 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Lista ID nie może być pusta.", HttpContext.Request.Path));
            }
            try
            {
                await _assetRepo.BulkRestoreAssetsAsync(assetIds, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, ex.Message, HttpContext.Request.Path));
            }
        }

        [HttpPost("bulk/permanent-delete")]
        public async Task<ActionResult> BulkPermanentlyDeleteAssets([FromBody] List<int> assetIds, CancellationToken cancellationToken)
        {
            if (assetIds == null || assetIds.Count == 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Lista ID nie może być pusta.", HttpContext.Request.Path));
            }
            try
            {
                await _assetRepo.BulkPermanentDeleteAssetsAsync(assetIds, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, ex.Message, HttpContext.Request.Path));
            }
        }

        [HttpGet("colors")]
        public async Task<ActionResult<List<string>>> GetColorsList(CancellationToken cancellationToken)
        {
            var colorList = await _assetRepo.GetColorsListAsync(cancellationToken);
            return Ok(colorList);
        }
    }

}
