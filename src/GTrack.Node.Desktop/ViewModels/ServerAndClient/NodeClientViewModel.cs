using System.Collections.ObjectModel;
using System.Net;
using GTrack.Core.Events.NodeClient;
using GTrack.Core.Models;
using GTrack.Node.Client;
using Microsoft.Extensions.Logging;

namespace GTrack.Node.Desktop.ViewModels.ServerAndClient;

/// <summary>
/// ViewModel for Node Client connection and management.
/// ViewModel для управления Node Client и подключением к серверу.
/// </summary>
public class NodeClientViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<NodeClientViewModel> _logger;
    private readonly IGTrackNodeClient _client;
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogService _dialogService;

    // IP address of the server to connect to
    // IP-адрес сервера для подключения
    private string _ip;
    public string IP
    {
        get => _ip;
        set => SetProperty(ref _ip, value);
    }

    // Port of the server
    // Порт сервера
    private int _port;
    public int Port
    {
        get => _port;
        set => SetProperty(ref _port, value);
    }

    // List of stations received from server
    // Список станций, полученных с сервера
    private ObservableCollection<Core.Models.Station> _stations = new();
    public ObservableCollection<Core.Models.Station> Stations
    {
        get => _stations;
        set => SetProperty(ref _stations, value);
    }

    // Commands for connecting/disconnecting server
    // Команды для подключения/отключения от сервера
    public DelegateCommand ConnectServerCommand { get; }
    public DelegateCommand DisconnectServerCommand { get; }

    /// <summary>
    /// Constructor initializes services, commands, and client event handlers.
    /// Конструктор инициализирует сервисы, команды и обработчики событий клиента.
    /// </summary>
    public NodeClientViewModel(
        ILogger<NodeClientViewModel> logger,
        IGTrackNodeClient client,
        IEventAggregator eventAggregator,
        IDialogService dialogService)
    {
        _logger = logger;
        _client = client;
        _eventAggregator = eventAggregator;
        _dialogService = dialogService;

        ConnectServerCommand = new DelegateCommand(OnConnectServer, CanConnectServer);
        DisconnectServerCommand = new DelegateCommand(OnDisconnectServer, CanDisconnectServer);

        // Node client events / События Node Client
        _client.OnConnected += () =>
        {
            _eventAggregator.GetEvent<NodeClientConnectedEvent>().Publish();
            RaiseCommands();
            ShowMessage("Подключение к серверу успешно");
        };

        _client.OnDisconnected += () =>
        {
            _eventAggregator.GetEvent<NodeClientDisconnectedEvent>().Publish();
            ShowMessage("Отключение от сервера выполнено");
            RaiseCommands();
        };

        _client.OnNodeIdReceived += nodeId =>
        {
            // TODO: handle node ID if necessary
        };

        _client.OnTleReceived += (station, tle) =>
        {
            _eventAggregator.GetEvent<NodeClientTleReceivedEvent>().Publish((station, tle));
        };

        _client.OnMessageReceived += msg =>
        {
            try
            {
                Stations.Clear();
                foreach (var s in _client.Stations)
                    Stations.Add(s);

                foreach (var s in _client.Stations)
                    _eventAggregator.GetEvent<NodeClientStationReceivedEvent>().Publish(s);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Node client VM] Ошибка обработки сообщения");
                _eventAggregator.GetEvent<NodeClientCrashedEvent>().Publish(ex);
            }
        };
    }

    /// <summary>
    /// Checks if connection is possible.
    /// Проверяет возможность подключения.
    /// </summary>
    private bool CanConnectServer() => !_client.IsConnected;

    /// <summary>
    /// Checks if disconnection is possible.
    /// Проверяет возможность отключения.
    /// </summary>
    private bool CanDisconnectServer() => _client.IsConnected;

    /// <summary>
    /// Connects to the server asynchronously.
    /// Асинхронное подключение к серверу.
    /// </summary>
    private async void OnConnectServer()
    {
        if (!ValidateInputs()) return;

        try
        {
            await _client.StartAsync(IP, Port);
            _eventAggregator.GetEvent<NodeClientIPandPortEvent>().Publish(new Core.Models.Node
            {
                IP = this.IP,
                Port = this.Port,
            });
        }
        catch (Exception ex)
        {
            ShowMessage("Ошибка подключения" + ex.Message);
        }
    }

    /// <summary>
    /// Disconnects from the server.
    /// Отключение от сервера.
    /// </summary>
    private void OnDisconnectServer()
    {
        try
        {
            _logger.LogInformation("[Node client VM] Disconnecting from server...");
            _client.Stop();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Node client VM] Disconnect error / Ошибка при отключении");
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
            ShowMessage("Указан некорректный IP-адрес");
            _logger.LogWarning("[Node client VM] Указан некорректный IP-адрес: {IP}", IP);
            return false;
        }

        if (Port < 1024 || Port > 65535)
        {
            ShowMessage("Порт должен быть в диапазоне 1024–65535 / Port must be 1024–65535.");
            _logger.LogWarning("[Node client VM] Invalid port: {Port}", Port);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Shows a dialog message.
    /// Показывает сообщение через диалоговое окно.
    /// </summary>
    private void ShowMessage(string message)
    {
        _dialogService.ShowDialog("DialogMessageView",
            new DialogParameters { { "message", message } },
            _ => { });
    }

    /// <summary>
    /// Refreshes the availability of commands.
    /// Обновляет доступность команд.
    /// </summary>
    private void RaiseCommands()
    {
        ConnectServerCommand.RaiseCanExecuteChanged();
        DisconnectServerCommand.RaiseCanExecuteChanged();
    }

    public void OnNavigatedTo(NavigationContext navigationContext) { }
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}