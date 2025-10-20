using System.Collections.ObjectModel;
using System.Windows;
using GTrack.Core.Services;
using GTrack.Dialog.Module.Modules;
using GTrack.Infrastructure.Services;
using GTrack.Node.Client;
using GTrack.Node.Desktop.ViewModels;
using GTrack.Node.Desktop.ViewModels.ServerAndClient;
using GTrack.Node.Desktop.Views;
using GTrack.Node.Desktop.Views.ServerAndClient;
using GTrack.ObserverMap.Module.Modules;
using GTrack.SGP4;
using GTrack.Station.Server;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace GTrack.Node.Desktop;

/// <summary>
/// Application entry point for the GTrack Node Desktop.
/// Точка входа приложения для GTrack Node Desktop.
/// </summary>
public partial class App : PrismApplication
{
    /// <summary>
    /// Creates the main window (shell) for the application.
    /// Создает главное окно приложения (shell).
    /// </summary>
    protected override Window CreateShell() => Container.Resolve<MainView>();

    /// <summary>
    /// Registers dependencies, services, loggers, and ViewModels in the DI container.
    /// Регистрирует зависимости, сервисы, логгеры и ViewModels в DI контейнере.
    /// </summary>
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Logging setup / Настройка логирования
        var monitoringLogCollection = new ObservableCollection<string>();
        var observableSink = new LogSink(monitoringLogCollection);
        containerRegistry.RegisterInstance<ILogSink>(observableSink);
        containerRegistry.RegisterInstance<ObservableCollection<string>>(monitoringLogCollection);
        
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
        
        var factory = new SerilogLoggerFactory(Log.Logger);
        containerRegistry.RegisterInstance<ILoggerFactory>(factory);
        containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

        // Theme configuration / Конфигурация темы
        string appName = "GTrackNodeDesktop";
        var configThemeService = new ConfigThemeService(appName);
        containerRegistry.RegisterInstance<IConfigThemeService>(configThemeService);

        // Core services / Основные сервисы
        containerRegistry.RegisterSingleton<IFileDialogService, FileDialogService>();
        containerRegistry.RegisterSingleton<ISatelliteObserver, SatelliteObserver>();
        containerRegistry.RegisterSingleton<IObserverLocationService, ObserverLocationService>();

        // Node/Server services / Сервисы Node/Server
        containerRegistry.RegisterSingleton<IGTrackNodeClient, GTrackNodeClient>();
        containerRegistry.RegisterSingleton<IGTrackStationServer, GTrackStationServer>();

        // ViewModels / ViewModels
        containerRegistry.RegisterSingleton<MainViewModel>();
        containerRegistry.RegisterForNavigation<SplashScreenView, SplashScreenViewModel>();
        containerRegistry.RegisterForNavigation<WorkspaceView, WorkspaceViewModel>();
        containerRegistry.RegisterForNavigation<GTrackNodeView, GTrackNodeViewModel>();
        containerRegistry.RegisterForNavigation<StationServerView, StationServerViewModel>();
        containerRegistry.RegisterForNavigation<NodeClientView, NodeClientViewModel>();
        containerRegistry.RegisterForNavigation<ObserverMapView, ObserverMapViewModel>();
        containerRegistry.RegisterDialog<ObserverAddView, ObserverAddViewModel>();
        containerRegistry.RegisterForNavigation<MonitoringView, MonitoringViewModel>();
        containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
    }

    /// <summary>
    /// Called when application initialization is complete. Navigate to the splash screen.
    /// Вызывается после инициализации приложения. Переход к splash screen.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        var regionManager = Container.Resolve<IRegionManager>();
        regionManager.RequestNavigate("MainRegion", nameof(SplashScreenView));
    }

    /// <summary>
    /// Configures module catalog. Adds Prism modules to the application.
    /// Конфигурирует каталог модулей. Добавляет Prism модули в приложение.
    /// </summary>
    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        base.ConfigureModuleCatalog(moduleCatalog);
        moduleCatalog.AddModule<DialogModule>();
        moduleCatalog.AddModule<ObserverMapModule>();
    }
}