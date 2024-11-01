using PersonalSite.Application.DTOs.File;
using PersonalSite.WebUI.Client.Interfaces;
using System.Net.Http;

namespace PersonalSite.WebUI.Client.Services;

public class FileSystemService : IFileSystemService
{
    private readonly IHttpService _httpService;

    public FileSystemService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<FileSystemEntry?> GetFileHierarchyAsync()
    {
        return await _httpService.Get<FileSystemEntry>("file/hierarchy");
    }
    public async Task DeleteFile(string fileName)
    {
        await _httpService.Delete<Task>($"file/{fileName}");
    }
}
