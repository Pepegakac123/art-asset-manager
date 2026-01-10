using System.Net;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Errors;
using ArtAssetManager.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtAssetManager.Api.Controllers
{
    // Kontroler dostarczający statystyki dla dashboardu i paska bocznego
    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly IAssetRepository _assetRepository;
        public StatsController(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        // Główne statystyki biblioteki (ilość plików, zajęte miejsce itp.)
        [HttpGet("library")]
        public async Task<ActionResult<LibraryStatsDto>> GetStats(CancellationToken cancellationToken)
        {
            var stats = await _assetRepository.GetStatsAsync(cancellationToken);
            return Ok(stats);
        }
        
        // Statystyki pomocnicze dla menu bocznego
        [HttpGet("sidebar")]
        public async Task<ActionResult<SidebarStatsDto>> GetSidebarStats(CancellationToken cancellationToken)
        {
            var stats = await _assetRepository.GetSidebarStatsAsync(cancellationToken);
            return Ok(stats);
        }
    }
}