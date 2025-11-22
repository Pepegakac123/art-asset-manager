using ArtAssetManager.Api.Enums;
using ArtAssetManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArtAssetManager.Api.Controllers
{
    [ApiController]
    [Route("api/scanner")]
    public class ScannerController : ControllerBase
    {
        private readonly IScannerTrigger _trigger;
        public ScannerController(IScannerTrigger trigger)
        {
            _trigger = trigger;
        }
        [HttpPost("start")]
        public async Task<ActionResult> TriggerScan()
        {

            await _trigger.TriggerScanAsync(ScanMode.Manual);
            return NoContent();
        }
        [HttpGet("status")]
        public IActionResult GetScanStatus()
        {
            var response = new { isScanning = _trigger.IsScanning };
            return Ok(response);
        }
    }

}