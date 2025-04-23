using Microsoft.AspNetCore.Mvc;
using PersonalSite.Application.Common.Interfaces;
using PersonalSite.Application.DTOs.File;
using System.Web;

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

    [HttpPost("move")]
    public async Task<IActionResult> MoveFile(MoveFileRequest request)
    {
        try
        {
            await _fileStorageService.MoveFileAsync(request);

            return Ok("File moved successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to move file: {ex.Message}");
        }
    }

    [HttpPost("CreateFolder")]
    public async Task<IActionResult> CreateFolder(CreateFolderRequest folderRequest)
    {
        try
        {
            await _fileStorageService.CreateFolder(folderRequest.FolderPath);
            return Ok("Folder created successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to create folder: {ex.Message}");
        }
    }


    [HttpGet("{fileName}")]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        fileName = HttpUtility.UrlDecode(fileName);
        fileName = fileName.TrimStart('/');
        var fileStream = await _fileStorageService.GetFileAsync(fileName);

        if (fileStream == null)
            return NotFound();

        return File(fileStream, "application/octet-stream", fileName);
    }

    [HttpDelete("{fileName}")]
    public async Task<IActionResult> DeleteFile(string fileName)
    {
        fileName = HttpUtility.UrlDecode(fileName);
        fileName = fileName.TrimStart('/');

        var deleted = await _fileStorageService.DeleteFileAsync(fileName);

        if (!deleted)
            return NotFound();

        return Ok("File deleted succesfully");
    }

    [HttpGet("hierarchy")]
    public async Task<IActionResult> ListFilesAndDirectories()
    {
        var hierarchy = await _fileStorageService.GetFileSystemHierarchyAsync();
        return Ok(hierarchy);
    }
}
