namespace GTrack.Core.Services;

/// <summary>
/// Service interface for showing file dialogs.
/// </summary>
///
/// <summary>
/// Интерфейс сервиса для отображения диалогов выбора файлов.
/// </summary>
public interface IFileDialogService
{
    /// <summary>
    /// Opens a file dialog for selecting a file to open.
    /// </summary>
    ///
    /// <summary>
    /// Открывает диалог выбора файла для открытия.
    /// </summary>
    /// <param name="filter">Optional file filter string.</param>
    /// <param name="filter">Необязательная строка фильтра файлов.</param>
    /// <returns>Path to the selected file or null if cancelled.</returns>
    /// <returns>Путь к выбранному файлу или null, если операция отменена.</returns>
    string? OpenFile(string filter = "All files (*.*)|*.*");

    /// <summary>
    /// Opens a file dialog for selecting a file path to save.
    /// </summary>
    ///
    /// <summary>
    /// Открывает диалог выбора файла для сохранения.
    /// </summary>
    /// <param name="filter">Optional file filter string.</param>
    /// <param name="filter">Необязательная строка фильтра файлов.</param>
    /// <returns>Path to save the file or null if cancelled.</returns>
    /// <returns>Путь для сохранения файла или null, если операция отменена.</returns>
    string? SaveFile(string filter = "All files (*.*)|*.*");
}