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
            if (string.IsNullOrWhiteSpace(body?.FolderPath))
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Ścieżka do folderu nie może być pusta", HttpContext.Request.Path));
            }
            string normalizedPath;
            try
            {
                normalizedPath = Path.GetFullPath(body.FolderPath);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Nieprawidłowy znak w ścieżce folderu", HttpContext.Request.Path));
            }
            catch (PathTooLongException ex)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Długość Ścieżki jest za długa", HttpContext.Request.Path));
            }
            catch (Exception)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Ścieżka jest niepoprawna", HttpContext.Request.Path));
            }
            if (!Directory.Exists(normalizedPath))
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Podany folder nie istnieje", HttpContext.Request.Path));
            }
            if (normalizedPath.Length <= 3)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Nie można skanować głownego dysku", HttpContext.Request.Path));
            }


            ScanFolder newScanFolder = new ScanFolder
            {
                Path = normalizedPath,
                DateAdded = DateTime.UtcNow,
                IsActive = true,
            };
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