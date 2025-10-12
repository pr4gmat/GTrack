using System.IO;
using GTrack.Core.Events;
using GTrack.Core.Models;
using GTrack.Core.Services;
using GTrack.Dialog.Module.Views;
using Microsoft.Extensions.Logging;

namespace GTrack.Control.Desktop.ViewModels.Data;

/// <summary>
/// ViewModel for selecting and loading a TLE file.
/// / Модель представления для выбора и загрузки TLE файла.
/// </summary>
public class TLESettingViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<TLESettingViewModel> _logger;
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IEventAggregator _eventAggregator;

    private string _tleFilePath;
    public string TleFilePath
    {
        get => _tleFilePath;
        set => SetProperty(ref _tleFilePath, value);
    }

    /// <summary>
    /// Command to load TLE file.
    /// / Команда для загрузки TLE файла.
    /// </summary>
    public DelegateCommand LoadTleCommand { get; }

    public TLESettingViewModel(ILogger<TLESettingViewModel> logger,
                               IFileDialogService fileDialogService, 
                               IDialogService dialogService,
                               IEventAggregator eventAggregator)
    {
        _logger = logger;
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _eventAggregator = eventAggregator;

        LoadTleCommand = new DelegateCommand(OnLoadTle);
    }

    /// <summary>
    /// Handles loading of a TLE file, publishes event, shows dialog.
    /// / Обрабатывает загрузку TLE файла, публикует событие и показывает диалог.
    /// </summary>
    private void OnLoadTle()
    {
        var filePath = _fileDialogService.OpenFile("TLE files (*.txt)|*.txt|All files (*.*)|*.*");

        if (!string.IsNullOrEmpty(filePath))
        {
            TleFilePath = filePath;
            var lines = File.ReadAllLines(filePath);

            if (lines.Length >= 3)
            {
                var tle = new TLE
                {
                    Name = lines[0].Trim(),
                    Line1 = lines[1].Trim(),
                    Line2 = lines[2].Trim()
                };

                _eventAggregator.GetEvent<TleFilePathSelectedEvent>().Publish(tle);

                _dialogService.ShowDialog(nameof(DialogMessageView), new DialogParameters
                {
                    { "message", $"Файл TLE загружен:\n{filePath}\n\nИмя: {tle.Name}\n{tle.Line1}\n{tle.Line2}" }
                });
            }
            else
            {
                _dialogService.ShowDialog(nameof(DialogMessageView), new DialogParameters
                {
                    { "message", $"Файл TLE содержит недостаточно данных: {filePath}" }
                });
            }
        }
        else
        {
            TleFilePath = string.Empty;
            _dialogService.ShowDialog(nameof(DialogMessageView), new DialogParameters
            {
                { "message", "Файл не выбран." }
            });
        }
    }

    public void OnNavigatedTo(NavigationContext navigationContext) { }
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}