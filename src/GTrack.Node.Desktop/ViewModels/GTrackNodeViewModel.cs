using System.Collections.ObjectModel;
using GTrack.Core.Events;
using GTrack.Core.Events.NodeClient;
using GTrack.Core.Events.StationServer;
using GTrack.Core.Models;
using GTrack.Core.Services;
using GTrack.Node.Client;
using GTrack.SGP4;
using GTrack.Station.Server;
using Microsoft.Extensions.Logging;

namespace GTrack.Node.Desktop.ViewModels;

/// <summary>
/// ViewModel for GTrack Node.
/// Handles Node Client, Station Server, TLE data, and satellite observation.
/// ViewModel для GTrack Node.
/// Управляет Node Client, Station Server, данными TLE и наблюдением спутников.
/// </summary>
public class GTrackNodeViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<GTrackNodeViewModel> _logger;
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogService _dialogService;
    private readonly IGTrackStationServer _server;
    private readonly IGTrackNodeClient _client;
    private readonly ISatelliteObserver _satelliteObserver;
    private readonly IObserverLocationService _observerLocationService;

    // Status of Node Client
    // Статус Node Client
    private ServerStatus _nodeClient;
    public ServerStatus NodeClient
    {
        get => _nodeClient;
        set => SetProperty(ref _nodeClient, value);
    }
    
    private ServerStatus _stationServer;
    public ServerStatus StationServer
    {
        get => _stationServer;
        set => SetProperty(ref _stationServer, value);
    }
    
    // List of stations known to the node
    // Список станций, известных Node
    private ObservableCollection<Core.Models.Station> _stations = new();
    public ObservableCollection<Core.Models.Station> Stations
    {
        get => _stations;
        set => SetProperty(ref _stations, value);
    }

    // Current satellite observation parameters
    // Параметры текущего наблюдения спутника
    private ObservableCollection<TargetTracking> _observationParameters = new();
    public ObservableCollection<TargetTracking> ObservationParameters
    {
        get => _observationParameters;
        set => SetProperty(ref _observationParameters, value);
    }

    // Currently selected station
    // Текущая выбранная станция
    private Core.Models.Station _currentStation;
    public Core.Models.Station CurrentStation
    {
        get => _currentStation;
        set => SetProperty(ref _currentStation, value);
    }

    // Current TLE for satellite tracking
    // Текущий TLE для отслеживания спутника
    private TLE _currentTLE;
    public TLE CurrentTLE
    {
        get => _currentTLE;
        set => SetProperty(ref _currentTLE, value);
    }

    // Observer location for satellite calculations
    // Расположение наблюдателя для расчёта положения спутника
    private ObserverLocation _currentObserverLocation;
    public ObserverLocation CurrentObserverLocation
    {
        get => _currentObserverLocation;
        set => SetProperty(ref _currentObserverLocation, value);
    }

    /// <summary>
    /// Constructor initializes services, event subscriptions, and default server statuses.
    /// Конструктор инициализирует сервисы, подписки на события и начальные статусы серверов.
    /// </summary>
    public GTrackNodeViewModel(
        ILogger<GTrackNodeViewModel> logger, IEventAggregator eventAggregator, 
        IGTrackNodeClient client, IDialogService dialogService, 
        ISatelliteObserver satelliteObserver, IGTrackStationServer server,
        IObserverLocationService observerLocationService)
    {
        _logger = logger;
        _eventAggregator = eventAggregator;
        _dialogService = dialogService;
        _client = client;
        _satelliteObserver = satelliteObserver;
        _server = server;
        _observerLocationService = observerLocationService;

        NodeClient = new ServerStatus { Name = "Node", Status = false };
        StationServer = new ServerStatus { Name = "Houston", Status = false };

        // Subscribe to Node Client events
        // Подписка на события Node Client
        _eventAggregator.GetEvent<NodeClientConnectedEvent>().Subscribe(() => NodeClient.Status = true);
        _eventAggregator.GetEvent<NodeClientDisconnectedEvent>().Subscribe(() => NodeClient.Status = false);
        _eventAggregator.GetEvent<NodeClientCrashedEvent>().Subscribe(ex => NodeClient.Status = false);

        _eventAggregator.GetEvent<NodeClientTleReceivedEvent>().Subscribe(data =>
        {
            CurrentStation = data.Station;
            CurrentTLE = data.Tle;
            Task.Run(async () => await DataCounting()); // Start observation calculations
        });

        _eventAggregator.GetEvent<ObserverLocationAddedEvent>().Subscribe(observer =>
        {
            CurrentObserverLocation = observer;
            Task.Run(async () => await DataCounting());
        });
        
        _eventAggregator.GetEvent<ObserverLocationDeletedEvent>().Subscribe(() =>
        {
            _logger.LogInformation("[UI] Observer location deleted — clearing data / Точка наблюдения удалена — очищаем данные");

            CurrentObserverLocation = null;

            App.Current.Dispatcher.Invoke(() =>
            {
                ObservationParameters.Clear();
            });
        });
        
        // Subscribe to Station Server events
        // Подписка на события Station Server
        _eventAggregator.GetEvent<StationServerStartedEvent>().Subscribe(() =>
        {
            StationServer.Status = true;
            _logger.LogInformation("[UI] StationServer started / StationServer запущен");
        });

        _eventAggregator.GetEvent<StationServerStoppedEvent>().Subscribe(() =>
        {
            StationServer.Status = false;
            _logger.LogInformation("[UI] StationServer stopped / StationServer остановлен");
        });

        _eventAggregator.GetEvent<StationServerCrashedEvent>().Subscribe(ex =>
        {
            StationServer.Status = false;
            _logger.LogError(ex, "[UI] StationServer crashed / StationServer аварийно завершился");
        });

        _eventAggregator.GetEvent<StationServerStationReceivedEvent>().Subscribe(station =>
        {
            _client.Stations.Add(station);
            Stations.Add(station);
        });

        _eventAggregator.GetEvent<StationServerStationDisconnectedEvent>().Subscribe(station =>
        {
            _client.Stations.Remove(station);
            Stations.Remove(station);
        });
    }

    /// <summary>
    /// Performs satellite observation calculations and broadcasts data.
    /// Выполняет расчёты наблюдения спутника и рассылает данные.
    /// </summary>
    private async Task DataCounting()
    {
        if (CurrentTLE != null && CurrentObserverLocation != null)
        {
            var result = await _satelliteObserver.ObserveFromTleDataAsync(
                CurrentTLE.Name,
                CurrentTLE.Line1,
                CurrentTLE.Line2,
                CurrentObserverLocation.Latitude,
                CurrentObserverLocation.Longitude, 
                CurrentObserverLocation.Height,
                txFrequencyHz: 437.5e6);

            var firstSatelliteName = result.FirstOrDefault()?.Name;

            while (_nodeClient.Status)
            {
                var obs = _satelliteObserver.UpdateSatelliteData(firstSatelliteName);

                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    ObservationParameters.Clear();
                    ObservationParameters.Add(new TargetTracking()
                    {
                        Time = $"{obs.Time:HH:mm:ss} UTC",
                        Name = $"{obs.Name}",
                        Azimuth = $"{obs.Azimuth:F2}°",
                        Elevation = $"{obs.Elevation:F2}°",
                        Range = $"{obs.Range:F2} km",
                        Doppler = $"{obs.Doppler:F2} Hz"
                    });
                });

                if (obs.Elevation > 0)
                {
                    string message =
                        $"NSRD:{CurrentStation?.Id}" +
                        $"|{obs.Time:HH:mm:ss} UTC" +
                        $"|{obs.Name}" +
                        $"|{obs.Azimuth:F2}°" +
                        $"|{obs.Elevation:F2}°" +
                        $"|{obs.Range:F2} km" +
                        $"|{obs.Doppler:F2} Hz";

                    await _server.BroadcastAsync(message);
                }

                await Task.Delay(1000); // Update every second / Обновление каждую секунду
            }
        }
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

    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        try
        {
            var savedLocations = await _observerLocationService.LoadAsync();
            if (savedLocations.Any())
            {
                CurrentObserverLocation = savedLocations.First();
                _logger.LogInformation($"[UI] Загрузка сохранённой точки наблюдения: {CurrentObserverLocation.Name}");
                Task.Run(async () => await DataCounting());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке точки наблюдения");
        }
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}