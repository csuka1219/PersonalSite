using Azure.Core;
using PersonalSite.Application.Common.Interfaces;
using PersonalSite.Application.DTOs.File;

namespace PersonalSite.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

    public FileStorageService()
    {
        Directory.CreateDirectory(_basePath);
    }

    public async Task<IEnumerable<string>> SaveFilesAsync(IEnumerable<(Stream FileStream, string FileName)> files)
    {
        var filePaths = new List<string>();

        foreach (var (fileStream, fileName) in files)
        {
            var filePath = Path.Combine(_basePath, fileName);

            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            filePaths.Add(filePath);
        }

        return filePaths;
    }

    public async Task<Stream?> GetFileAsync(string fileName)
    {
        var filePath = Path.Combine(_basePath, fileName);

        if (File.Exists(filePath))
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        return null;
    }

    public Task<bool> DeleteFileAsync(string fileName)
    {
        var filePath = Path.Combine(_basePath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<bool> MoveFileAsync(MoveFileRequest request)
    {
        var currentFilePath = Path.Combine(_basePath, request.CurrentFilePath.TrimStart('/'));
        var newLocationPath = Path.Combine(_basePath, request.NewLocation.TrimStart('/'));

        if (!System.IO.File.Exists(currentFilePath))
            return Task.FromResult(false);

        System.IO.File.Move(currentFilePath, newLocationPath);

        return Task.FromResult(true);
    }

    public Task<FileSystemEntry> GetFileSystemHierarchyAsync()
    {
        return Task.FromResult(GetDirectoryStructure(_basePath));
    }

    private FileSystemEntry GetDirectoryStructure(string path)
    {
        string rootPath = _basePath;
        var relativePath = Path.GetRelativePath(rootPath, path);

        var directoryEntry = new FileSystemEntry
        {
            Name = Path.GetFileName(path),
            IsDirectory = true,
            FullPath = relativePath,
            Children = new List<FileSystemEntry>()
        };

        // Add files in the current directory
        foreach (var file in Directory.GetFiles(path))
        {
            var relativeFilePath = Path.GetRelativePath(rootPath, file);

            directoryEntry.Children!.Add(new FileSystemEntry
            {
                Name = Path.GetFileName(file),
                IsDirectory = false,
                FullPath = relativePath,
            });
        }

        // Add subdirectories recursively
        foreach (var directory in Directory.GetDirectories(path))
        {
            directoryEntry.Children!.Add(GetDirectoryStructure(directory));
        }

        return directoryEntry;
    }
}
