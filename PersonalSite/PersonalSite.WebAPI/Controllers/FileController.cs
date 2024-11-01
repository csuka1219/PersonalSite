using Microsoft.AspNetCore.Mvc;
using PersonalSite.Application.Common.Interfaces;

namespace PersonalSite.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public FileController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFiles(List<IFormFile> files)
    {
        if (files == null || !files.Any())
        {
            return BadRequest("No files provided.");
        }

        var fileData = files.Select(file => (file.OpenReadStream(), file.FileName));
        var savedFilePaths = await _fileStorageService.SaveFilesAsync(fileData);

        return Ok(savedFilePaths);
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        var fileStream = await _fileStorageService.GetFileAsync(fileName);

        if (fileStream == null)
            return NotFound();

        return File(fileStream, "application/octet-stream", fileName);
    }

    [HttpDelete("{fileName}")]
    public async Task<IActionResult> DeleteFile(string fileName)
    {
        fileName = fileName.Replace('%', '/');
        if (fileName.StartsWith("/"))
            fileName = fileName.Substring(1);

        var deleted = await _fileStorageService.DeleteFileAsync(fileName);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("hierarchy")]
    public async Task<IActionResult> ListFilesAndDirectories()
    {
        var hierarchy = await _fileStorageService.GetFileSystemHierarchyAsync();
        return Ok(hierarchy);
    }
}
