﻿using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using PersonalSite.WebUI.Client.Services;
using PersonalSite.WebUI.Client.Interfaces;
using PersonalSite.Application.DTOs.File;
using System;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace PersonalSite.WebUI.Client.Pages;

public partial class FileSystemPage : IDisposable
{
    [Inject]
    public HttpInterceptorService Interceptor { get; set; } = default!;
    [Inject]
    public IFileSystemService fileSystemService { get; set; } = default!;
    [Inject]
    public IConfiguration configuration { get; set; } = default;

    private IEnumerable<FileSystemEntry> fileSystemEntries = new List<FileSystemEntry>();
    private object selection = default!;
    private FileSystemEntry draggedFile = default!;
    private string folderName = string.Empty;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Interceptor.RegisterEvent();
        await SetFileSystemEntries();
    }

    private async Task SetFileSystemEntries()
    {
        try
        {
            fileSystemEntries = null;
            var rootEntry = await fileSystemService.GetFileHierarchyAsync();
            if (rootEntry != null && rootEntry.Children != null)
            {
                fileSystemEntries = rootEntry.Children;
            }
            StateHasChanged();
        }
        catch (Exception)
        {
            //TODO toaster
        }
    }

    private async void DeleteFile()
    {
        try
        {
            FileSystemEntry? file = selection as FileSystemEntry;

            if (file == null)
                return;

            if (file.FullPath == ".")
                file.FullPath = string.Empty;

            string encodedParam = HttpUtility.UrlEncode($"{file.FullPath}/{file.Name}");
            await fileSystemService.DeleteFile(encodedParam);
            await SetFileSystemEntries();
        }
        catch (Exception e)
        {
            //TODO toaster
        }
    }

    private async void DownloadFile()
    {
        try
        {
            FileSystemEntry? file = selection as FileSystemEntry;

            if (file == null)
                return;

            if (file.FullPath == ".")
                file.FullPath = string.Empty;

            string encodedParam = HttpUtility.UrlEncode($"{file.FullPath}/{file.Name}");
            await fileSystemService.DownloadFile(encodedParam);
        }
        catch (Exception e)
        {
            //TODO toaster
        }
    }

    private async void CreateFolder()
    {
        try
        {
            FileSystemEntry? file = selection as FileSystemEntry;
            string fullpath = file == null ? string.Empty : file.FullPath;

            if (fullpath == ".")
                fullpath = string.Empty;

            CreateFolderRequest folderRequest = new CreateFolderRequest()
            {
                FolderPath = Path.Combine(fullpath, folderName)
            };
            await fileSystemService.CreateFolder(folderRequest);
            await SetFileSystemEntries();
        }
        catch (Exception e)
        {
            //TODO toaster
        }
    }

    private void ItemRender(TreeItemRenderEventArgs args)
    {
        var file = (FileSystemEntry)args.Value;
        MoveFileRequest moveFileRequest = new MoveFileRequest();

        // Allow drag of all items except the root item.
        if (!file.IsDirectory)
        {
            args.Attributes.Add("title", "Drag item to reorder");
            args.Attributes.Add("style", "cursor:grab");
            args.Attributes.Add("draggable", "true");
            args.Attributes.Add("ondragstart", EventCallback.Factory.Create<DragEventArgs>(this, () =>
            {
                if (draggedFile == null)
                {
                    draggedFile = file;
                }
            }));
        }

        // Allow drop over any item including the root item.
        args.Attributes.Add("ondragover", "event.preventDefault()");
        args.Attributes.Add("ondrop", EventCallback.Factory.Create<DragEventArgs>(this, async () =>
        {
            try
            {
                moveFileRequest.CurrentFilePath = $"{draggedFile.FullPath}/{draggedFile.Name}";
                moveFileRequest.NewLocation = $"{file.FullPath}/{draggedFile.Name}";
                await fileSystemService.MoveFile(moveFileRequest);
                await SetFileSystemEntries();
                draggedFile = null;
            }
            catch (Exception e)
            {
                //TODO toaster
            }
        }));
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
    private async void OnUploadCompleted(UploadCompleteEventArgs args)
    {
        await SetFileSystemEntries();
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

    private string GetApiUrl()
    {
        return configuration["ApiUrl"] + "File";
    }
    public void Dispose() => Interceptor.DisposeEvent();
}
