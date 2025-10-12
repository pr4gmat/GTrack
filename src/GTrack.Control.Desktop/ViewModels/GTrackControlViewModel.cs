using System.Collections.ObjectModel;
using GTrack.Core.Events;
using GTrack.Core.Events.HoustonServer;
using GTrack.Core.Events.NodeServer;
using GTrack.Core.Models;
using GTrack.Node.Server;
using Microsoft.Extensions.Logging;

namespace GTrack.Control.Desktop.ViewModels;

/// <summary>
/// ViewModel for the main GTrack control panel
/// Основная ViewModel для управления GTrack: отслеживание серверов и отправка TLE на станции
/// </summary>
public class GTrackControlViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<GTrackControlViewModel> _logger;
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogService _dialogService;
    private readonly IGTrackNodeServer _gTrackNodeServer;

    private readonly HashSet<string> _stationsSent = new();
    private string _lastStationSentId;
    
    private ServerStatus _nodeServer;
    public ServerStatus NodeServer
    {
        get => _nodeServer;
        set => SetProperty(ref _nodeServer, value);
    }

    private ServerStatus _houstonServer;
    public ServerStatus HoustonServer
    {
        get => _houstonServer;
        set => SetProperty(ref _houstonServer, value);
    }

    public ObservableCollection<Core.Models.Node> Nodes { get; } = new();
    public ObservableCollection<Station> Stations { get; } = new();

    private Station _selectedStation;
    public Station SelectedStation
    {
        get => _selectedStation;
        set
        {
            SetProperty(ref _selectedStation, value);
            UpdateCurrentObservation();
            ApplyCommand.RaiseCanExecuteChanged();
        }
    }

    private string _currentObservation;
    public string CurrentObservation
    {
        get => _currentObservation;
        set => SetProperty(ref _currentObservation, value);
    }
    
    private TLE _tle;
    public TLE Tle
    {
        get => _tle;
        set
        {
            SetProperty(ref _tle, value);
            UpdateCurrentObservation();
            ApplyCommand.RaiseCanExecuteChanged();
        }
    }

    public AsyncDelegateCommand ApplyCommand { get; }

    public GTrackControlViewModel(
        ILogger<GTrackControlViewModel> logger, IGTrackNodeServer gTrackNodeServer,
        IEventAggregator eventAggregator, IDialogService dialogService)
    {
        _logger = logger;
        _eventAggregator = eventAggregator;
        _dialogService = dialogService;
        _gTrackNodeServer = gTrackNodeServer;

        NodeServer = new ServerStatus { Name = "Node", Status = false };
        HoustonServer = new ServerStatus { Name = "Houston", Status = false };

        ApplyCommand = new AsyncDelegateCommand(OnApply, CanApply);

        eventAggregator.GetEvent<HoustonServerStartedEvent>().Subscribe(() =>
        {
            HoustonServer.Status = true;
            RaisePropertyChanged(nameof(HoustonServer));
        });

        eventAggregator.GetEvent<HoustonServerStoppedEvent>().Subscribe(() =>
        {
            HoustonServer.Status = false;
            RaisePropertyChanged(nameof(HoustonServer));
        });

        eventAggregator.GetEvent<HoustonServerCrashedEvent>().Subscribe(ex =>
        {
            HoustonServer.Status = false;
            RaisePropertyChanged(nameof(HoustonServer));
        });
        
        eventAggregator.GetEvent<NodeServerStartedEvent>().Subscribe(() =>
        {
            NodeServer.Status = true;
            RaisePropertyChanged(nameof(NodeServer));
        });

        eventAggregator.GetEvent<NodeServerStoppedEvent>().Subscribe(() =>
        {
            NodeServer.Status = false;
            RaisePropertyChanged(nameof(NodeServer));
        });

        eventAggregator.GetEvent<NodeServerCrashedEvent>().Subscribe(ex =>
        {
            NodeServer.Status = false;
            RaisePropertyChanged(nameof(NodeServer));
        });

        eventAggregator.GetEvent<NodeServerNodeReceivedEvent>().Subscribe(node =>
        {
            var existingNode = Nodes.FirstOrDefault(n => n.Id == node.Id);
            if (existingNode != null)
            {
                var idx = Nodes.IndexOf(existingNode);
                Nodes[idx] = node;
            }
            else
            {
                Nodes.Add(node);
            }
            
            var nodeStationIds = node.Stations.Select(s => s.Id).ToHashSet();
            var stationsToRemove = Stations.Where(s => !nodeStationIds.Contains(s.Id)).ToList();
            foreach (var s in stationsToRemove)
                Stations.Remove(s);
            
            foreach (var station in node.Stations)
            {
                var existingStation = Stations.FirstOrDefault(s => s.Id == station.Id);
                if (existingStation == null)
                {
                    Stations.Add(station);
                }
                else
                {
                    var idx = Stations.IndexOf(existingStation);
                    Stations[idx] = station;
                }
            }
        });
        
        eventAggregator.GetEvent<NodeServerNodeDisconnectedEvent>().Subscribe(node =>
        {
            var existingNode = Nodes.FirstOrDefault(n => n.Id == node.Id);
            if (existingNode != null)
                Nodes.Remove(existingNode);

            foreach (var station in node.Stations)
            {
                var st = Stations.FirstOrDefault(s => s.Id == station.Id);
                if (st != null)
                    Stations.Remove(st);
            }
        });
        
        _eventAggregator.GetEvent<TleFilePathSelectedEvent>().Subscribe(tle =>
        {
            Tle = tle;
        });
        
        UpdateCurrentObservation();
    }

    private async Task OnApply()
    {
        if (SelectedStation != null && Tle != null)
        {
            CurrentObservation = $"Выбрана станция: {SelectedStation.Id}";

            try
            {
                await _gTrackNodeServer.SendTleAsync(SelectedStation, Tle);

                _stationsSent.Add(SelectedStation.Id);
                ApplyCommand.RaiseCanExecuteChanged();
                UpdateCurrentObservation();
            }
            catch (Exception ex)
            {
                CurrentObservation = $"Ошибка при отправке TLE: {ex.Message}";
                _logger.LogError(ex, "Ошибка при вызове SendTleAsync");
            }
        }
    }
    
    private bool CanApply()
    {
        return SelectedStation != null 
               && Tle != null 
               && !_stationsSent.Contains(SelectedStation.Id);
    }
    
    private void UpdateCurrentObservation()
    {
        if (SelectedStation == null)
            CurrentObservation = "Выберите станцию для отправки TLE";
        else if (Tle == null)
            CurrentObservation = $"Станция {SelectedStation.Id} выбрана, выберите TLE-файл";
        else if (_stationsSent.Contains(SelectedStation.Id))
            CurrentObservation = $"TLE уже отправлен на станцию {SelectedStation.Id}";
        else
            CurrentObservation = $"Выбрана станция: {SelectedStation.Id}, можно отправить TLE";
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