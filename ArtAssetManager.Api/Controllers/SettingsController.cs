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
    [Route("api/settings")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepository _settingsRepo;

        private readonly IMapper _mapper;

        public SettingsController(ISettingsRepository settingsRepo, IMapper mapper)
        {
            _settingsRepo = settingsRepo;
            _mapper = mapper;
        }

        [HttpGet("folders/{id}")]
        public async Task<ActionResult<ScanFolderDto>> GetScanFoldersById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }

            var scanFolder = await _settingsRepo.GetScanFolderByIdAsync(id);

            if (scanFolder == null)
            {
                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Folder skanowania o ID {id} nie został znaleziony.", HttpContext.Request.Path));
            }

            var scanFolderDto = _mapper.Map<ScanFolderDto>(scanFolder);
            return Ok(scanFolderDto);
        }

        [HttpGet("folders")]
        public async Task<ActionResult<IEnumerable<ScanFolderDto>>> GetScanFoldersAsync()
        {
            var scanFolders = await _settingsRepo.GetScanFoldersAsync();
            var scanFoldersDto = _mapper.Map<IEnumerable<ScanFolderDto>>(scanFolders);
            return Ok(scanFoldersDto);
        }

        [HttpPost("folders")]
        public async Task<ActionResult<ScanFolderDto>> AddScanFolderAsync([FromBody] AddScanFolderRequest body)
        {

            var normalizedPath = Path.GetFullPath(body.FolderPath);

            if (!Directory.Exists(normalizedPath))
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Podany folder nie istnieje", HttpContext.Request.Path));
            }
            ScanFolder newScanFolder = ScanFolder.Create(normalizedPath);
            var scanFolder = await _settingsRepo.AddScanFolderAsync(newScanFolder);
            var ScanFolderDto = _mapper.Map<ScanFolderDto>(scanFolder);
            return CreatedAtAction(nameof(GetScanFoldersById), new { id = ScanFolderDto.Id }, ScanFolderDto);

        }
        [HttpDelete("folders/{id}")]
        public async Task<ActionResult> DeleteScanFolder([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }

            try
            {
                await _settingsRepo.DeleteScanFolderAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Folder skanowania o ID {id} nie został znaleziony.", HttpContext.Request.Path));
            }
        }
        [HttpPatch("folders/{id}/toggle")]
        public async Task<ActionResult<ScanFolderDto>> ToggleScanFolderActive([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "ID musi być większe od 0.", HttpContext.Request.Path));
            }
            try
            {
                var scanFolder = await _settingsRepo.ToggleScanFolderActiveAsync(id);
                var ScanFolderDto = _mapper.Map<ScanFolderDto>(scanFolder);
                return Ok(ScanFolderDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiErrorResponse(HttpStatusCode.NotFound, $"Folder skanowania o ID {id} nie został znaleziony.", HttpContext.Request.Path));
            }
        }


    }
}