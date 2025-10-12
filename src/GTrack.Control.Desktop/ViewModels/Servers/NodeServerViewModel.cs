using System.Collections.ObjectModel;
using System.Net;
using GTrack.Core.Events.NodeServer;
using GTrack.Node.Server;
using Microsoft.Extensions.Logging;

namespace GTrack.Control.Desktop.ViewModels.Servers;

/// <summary>
/// ViewModel for controlling the Node server (start/stop, node updates)
/// / Модель представления для управления Node-сервером (запуск/остановка, обновления узлов)
/// </summary>
public class NodeServerViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<NodeServerViewModel> _logger;
    private readonly IGTrackNodeServer _server;
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogService _dialogService;
    
    private string _ip = "127.0.0.1";
    public string IP
    {
        get => _ip;
        set => SetProperty(ref _ip, value);
    }

    private int _port = 9000;
    public int Port
    {
        get => _port;
        set => SetProperty(ref _port, value);
    }
    
    private ObservableCollection<Core.Models.Node> _nodes = new();
    public ObservableCollection<Core.Models.Node> Nodes
    {
        get => _nodes;
        set => SetProperty(ref _nodes, value);
    }

    public DelegateCommand StartServerCommand { get; }
    public DelegateCommand StopServerCommand { get; }
    
    public NodeServerViewModel(
        ILogger<NodeServerViewModel> logger,
        IGTrackNodeServer server,
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
            _eventAggregator.GetEvent<NodeServerStartedEvent>().Publish();
            _logger.LogInformation("[Node server] Сервер успешно запущен.");
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            ShowMessage("Node-сервер успешно запущен.");
        };

        _server.OnStopped += () =>
        {
            _eventAggregator.GetEvent<NodeServerStoppedEvent>().Publish();
            _logger.LogInformation("[Node server] Сервер успешно остановлен.");
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            ShowMessage("Node-сервер успешно остановлен.");
        };
        
        _server.OnMessageReceivedNode += node =>
        {
            _eventAggregator.GetEvent<NodeServerNodeReceivedEvent>().Publish(node);
            _logger.LogInformation("[Node server] Получено обновление от узла {Node}", node.Id);

            var existing = Nodes.FirstOrDefault(n => n.Id == node.Id);
            if (existing != null)
            {
                var idx = Nodes.IndexOf(existing);
                Nodes[idx] = node;
            }
            else
            {
                Nodes.Add(node);
            }
        };
        
        _server.OnNodeDisconnected += node =>
        {
            _eventAggregator.GetEvent<NodeServerNodeDisconnectedEvent>().Publish(node);

            var existing = Nodes.FirstOrDefault(n => n.Id == node.Id);
            if (existing != null)
            {
                Nodes.Remove(existing);
            }
        };
    }
    
    private bool CanStartServer() => !_server.IsRunning;
    private bool CanStopServer() => _server.IsRunning;
    
    private async void OnStartServer()
    {
        if (!ValidateInputs())
            return;

        if (!_server.IsRunning)
        {
            try
            {
                _logger.LogInformation("[Node server] Попытка запуска сервера...");
                await _server.StartAsync(IP, Port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Node server] Ошибка при запуске сервера.");
                _eventAggregator.GetEvent<NodeServerCrashedEvent>().Publish(ex);
            }
        }
    }

    private async void OnStopServer()
    {
        if (_server.IsRunning)
        {
            try
            {
                _logger.LogInformation("[Node server] Попытка остановки сервера...");
                await _server.StopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Node server] Ошибка при остановке сервера.");
                _eventAggregator.GetEvent<NodeServerCrashedEvent>().Publish(ex);
            }
        }
    }
    
    private bool ValidateInputs()
    {
        if (!IPAddress.TryParse(IP, out _))
        {
            ShowMessage("Указан некорректный IP-адрес.");
            _logger.LogWarning("[Node server] Некорректный IP: {IP}", IP);
            return false;
        }

        if (Port < 1024 || Port > 65535)
        {
            ShowMessage("Порт должен быть в диапазоне 1024–65535.");
            _logger.LogWarning("[Node server] Некорректный порт: {Port}", Port);
            return false;
        }

        return true;
    }

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