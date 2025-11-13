using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Entities;
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
            if (id <= 0) return BadRequest();
            var scanFolder = await _settingsRepo.GetScanFolderByIdAsync(id);
            if (scanFolder == null) return NotFound();
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
            if (!string.IsNullOrWhiteSpace(body.FolderPath) && Directory.Exists(body?.FolderPath))
            {

                ScanFolder newScanFolder = new ScanFolder
                {
                    Path = body.FolderPath,
                    DateAdded = DateTime.UtcNow,
                    IsActive = true,
                };
                var scanFolder = await _settingsRepo.AddScanFolderAsync(newScanFolder);
                var ScanFolderDto = _mapper.Map<ScanFolderDto>(scanFolder);
                return CreatedAtAction(nameof(GetScanFoldersById), new { id = ScanFolderDto.Id }, ScanFolderDto);

            }
            return BadRequest();

        }
        [HttpDelete("folders/{id}")]
        public async Task<ActionResult> DeleteScanFolder([FromRoute] int id)
        {
            if (id <= 0) return BadRequest();
            try
            {
                await _settingsRepo.DeleteScanFolderAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {

                return NotFound();
            }
        }

    }
}