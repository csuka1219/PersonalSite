using PersonalSite.Application.DTOs.File;

namespace PersonalSite.WebUI.Client.Interfaces;

public interface IFileSystemService
{
    Task<FileSystemEntry?> GetFileHierarchyAsync();
    Task DownloadFile(string fileName);
    Task DeleteFile(string fileName);
    Task MoveFile(MoveFileRequest moveFileRequest);
    Task CreateFolder(CreateFolderRequest folderRequest);
}
