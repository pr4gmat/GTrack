using System.Net;
using GTrack.Core.Events.HoustonServer;
using GTrack.Houston.Server;
using Microsoft.Extensions.Logging;

namespace GTrack.Control.Desktop.ViewModels.Servers;

/// <summary>
/// ViewModel for controlling the Houston server (start/stop and validation)
/// / Модель представления для управления сервером Houston (запуск/остановка и валидация)
/// </summary>
public class HoustonServerViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<HoustonServerViewModel> _logger;
    private readonly IGTHServer _server;
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogService _dialogService;

    private string _ip;
    public string IP
    {
        get => _ip;
        set => SetProperty(ref _ip, value);
    }

    private int _port;
    public int Port
    {
        get => _port;
        set => SetProperty(ref _port, value);
    }

    public DelegateCommand StartServerCommand { get; }
    public DelegateCommand StopServerCommand { get; }

    public HoustonServerViewModel(
        ILogger<HoustonServerViewModel> logger,
        IGTHServer server,
        IEventAggregator eventAggregator,
        IDialogService dialogService)
    {
        _logger = logger;
        _server = server;
        _eventAggregator = eventAggregator;
        _dialogService = dialogService;

        StartServerCommand = new DelegateCommand(OnStartServer, CanStartServer);
        StopServerCommand = new DelegateCommand(OnStopServer, CanStopServer);

        _server.OnStarted += () =>
        {
            _eventAggregator.GetEvent<HoustonServerStartedEvent>().Publish();
            _logger.LogInformation("[Houston server] Сервер успешно запущен.");
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            ShowMessage("Сервер успешно запущен.");
        };

        _server.OnStopped += () =>
        {
            _eventAggregator.GetEvent<HoustonServerStoppedEvent>().Publish();
            _logger.LogInformation("[Houston server] Сервер успешно остановлен.");
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            ShowMessage("Сервер успешно остановлен.");
        };
    }

    private bool CanStartServer() => !_server.IsRunning; // / Can start only if server is stopped
    private bool CanStopServer() => _server.IsRunning;  // / Can stop only if server is running

    /// <summary>
    /// Start the Houston server asynchronously with input validation
    /// / Асинхронный запуск сервера Houston с проверкой введённых данных
    /// </summary>
    private async void OnStartServer()
    {
        if (!ValidateInputs()) return;

        try
        {
            _logger.LogInformation("[Houston server] Попытка запуска сервера...");
            await _server.StartAsync(IP, Port);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Houston server] Ошибка при запуске сервера.");
            _eventAggregator.GetEvent<HoustonServerCrashedEvent>().Publish(ex);
        }
    }

    /// <summary>
    /// Stop the Houston server asynchronously
    /// / Асинхронная остановка сервера Houston
    /// </summary>
    private async void OnStopServer()
    {
        try
        {
            _logger.LogInformation("[Houston server] Попытка остановки сервера...");
            await _server.StopAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Houston server] Ошибка при остановке сервера.");
            _eventAggregator.GetEvent<HoustonServerCrashedEvent>().Publish(ex);
        }
    }

    /// <summary>
    /// Validate IP and port inputs
    /// / Проверка корректности IP и порта
    /// </summary>
    private bool ValidateInputs()
    {
        if (!IPAddress.TryParse(IP, out _))
        {
            ShowMessage("Указан некорректный IP-адрес. Пожалуйста, введите правильный адрес.");
            _logger.LogWarning("[Houston server] Некорректный IP-адрес: {IP}", IP);
            return false;
        }

        if (Port < 1024 || Port > 65535)
        {
            ShowMessage("Порт должен быть в диапазоне 1024–65535.");
            _logger.LogWarning("[Houston server] Некорректный порт: {Port}", Port);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Show message dialog
    /// / Показать сообщение в диалоговом окне
    /// </summary>
    private void ShowMessage(string message)
    {
        _dialogService.ShowDialog("DialogMessageView",
            new DialogParameters { { "message", message } },
            r => { });
    }

    public void OnNavigatedTo(NavigationContext navigationContext) { }
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}