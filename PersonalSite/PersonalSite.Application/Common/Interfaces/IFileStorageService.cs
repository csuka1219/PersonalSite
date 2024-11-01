using PersonalSite.Application.DTOs.File;

namespace PersonalSite.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<IEnumerable<string>> SaveFilesAsync(IEnumerable<(Stream FileStream, string FileName)> files);
    Task<Stream?> GetFileAsync(string fileName);
    Task<bool> DeleteFileAsync(string fileName);
    Task<FileSystemEntry> GetFileSystemHierarchyAsync();
}
