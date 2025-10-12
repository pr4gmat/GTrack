using System.Collections.ObjectModel;
using System.Windows;
using GTrack.Control.Desktop.ViewModels;
using GTrack.Control.Desktop.ViewModels.Data;
using GTrack.Control.Desktop.ViewModels.Servers;
using GTrack.Control.Desktop.Views;
using GTrack.Control.Desktop.Views.Data;
using GTrack.Control.Desktop.Views.Servers;
using GTrack.Core.Services;
using GTrack.Dialog.Module.Modules;
using GTrack.Houston.Server;
using GTrack.Infrastructure.Services;
using GTrack.Node.Server;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace GTrack.Control.Desktop;

/// <summary>
/// Main application class for the WPF desktop app.
/// Главный класс приложения для WPF Desktop.
/// Handles dependency injection, logging, module registration, and navigation.
/// Отвечает за внедрение зависимостей, логирование, регистрацию модулей и навигацию.
/// </summary>
public partial class App : PrismApplication
{
    /// <summary>
    /// Creates the main window (shell) of the application.
    /// Создает главное окно (shell) приложения.
    /// </summary>
    protected override Window CreateShell() => Container.Resolve<MainView>();

    /// <summary>
    /// Registers all dependencies, services, ViewModels, and views.
    /// Регистрирует все зависимости, сервисы, ViewModel и представления.
    /// </summary>
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Set up observable collection for monitoring logs
        // Настройка ObservableCollection для логов мониторинга
        var monitoringLogCollection = new ObservableCollection<string>();
        var observableSink = new LogSink(monitoringLogCollection);
        containerRegistry.RegisterInstance<ILogSink>(observableSink);
        containerRegistry.RegisterInstance<ObservableCollection<string>>(monitoringLogCollection);

        // Configure Serilog logger
        // Настройка логгера Serilog
        string logFileName = $"logs/log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}_{Guid.NewGuid()}.txt";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: logFileName,
                rollingInterval: RollingInterval.Infinite,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.Sink(observableSink)
            .CreateLogger();

        // Register logging factories and ILogger<>
        // Регистрация фабрик логирования и ILogger<>
        var factory = new SerilogLoggerFactory(Log.Logger);
        containerRegistry.RegisterInstance<ILoggerFactory>(factory);
        containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

        // Register theme configuration service
        // Регистрация сервиса конфигурации тем
        string appName = "GTrackControlDesktop";
        var configThemeService = new ConfigThemeService(appName);
        containerRegistry.RegisterInstance<IConfigThemeService>(configThemeService);

        // Register core servers and services
        // Регистрация основных серверов и сервисов
        containerRegistry.RegisterSingleton<IGTHServer, GTHServer>();
        containerRegistry.RegisterSingleton<IGTrackNodeServer, GTrackNodeServer>();
        containerRegistry.RegisterSingleton<IFileDialogService, FileDialogService>();

        // Register main ViewModel and navigation views
        // Регистрация главной ViewModel и навигационных представлений
        containerRegistry.RegisterSingleton<MainViewModel>();
        containerRegistry.RegisterForNavigation<SplashScreenView, SplashScreenViewModel>();
        containerRegistry.RegisterForNavigation<WorkspaceView, WorkspaceViewModel>();
        containerRegistry.RegisterForNavigation<GTrackControlView, GTrackControlViewModel>();

        containerRegistry.RegisterForNavigation<NodeServerView, NodeServerViewModel>();
        containerRegistry.RegisterForNavigation<HoustonServerView, HoustonServerViewModel>();

        containerRegistry.RegisterForNavigation<TLESettingView, TLESettingViewModel>();
        containerRegistry.RegisterForNavigation<StationInstancesView, StationInstancesViewModel>();

        containerRegistry.RegisterForNavigation<MonitoringView, MonitoringViewModel>();
        containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
    }

    /// <summary>
    /// Called after Prism initializes the application.
    /// Вызывается после инициализации приложения Prism.
    /// Navigates to the splash screen.
    /// Выполняет переход на экран загрузки (SplashScreen).
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        var regionManager = Container.Resolve<IRegionManager>();
        regionManager.RequestNavigate("MainRegion", nameof(SplashScreenView));
    }

    /// <summary>
    /// Registers Prism modules, e.g., dialog module.
    /// Регистрирует модули Prism, например, модуль диалогов.
    /// </summary>
    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        base.ConfigureModuleCatalog(moduleCatalog);
        moduleCatalog.AddModule<DialogModule>();
    }
}