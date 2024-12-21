using LargeFilesManager.BL.Interfaces.Files;
using Microsoft.AspNetCore.Mvc;

namespace LargeFilesManager.Server.Controllers
{
    [ApiController]
    [Route("api/file/sorting")]
    public class FileSortingController : ControllerBase
    {
        private readonly IFileSortingService _fileSortingService;

        public FileSortingController(IFileSortingService fileSortingService)
        {
            _fileSortingService = fileSortingService;
        }

        [HttpGet("file-exists")]
        public IActionResult FileExists()
        {
            var result = _fileSortingService.FileExists();

            return Ok(result);
        }

        [HttpPost("sort")]
        public async Task<IActionResult> SortFile()
        {
            var result = await _fileSortingService.SortAsync();

            return Ok(result);
        }

        [HttpGet("file-status")]
        public async Task<IActionResult> GetSortStatus([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }

            var sortStatus = await _fileSortingService.GetFileSortAsync(fileName);
            return Ok(sortStatus);
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File Name is required.");
            }

            var fileStream = await _fileSortingService.DownloadAsync(fileName);

            var contentType = "application/octet-stream";
            var contentDisposition = $"attachment; filename={fileName}";

            Response.Headers.Add("Content-Disposition", contentDisposition);

            return File(fileStream, contentType);
        }

        [HttpDelete("delete")]
        public IActionResult DeleteSortedFiles()
        {
            _fileSortingService.DeleteFiles();

            return Ok();
        }
    }
}
