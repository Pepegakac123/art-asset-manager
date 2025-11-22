using System.Net;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Errors;
using ArtAssetManager.Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ArtAssetManager.Api.Controllers
{
    [ApiController]
    [Route("api/saved-searches")]
    public class SavedSearchesController : ControllerBase
    {
        private readonly ISavedSearchRepository _savedSearchRepository;
        private readonly IMapper _mapper;

        public SavedSearchesController(ISavedSearchRepository savedSearchRepository, IMapper mapper)
        {
            _savedSearchRepository = savedSearchRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavedSearchDto>>> GetSavedSearches(CancellationToken cancellationToken)
        {
            var savedSearches = await _savedSearchRepository.GetAllAsync(cancellationToken);
            var savedSearchesDto = _mapper.Map<IEnumerable<SavedSearchDto>>(savedSearches);
            return Ok(savedSearchesDto);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<SavedSearchDto>> GetSavedSearch(int id, CancellationToken cancellationToken)
        {
            var savedSearch = await _savedSearchRepository.GetByIdAsync(id, cancellationToken);
            var savedSearchDto = _mapper.Map<SavedSearchDto>(savedSearch);
            return Ok(savedSearchDto);
        }
        [HttpPost]
        public async Task<ActionResult<SavedSearchDto>> AddSavedSearch([FromBody] CreateSavedSearchRequest body, CancellationToken cancellationToken)
        {
            var newSavedSearch = _mapper.Map<SavedSearch>(body);
            var savedSearch = await _savedSearchRepository.AddAsync(newSavedSearch, cancellationToken);
            var savedSearchDto = _mapper.Map<SavedSearchDto>(savedSearch);
            return CreatedAtAction(nameof(GetSavedSearch), new { id = savedSearchDto.Id }, savedSearchDto);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<SavedSearchDto>> UpdateSavedSearch(int id, [FromBody] UpdateSavedSearchRequest body, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                var updateData = _mapper.Map<SavedSearch>(body);
                var updatedEntity = await _savedSearchRepository.UpdateAsync(id, updateData, cancellationToken);
                var dto = _mapper.Map<SavedSearchDto>(updatedEntity);
                return Ok(dto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict(new ApiErrorResponse(HttpStatusCode.Conflict, "Kolekcja o takiej nazwie już istnieje.", HttpContext.Request.Path));
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSavedSearch(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                await _savedSearchRepository.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Błąd: {ex.Message}", HttpContext.Request.Path));
            }
        }
    }
}