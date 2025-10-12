using System.Collections.ObjectModel;
using GTrack.Core.Events.NodeServer;
using GTrack.Core.Models;
using Microsoft.Extensions.Logging;

namespace GTrack.Control.Desktop.ViewModels.Data;

/// <summary>
/// ViewModel for displaying nodes and their stations in the UI.
/// / Модель представления для отображения нод и их станций в UI.
/// </summary>
public class StationInstancesViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<StationInstancesViewModel> _logger; // Logger / Логгер
    private readonly IEventAggregator _eventAggregator; // Event aggregator / Агрегатор событий

    /// <summary>
    /// Collection of nodes received from NodeServer.
    /// / Коллекция нод, полученных с NodeServer.
    /// </summary>
    public ObservableCollection<Core.Models.Node> Nodes { get; } = new();

    /// <summary>
    /// Collection of stations aggregated from all nodes.
    /// / Коллекция станций, агрегированных со всех нод.
    /// </summary>
    public ObservableCollection<Station> Stations { get; } = new();

    private Core.Models.Node _selectedStation;
    public Core.Models.Node SelectedStation
    {
        get => _selectedStation;
        set => SetProperty(ref _selectedStation, value);
    }

    public StationInstancesViewModel(ILogger<StationInstancesViewModel> logger,
                                     IEventAggregator eventAggregator)
    {
        _logger = logger;
        _eventAggregator = eventAggregator;

        // Subscribe to node updates / Подписка на обновления нод
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

            foreach (var station in node.Stations)
            {
                if (!Stations.Any(s => s.Id == station.Id))
                    Stations.Add(station);
            }
        });

        // Subscribe to node disconnections / Подписка на отключения нод
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
    }

    public void OnNavigatedTo(NavigationContext navigationContext) { }
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}