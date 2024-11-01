using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalSite.Application.DTOs.File;

public class FileSystemEntry
{
    public string Name { get; set; } = string.Empty;
    public bool IsDirectory { get; set; }
    public string FullPath { get; set; } = string.Empty;
    public List<FileSystemEntry>? Children { get; set; }
}