namespace PersonalSite.Application.DTOs.File;

public class MoveFileRequest
{
    public string CurrentFilePath { get; set; } = string.Empty;
    public string NewLocation { get; set; } = string.Empty;
}
