using LargeFilesManager.BL.Interfaces.Files;
using LargeFilesManager.BL.Models;
using Microsoft.AspNetCore.Mvc;

namespace LargeFilesManager.Server.Controllers
{
    [ApiController]
    [Route("api/file/generation")]
    public class FileGenerationController : ControllerBase
    {
        private readonly IFileGenerationService _fileGenerationService;

        public FileGenerationController(IFileGenerationService fileGenerationService)
        {
            _fileGenerationService = fileGenerationService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateFile([FromBody] GenerateFileRequest request)
        {
            if (request.FileSize <= 0)
            {
                return BadRequest("File size must be greater than 0.");
            }

            var result = await _fileGenerationService.GenerateAsync(request.FileSize);

            return Ok(result);
        }

        [HttpGet("file-status")]
        public async Task<IActionResult> GetFileStatus([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }

            var fileStatus = await _fileGenerationService.GetFileStatusAsync(fileName);
            return Ok(fileStatus);
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File Name is required.");
            }

            var fileStream = await _fileGenerationService.DownloadAsync(fileName);

            var contentType = "application/octet-stream";
            var contentDisposition = $"attachment; filename={fileName}";

            Response.Headers.Add("Content-Disposition", contentDisposition);

            return File(fileStream, contentType);
        }

        [HttpDelete("delete")]
        public IActionResult DeleteGeneratedFile()
        {
            _fileGenerationService.DeleteFiles();

            return Ok();
        }
    }
}
