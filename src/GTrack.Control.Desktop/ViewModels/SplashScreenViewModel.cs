using GTrack.Core.Services;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Appearance;

namespace GTrack.Control.Desktop.ViewModels;

/// <summary>
/// ViewModel for the splash screen
/// ViewModel для стартового экрана (Splash Screen)
/// </summary>
public class SplashScreenViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<SplashScreenViewModel> _logger;
    private readonly IConfigThemeService _configService;

    private string _imageSource;
    /// <summary>
    /// Path to the splash screen image based on current theme
    /// Путь к изображению стартового экрана в зависимости от выбранной темы
    /// </summary>
    public string ImageSource
    {
        get => _imageSource;
        set => SetProperty(ref _imageSource, value);
    }

    /// <summary>
    /// Constructor
    /// Конструктор SplashScreenViewModel
    /// </summary>
    public SplashScreenViewModel(ILogger<SplashScreenViewModel> logger, IConfigThemeService configService)
    {
        _logger = logger;
        _configService = configService;

        // Load theme configuration and update splash screen image
        // Загружаем конфигурацию темы и обновляем изображение стартового экрана
        var config = _configService.Load();
        UpdateImage(config.Theme);
    }

    /// <summary>
    /// Updates the splash screen image according to the selected theme
    /// Обновляет изображение стартового экрана в зависимости от выбранной темы
    /// </summary>
    /// <param name="theme">Selected application theme / Выбранная тема приложения</param>
    private void UpdateImage(ApplicationTheme theme)
    {
        ImageSource = theme switch
        {
            ApplicationTheme.Dark => "/GTrack.Control.Desktop;component/Assets/gtrack-control-dark.png",
            ApplicationTheme.Light => "/GTrack.Control.Desktop;component/Assets/gtrack-control-light.png",
            _ => "/GTrack.Control.Desktop;component/Assets/gtrack-control-light.png"
        };
    }

    // INavigationAware implementation
    // Реализация INavigationAware
    public void OnNavigatedTo(NavigationContext navigationContext) { }
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}