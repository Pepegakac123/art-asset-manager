using System.Net;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ArtAssetManager.Api.Controllers
{
    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        [HttpPost("validate-path")]
        public IActionResult ValidatePath([FromBody] ValidatePathRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Path))
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Path cannot be empty.", request.Path));
            }

            try
            {

                if (!Directory.Exists(request.Path))
                {
                    Ok(new { isValid = false, message = "directory does not exist" });
                }

                Directory.GetFiles(request.Path, "*", SearchOption.TopDirectoryOnly);

                // Jak przeszło, to znaczy że jest OK
                return Ok(new { isValid = true, message = "OK" });
            }
            catch (UnauthorizedAccessException)
            {
                return Ok(new { isValid = false, message = "unauthorized to access this path" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "There was an error.", request.Path));
            }
        }
        [HttpPost("open-in-explorer")]
        public IActionResult OpenInExplorer([FromBody] ValidatePathRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Path) || !Directory.Exists(request.Path))
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Path cannot be empty.", request.Path));
            }
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", request.Path);
                return Ok(new { message = "Explorer opened" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Could not open explorer", error = ex.Message });
            }
        }
    }
}