using ArtAssetManager.Api.Enums;
using ArtAssetManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArtAssetManager.Api.Controllers
{
    // Kontroler do ręcznego sterowania procesem skanowania dysku
    [ApiController]
    [Route("api/scanner")]
    public class ScannerController : ControllerBase
    {
        private readonly IScannerTrigger _trigger;
        public ScannerController(IScannerTrigger trigger)
        {
            _trigger = trigger;
        }

        // Ręczne wymuszenie skanowania folderów (np. przycisk "Odśwież" w UI)
        [HttpPost("start")]
        public async Task<ActionResult> TriggerScan()
        {

            await _trigger.TriggerScanAsync(ScanMode.Manual);
            return NoContent();
        }

        // Sprawdza, czy skaner aktualnie pracuje
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { isScanning = _trigger.IsScanning });
        }
    }

}
