using GTrack.Core.Services;
using Microsoft.Win32;

namespace GTrack.Infrastructure.Services;

/// <summary>
/// Service for showing open and save file dialogs.
/// </summary>
/// <summary>
/// Сервис для отображения диалогов открытия и сохранения файлов.
/// </summary>
public class FileDialogService : IFileDialogService
{
    public string? OpenFile(string filter = "All files (*.*)|*.*")
    {
        var dialog = new OpenFileDialog { Filter = filter };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? SaveFile(string filter = "All files (*.*)|*.*")
    {
        var dialog = new OpenFileDialog { Filter = filter };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}