using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using PersonalSite.WebUI.Client.Services;
using PersonalSite.WebUI.Client.Interfaces;
using PersonalSite.Application.DTOs.File;

namespace PersonalSite.WebUI.Client.Pages;

public partial class FileSystemPage : IDisposable
{
    [Inject]
    public HttpInterceptorService Interceptor { get; set; } = default!;
    [Inject]
    public IFileSystemService fileSystemService { get; set; } = default!;

    private IEnumerable<FileSystemEntry> fileSystemEntries = new List<FileSystemEntry>();
    private object selection;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Interceptor.RegisterEvent();
        var rootEntry = await fileSystemService.GetFileHierarchyAsync();
        if (rootEntry != null && rootEntry.Children != null)
        {
            fileSystemEntries = rootEntry.Children;
        }
    }

    private void DeleteFile()
    {
        FileSystemEntry? file = selection as FileSystemEntry;

        if (file == null)
            return;

        if (file.FullPath == ".")
            file.FullPath = string.Empty;

        fileSystemService.DeleteFile($"{file.FullPath}%{file.Name}");
    }

    private void LoadFiles(TreeExpandEventArgs args)
    {
        if (args.Value is FileSystemEntry entry && entry.Children != null)
        {
            args.Children.Data = entry.Children;
            args.Children.Text = GetTextForNode;
            args.Children.HasChildren = item => item is FileSystemEntry e && e.IsDirectory;
            args.Children.Template = FileOrFolderTemplate;
            args.Children.Checkable = _ => false;
        }
    }

    private string GetTextForNode(object data)
    {
        return data is FileSystemEntry entry ? entry.Name : string.Empty;
    }

    private RenderFragment<RadzenTreeItem> FileOrFolderTemplate = (context) => builder =>
    {
        if (context.Value is FileSystemEntry entry)
        {
            bool isDirectory = entry.IsDirectory;

            builder.OpenComponent<RadzenIcon>(0);
            builder.AddAttribute(1, nameof(RadzenIcon.Icon), isDirectory ? "folder" : "insert_drive_file");
            builder.CloseComponent();
            builder.AddContent(3, context.Text);
        }
    };

    public void Dispose() => Interceptor.DisposeEvent();
}
