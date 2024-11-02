using Microsoft.JSInterop;
using PersonalSite.Application.DTOs.File;
using PersonalSite.WebUI.Client.Interfaces;
using System.Net.Http;

namespace PersonalSite.WebUI.Client.Services;

public class FileSystemService : IFileSystemService
{
    private readonly IHttpService _httpService;
    private readonly IJSRuntime JS;

    public FileSystemService(IHttpService httpService, IJSRuntime jS)
    {
        _httpService = httpService;
        JS = jS;
    }

    public async Task<FileSystemEntry?> GetFileHierarchyAsync()
    {
        return await _httpService.Get<FileSystemEntry>("file/hierarchy");
    }
    public async Task DeleteFile(string fileName)
    {
        await _httpService.Delete<int>($"file/{fileName}");
    }

    public async Task MoveFile(MoveFileRequest moveFileRequest)
    {
        await _httpService.Post<string>($"file/move", moveFileRequest);
    }

    public async Task DownloadFile(string fileName)
    {
        var response = await _httpService.GetBasic($"file/{fileName}");
        if (response.IsSuccessStatusCode)
        {
            var fileStream = await response.Content.ReadAsStreamAsync();

            var sanitizedFileName = Path.GetFileName(fileName);

            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await JS.InvokeVoidAsync("downloadFileFromStream", sanitizedFileName, streamRef);
        }
        else
        {
            throw new Exception("Failed to download file.");
        }
        ;
    }
}
