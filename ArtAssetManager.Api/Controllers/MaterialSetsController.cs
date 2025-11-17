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
using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.Controllers
{
    [ApiController]
    [Route("api/materialsets")]
    public class MaterialSetsController : ControllerBase
    {
        private readonly IMaterialSetRepository _materialSetRepository;
        private readonly IMapper _mapper;

        public MaterialSetsController(IMaterialSetRepository materialSetRepository, IMapper mapper)
        {
            _materialSetRepository = materialSetRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialSetDto>>> GetMaterialSets(CancellationToken cancellationToken)
        {
            var materialSets = await _materialSetRepository.GetAllAsync(cancellationToken);
            var materialSetsDto = _mapper.Map<IEnumerable<MaterialSetDto>>(materialSets);
            return Ok(materialSetsDto);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialSetDetailsDto>> GetMaterialSet([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                var materialSet = await _materialSetRepository.GetByIdAsync(id, cancellationToken);
                var materialSetDto = _mapper.Map<MaterialSetDetailsDto>(materialSet);
                return Ok(materialSetDto);
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }
        [HttpPost]
        public async Task<ActionResult<MaterialSetDto>> AddMaterialSet([FromBody] CreateMaterialSetRequest body, CancellationToken cancellationToken)
        {
            var newMaterialSet = _mapper.Map<MaterialSet>(body);
            var createdMaterialSet = await _materialSetRepository.AddAsync(newMaterialSet, cancellationToken);
            var materialSetDto = _mapper.Map<MaterialSetDto>(createdMaterialSet);
            return CreatedAtAction(
        nameof(GetMaterialSet),
        new { id = materialSetDto.Id },
        materialSetDto
    );
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<MaterialSetDto>> UpdateMaterialSet([FromRoute] int id, [FromBody] UpdateMaterialSet body, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                var newMaterialSet = _mapper.Map<MaterialSet>(body);
                var updatedMaterialSet = await _materialSetRepository.UpdateAsync(id, newMaterialSet, cancellationToken);
                var materialSetDto = _mapper.Map<MaterialSetDto>(updatedMaterialSet);
                return Ok(materialSetDto);
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMaterialSet([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                await _materialSetRepository.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }

        [HttpGet("{id}/assets")]
        public async Task<ActionResult<PagedResponse<AssetDto>>> GetAssets(
                    [FromRoute] int setId, [FromQuery] AssetQueryParameters queryParams, CancellationToken cancellationToken
                )
        {
            if (setId <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            if (queryParams.PageNumber <= 0) queryParams.PageNumber = AssetQueryParameters.DefaultPage;
            if (queryParams.PageSize <= 0) queryParams.PageSize = AssetQueryParameters.DefaultPageSize;
            if (queryParams.PageSize > AssetQueryParameters.MaxPageSize) queryParams.PageSize = AssetQueryParameters.MaxPageSize;

            var pagedResult = await _materialSetRepository.GetAssetsForSetAsync(setId, queryParams, cancellationToken);

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

        [HttpPost("{setId}/assets/{assetId}")]
        public async Task<ActionResult> AddAssetToMaterialSet([FromRoute] int setId, [FromRoute] int assetId, CancellationToken cancellationToken)
        {
            if (setId <= 0 || assetId <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                await _materialSetRepository.AddAssetToSetAsync(assetId, setId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }
        [HttpDelete("{setId}/assets/{assetId}")]
        public async Task<ActionResult> RemoveAssetFromMaterialSet([FromRoute] int setId, [FromRoute] int assetId, CancellationToken cancellationToken)
        {
            if (setId <= 0 || assetId <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                await _materialSetRepository.RemoveAssetFromSetAsync(assetId, setId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }
    }
}