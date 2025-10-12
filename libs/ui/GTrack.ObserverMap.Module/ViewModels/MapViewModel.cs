using System.Collections.ObjectModel;
using GMap.NET;
using GTrack.Core.Events;
using GTrack.ObserverMap.Module.Services;

namespace GTrack.ObserverMap.Module.ViewModels;

/// <summary>
/// ViewModel for map control, manages center, zoom, and markers.
/// / Модель представления для карты, управляет центром, масштабом и маркерами.
/// </summary>
public class MapViewModel : BindableBase
{
    private readonly IMapCacheService _cacheService; // Map caching service / Сервис кеширования карты
    private readonly IEventAggregator _eventAggregator; // Event aggregator / Агрегатор событий

    private PointLatLng _center; // Current map center / Текущий центр карты
    public PointLatLng Center
    {
        get => _center;
        set => SetProperty(ref _center, value);
    }

    private double _zoom = 4; // Current zoom level / Текущий уровень масштабирования
    public double Zoom
    {
        get => _zoom;
        set => SetProperty(ref _zoom, value);
    }

    /// <summary>
    /// Collection of marker positions on the map.
    /// / Коллекция позиций маркеров на карте.
    /// </summary>
    public ObservableCollection<PointLatLng> Markers { get; } = new();

    public MapViewModel(IMapCacheService cacheService, IEventAggregator eventAggregator)
    {
        _cacheService = cacheService;
        _eventAggregator = eventAggregator;

        _cacheService.UseOnlineWithCache(); // Enable online mode with cache / Включить онлайн режим с кешем

        Zoom = 4; // Initial zoom / Начальный масштаб

        // Subscribe to location updates / Подписка на обновления локаций
        _eventAggregator.GetEvent<ObserverLocationsUpdatedEvent>()
            .Subscribe(UpdateMarkers);
    }

    /// <summary>
    /// Updates markers and recenters map based on new points.
    /// / Обновляет маркеры и центр карты на основе новых точек.
    /// </summary>
    /// <param name="points">List of points / Список точек</param>
    private void UpdateMarkers(IReadOnlyList<PointLatLng> points)
    {
        Markers.Clear();
        foreach (var p in points)
            Markers.Add(p);

        if (points.Count > 0)
        {
            var avgLat = points.Average(p => p.Lat);
            var avgLng = points.Average(p => p.Lng);
            Center = new PointLatLng(avgLat, avgLng);
        }
    }
}