using System.Net;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Errors;
using ArtAssetManager.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtAssetManager.Api.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly IAssetRepository _assetRepository;
        public StatsController(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        [HttpGet("library")]
        public async Task<ActionResult<LibraryStatsDto>> GetStats(CancellationToken cancellationToken)
        {
            var stats = await _assetRepository.GetStatsAsync(cancellationToken);
            return Ok(stats);
        }
    }
}