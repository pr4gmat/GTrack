using System.Collections.ObjectModel;
using System.Net;
using GTrack.Core.Events.StationServer;
using GTrack.Node.Client;
using GTrack.Station.Server;
using Microsoft.Extensions.Logging;

namespace GTrack.Node.Desktop.ViewModels.ServerAndClient;

/// <summary>
/// ViewModel for managing the Station Server, starting/stopping and handling connected stations.
/// ViewModel для управления сервером станций, запуска/остановки и обработки подключённых станций.
/// </summary>
public class StationServerViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<StationServerViewModel> _logger;
    private readonly IGTrackStationServer _server;
    private readonly IGTrackNodeClient _client;
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogService _dialogService;

    // IP address to bind the server to / IP-адрес для привязки сервера
    private string _ip = "127.0.0.1";
    public string IP
    {
        get => _ip;
        set => SetProperty(ref _ip, value);
    } 

    // Port to bind the server / Порт для сервера
    private int _port = 9000;
    public int Port
    {
        get => _port;
        set => SetProperty(ref _port, value);
    }

    // List of connected stations / Список подключенных станций
    private ObservableCollection<Core.Models.Station> _stations = new();
    public ObservableCollection<Core.Models.Station> Stations
    {
        get => _stations;
        set => SetProperty(ref _stations, value);
    }

    // Commands for starting/stopping the server / Команды для запуска/остановки сервера
    public DelegateCommand StartServerCommand { get; }
    public DelegateCommand StopServerCommand { get; }

    /// <summary>
    /// Constructor initializes server, client, commands and subscribes to events.
    /// Конструктор инициализирует сервер, клиент, команды и подписывается на события.
    /// </summary>
    public StationServerViewModel(
        ILogger<StationServerViewModel> logger,
        IGTrackStationServer server,
        IEventAggregator eventAggregator,
        IDialogService dialogService, 
        IGTrackNodeClient client)
    {
        _logger = logger;
        _server = server;
        _eventAggregator = eventAggregator;
        _dialogService = dialogService;
        _client = client;

        StartServerCommand = new DelegateCommand(OnStartServer, CanStartServer);
        StopServerCommand = new DelegateCommand(OnStopServer, CanStopServer);

        // Server events / События сервера
        _server.OnStarted += () =>
        {
            _eventAggregator.GetEvent<StationServerStartedEvent>().Publish();
            _logger.LogInformation("[Station server] Сервер успешно запущен / Server started successfully.");
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            ShowMessage("Сервер станций успешно запущен / Station server started successfully.");
        };

        _server.OnStopped += () =>
        {
            _eventAggregator.GetEvent<StationServerStoppedEvent>().Publish();
            _logger.LogInformation("[Station server] Сервер успешно остановлен / Server stopped successfully.");
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            ShowMessage("Сервер станций успешно остановлен / Station server stopped successfully.");
        };

        // When a station sends a message / Когда станция отправляет сообщение
        server.OnMessageReceived += station =>
        {
            _eventAggregator.GetEvent<StationServerStationReceivedEvent>().Publish(station);
            Stations.Add(station);
        };

        // When a station disconnects / Когда станция отключается
        _server.OnStationDisconnected += station =>
        {
            _eventAggregator.GetEvent<StationServerStationDisconnectedEvent>().Publish(station);
            _logger.LogInformation("[Station server] Станция {Station} отключилась / Station {Station} disconnected", station.Id);

            var existing = Stations.FirstOrDefault(s => s.Id == station.Id);
            if (existing != null)
            {
                Stations.Remove(existing);
            }
        };
    }

    /// <summary>
    /// Determines if the server can start.
    /// Проверяет возможность запуска сервера.
    /// </summary>
    private bool CanStartServer() => !_server.IsRunning;

    /// <summary>
    /// Determines if the server can stop.
    /// Проверяет возможность остановки сервера.
    /// </summary>
    private bool CanStopServer() => _server.IsRunning;

    /// <summary>
    /// Starts the server asynchronously.
    /// Асинхронный запуск сервера.
    /// </summary>
    private async void OnStartServer()
    {
        if (!ValidateInputs()) return;

        if (!_server.IsRunning)
        {
            try
            {
                if (!string.IsNullOrEmpty(_client.NodeId))
                {
                    _logger.LogInformation("[Station server] Попытка запуска сервера / Attempting to start server...");
                    await _server.StartAsync(IPAddress.Parse(IP), Port, _client.NodeId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Station server] Ошибка при запуске сервера / Server start error.");
                _eventAggregator.GetEvent<StationServerCrashedEvent>().Publish(ex);
            }
        }
    }

    /// <summary>
    /// Stops the server asynchronously.
    /// Асинхронная остановка сервера.
    /// </summary>
    private async void OnStopServer()
    {
        if (_server.IsRunning)
        {
            try
            {
                _logger.LogInformation("[Station server] Попытка остановки сервера / Attempting to stop server...");
                await _server.StopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Station server] Ошибка при остановке сервера / Server stop error.");
                _eventAggregator.GetEvent<StationServerCrashedEvent>().Publish(ex);
            }
        }
    }

    /// <summary>
    /// Validates IP and port inputs.
    /// Проверяет корректность IP и порта.
    /// </summary>
    private bool ValidateInputs()
    {
        if (!IPAddress.TryParse(IP, out _))
        {
            ShowMessage("Указан некорректный IP-адрес / Invalid IP address.");
            _logger.LogWarning("[Node server] Некорректный IP / Invalid IP: {IP}", IP);
            return false;
        }

        if (Port < 1024 || Port > 65535)
        {
            ShowMessage("Порт должен быть в диапазоне 1024–65535 / Port must be 1024–65535.");
            _logger.LogWarning("[Node server] Некорректный порт / Invalid port: {Port}", Port);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Shows a dialog message to the user.
    /// Показывает сообщение пользователю через диалоговое окно.
    /// </summary>
    private void ShowMessage(string message)
    {
        _dialogService.ShowDialog("DialogMessageView",
            new DialogParameters { { "message", message } },
            _ => { });
    }

    public void OnNavigatedTo(NavigationContext navigationContext) { }
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}