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
    // Kontroler do zarządzania "Material Sets" - grupami assetów tworzących jeden materiał (np. tekstury PBR)
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
        public async Task<ActionResult<MaterialSetDto>> GetMaterialSet([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                var materialSet = await _materialSetRepository.GetByIdAsync(id, cancellationToken);
                return Ok(materialSet);
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }

        // Tworzy nową kolekcję materiałów
        [HttpPost]
        public async Task<ActionResult<MaterialSetDto>> AddMaterialSet([FromBody] CreateMaterialSetRequest request, CancellationToken cancellationToken)
        {
            var exists = await _materialSetRepository.ExistsByNameAsync(request.Name, cancellationToken);

            if (exists)
            {
                return Conflict(new { message = $"Collection with name '{request.Name}' already exists." });
            }
            var materialSet = MaterialSet.Create(request.Name, request.Description, null, request.CustomCoverUrl, request.CustomColor);

            await _materialSetRepository.AddAsync(materialSet, cancellationToken);
            var dto = _mapper.Map<MaterialSetDto>(materialSet);

            return CreatedAtAction(nameof(GetMaterialSet), new { id = dto.Id }, dto);
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
                var materialCount = await _materialSetRepository.CountByMaterialSetIdAsync(id, cancellationToken);
                materialSetDto.TotalAssets = materialCount;
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

        // Pobiera wszystkie assety przypisane do danego zestawu materiałów
        [HttpGet("{setId}/assets")]
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

        // Dodaje asset do kolekcji
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
        
        // Usuwa asset z kolekcji (nie usuwa samego pliku)
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