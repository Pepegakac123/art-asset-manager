using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ArtAssetManager.Api.Controllers
{
    // Kontroler do interakcji z systemem operacyjnym (otwieranie plików, sprawdzanie ścieżek)
    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        [HttpGet("status")]
        public IActionResult GetStatus() => Ok(new { msg = "OK" });

        // Sprawdza, czy podana ścieżka istnieje i czy aplikacja ma do niej dostęp
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
                    return Ok(new { isValid = false, message = "directory does not exist" });
                }

                // Próba odczytu plików, żeby sprawdzić uprawnienia
                Directory.GetFiles(request.Path, "*", SearchOption.TopDirectoryOnly);

                // Jak przeszło, to znaczy że jest OK
                return Ok(new { isValid = true, message = "OK" });
            }
            catch (UnauthorizedAccessException)
            {
                return Ok(new { isValid = false, message = "unauthorized to access this path" });
            }
            catch (Exception)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "There was an error.", request.Path));
            }
        }

        // Otwiera plik w domyślnym programie systemowym
        [HttpPost("open-in-program")]
        public IActionResult OpenInProgram([FromBody] ValidatePathRequest request)
        {
            bool pathExists = System.IO.File.Exists(request.Path) || Directory.Exists(request.Path);
            if (string.IsNullOrWhiteSpace(request.Path) || !pathExists)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Path cannot be empty.", request.Path));
            }
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", request.Path);
                return Ok(new { message = "Program opened" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Could not open file in program", error = ex.Message });
            }
        }

        // Otwiera lokalizację pliku w eksploratorze plików (Windows Explorer, Finder, etc.)
        [HttpPost("open-in-explorer")]
        public IActionResult OpenInExplorer([FromBody] ValidatePathRequest request)
        {
            Console.WriteLine(request.Path);
            bool pathExists = System.IO.File.Exists(request.Path) || Directory.Exists(request.Path);
            Console.WriteLine("Path exists: " + pathExists);
            if (string.IsNullOrWhiteSpace(request.Path) || !pathExists)
            {
                return BadRequest(new ApiErrorResponse(HttpStatusCode.BadRequest, "Path cannot be empty.", request.Path));
            }
            try
            {
                // 2. Wykrywanie systemu i dobór odpowiedniej komendy systemowej
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Windows: /select zaznacza konkretny plik w oknie folderu
                    // Podmieniamy / na \ bo Windows preferuje backslashe w argumentach explorera
                    string winPath = request.Path.Replace("/", "\");
                    Process.Start("explorer.exe", $"/select,\"{winPath}\"");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // Linux: xdg-open otwiera folder. 
                    // Linux rzadko wspiera natywne "zaznaczanie" pliku, więc otwieramy folder nadrzędny.
                    string folderToOpen = request.Path;

                    if (System.IO.File.Exists(folderToOpen))
                    {
                        folderToOpen = Path.GetDirectoryName(folderToOpen);
                    }

                    // Używamy patternu z argumentami array, to bezpieczniejsze na Linuxie
                    Process.Start("xdg-open", folderToOpen);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // Mac: open -R odkrywa (reveal) plik w Finderze
                    Process.Start("open", $"-R \"{request.Path}\"");
                }
                else
                {
                    return BadRequest(new { message = "OS not supported for opening files." });
                }

                return Ok(new { message = "Explorer opened" });
            }
            catch (Exception ex)
            {
                // Logujemy dokładny błąd, żeby wiedzieć co poszło nie tak
                Console.WriteLine($"Error opening explorer: {ex.Message}");
                return StatusCode(500, new { message = "Could not open explorer", error = ex.Message });
            }
        }
    }
}
