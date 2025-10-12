using System.Collections.ObjectModel;
using GMap.NET;
using GTrack.Core.Events;
using GTrack.Core.Models;
using GTrack.Node.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace GTrack.Node.Desktop.ViewModels;

/// <summary>
/// ViewModel for managing observer locations on the map.
/// ViewModel для управления наблюдательными точками на карте.
/// </summary>
public class ObserverMapViewModel : BindableBase, INavigationAware
{
    private readonly ILogger<ObserverMapViewModel> _logger;
    private readonly IRegionManager _regionManager;
    private readonly IDialogService _dialogService;
    private readonly IEventAggregator _eventAggregator;

    /// <summary>
    /// Collection of observer locations.
    /// Коллекция точек наблюдения.
    /// </summary>
    private ObservableCollection<ObserverLocation> _observerLocations = new();
    public ObservableCollection<ObserverLocation> ObserverLocations
    {
        get => _observerLocations;
        set => SetProperty(ref _observerLocations, value);
    }
    
    /// <summary>
    /// Currently selected observer location.
    /// Текущая выбранная точка наблюдения.
    /// </summary>
    private ObserverLocation _selectedObserverLocation;
    public ObserverLocation SelectedObserverLocation
    {
        get => _selectedObserverLocation;
        set => SetProperty(ref _selectedObserverLocation, value);
    }
    
    /// <summary>
    /// Commands for adding, saving, and deleting observer locations.
    /// Команды для добавления, сохранения и удаления точек наблюдения.
    /// </summary>
    public DelegateCommand AddObserverLocationCommand { get; }
    public DelegateCommand SaveObserverLocationCommand { get; }
    public DelegateCommand DelObserverLocationCommand { get; }
    
    public ObserverMapViewModel(ILogger<ObserverMapViewModel> logger, IRegionManager regionManager,
        IDialogService dialogService, IEventAggregator eventAggregator)
    {
        _logger = logger;   
        _regionManager = regionManager;
        _dialogService = dialogService;
        _eventAggregator = eventAggregator;
        
        // Initialize commands and bind their CanExecute conditions
        // Инициализация команд и привязка условий CanExecute
        AddObserverLocationCommand = new DelegateCommand(AddObserverLocation, CanAddObserverLocation)
            .ObservesProperty(() => ObserverLocations.Count);
        DelObserverLocationCommand = new DelegateCommand(DeleteObserverLocation, CanSaveOrDelete)
            .ObservesProperty(() => SelectedObserverLocation);
    }

    /// <summary>
    /// Opens a dialog to add a new observer location.
    /// Открывает диалог для добавления новой точки наблюдения.
    /// </summary>
    private void AddObserverLocation()
    {
        _dialogService.ShowDialog(nameof(ObserverAddView), null, r =>
        {
            if (r.Result == ButtonResult.OK && r.Parameters.ContainsKey("observer"))
            {
                var newObs = r.Parameters.GetValue<ObserverLocation>("observer");
                ObserverLocations.Add(newObs);
                SelectedObserverLocation = newObs;
                
                // Update map after adding new location
                // Обновление карты после добавления новой точки
                PublishToMap();
                
                _eventAggregator.GetEvent<ObserverLocationAddedEvent>().Publish(newObs);
            }
        });
    }
    
    /// <summary>
    /// Determines if a new observer location can be added (limit: 1).
    /// Проверяет, можно ли добавить новую точку наблюдения (лимит: 1).
    /// </summary>
    private bool CanAddObserverLocation()
    {
        return ObserverLocations.Count < 1;
    }

    /// <summary>
    /// Deletes the selected observer location.
    /// Удаляет выбранную точку наблюдения.
    /// </summary>
    private void DeleteObserverLocation()
    {
        if (SelectedObserverLocation != null)
        {
            var name = SelectedObserverLocation.Name;
            ObserverLocations.Remove(SelectedObserverLocation);
            SelectedObserverLocation = null;
            
            // Update map after deletion
            // Обновление карты после удаления точки
            PublishToMap();
        }
    }

    /// <summary>
    /// Determines if a location can be saved or deleted.
    /// Проверяет, можно ли сохранить или удалить точку.
    /// </summary>
    private bool CanSaveOrDelete() => SelectedObserverLocation != null;
    
    /// <summary>
    /// Publishes the observer locations as map points to the event aggregator.
    /// Публикует точки наблюдения на карту через EventAggregator.
    /// </summary>
    private void PublishToMap()
    {
        var points = ObserverLocations
            .Select(o => new PointLatLng(o.Latitude, o.Longitude))
            .ToList();

        _eventAggregator.GetEvent<ObserverLocationsUpdatedEvent>().Publish(points);
    }
    
    /// <summary>
    /// Navigates to the MapView when this ViewModel is navigated to.
    /// Навигация к MapView при переходе на этот ViewModel.
    /// </summary>
    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        _regionManager.RequestNavigate("ObserverMapRegion", "MapView");
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}