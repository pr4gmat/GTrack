using GTrack.Core.Services;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Appearance;

namespace GTrack.Node.Desktop.ViewModels;

/// <summary>
/// ViewModel for the splash screen of the Node Desktop application.
/// ViewModel для стартового экрана Node Desktop приложения.
/// </summary>
public class SplashScreenViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<SplashScreenViewModel> _logger;
    private readonly IConfigThemeService _configService;
    
    /// <summary>
    /// Path to the splash screen image based on the theme.
    /// Путь к изображению стартового экрана в зависимости от темы.
    /// </summary>
    private string _imageSource;
    public string ImageSource
    {
        get => _imageSource;
        set => SetProperty(ref _imageSource, value);
    }

    /// <summary>
    /// Constructor initializes logger and theme service, and sets the initial image.
    /// Конструктор инициализирует логгер и сервис темы, устанавливает начальное изображение.
    /// </summary>
    public SplashScreenViewModel(ILogger<SplashScreenViewModel> logger, IConfigThemeService configService)
    {
        _logger = logger;
        _configService = configService;
        
        var config = _configService.Load();
        UpdateImage(config.Theme);
    }
    
    /// <summary>
    /// Updates the splash screen image according to the current application theme.
    /// Обновляет изображение стартового экрана в зависимости от текущей темы приложения.
    /// </summary>
    /// <param name="theme">Current application theme / Текущая тема приложения</param>
    private void UpdateImage(ApplicationTheme theme)
    {
        ImageSource = theme switch
        {
            ApplicationTheme.Dark => "/GTrack.Node.Desktop;component/Assets/gtrack-node-dark.png",
            ApplicationTheme.Light => "/GTrack.Node.Desktop;component/Assets/gtrack-node-light.png",
            _ => "/GTrack.Node.Desktop;component/Assets/gtrack-node-light.png"
        };
    }

    /// <summary>
    /// Called when the view is navigated to.
    /// Вызывается при переходе на представление.
    /// </summary>
    public void OnNavigatedTo(NavigationContext navigationContext) { }

    /// <summary>
    /// Determines if this instance can be reused for navigation.
    /// Определяет, может ли данный экземпляр использоваться повторно для навигации.
    /// </summary>
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;

    /// <summary>
    /// Called when navigating away from the view.
    /// Вызывается при уходе с представления.
    /// </summary>
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}