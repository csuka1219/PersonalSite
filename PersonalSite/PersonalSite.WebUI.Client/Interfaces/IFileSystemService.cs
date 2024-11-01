using PersonalSite.Application.DTOs.File;

namespace PersonalSite.WebUI.Client.Interfaces;

public interface IFileSystemService
{
    Task<FileSystemEntry?> GetFileHierarchyAsync();
    Task DeleteFile(string fileName);
}
