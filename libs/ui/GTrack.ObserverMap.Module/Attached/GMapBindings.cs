using System.Collections.ObjectModel;
using System.Windows;
using GMap.NET;
using GMap.NET.WindowsPresentation;

namespace GTrack.ObserverMap.Module.Attached;

/// <summary>
/// Attached properties for GMapControl: center and markers.
/// / Присоединённые свойства для GMapControl: центр и маркеры.
/// </summary>
public static class GMapBindings
{
    // ----- Center -----
    /// <summary>
    /// Attached property for map center.
    /// / Присоединённое свойство для центра карты.
    /// </summary>
    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.RegisterAttached(
            "Center",
            typeof(PointLatLng?),
            typeof(GMapBindings),
            new PropertyMetadata(null, OnCenterChanged));

    public static void SetCenter(DependencyObject element, PointLatLng? value) =>
        element.SetValue(CenterProperty, value);

    public static PointLatLng? GetCenter(DependencyObject element) =>
        (PointLatLng?)element.GetValue(CenterProperty);

    private static void OnCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not GMapControl map) return;

        if (e.NewValue is PointLatLng newCenter)
            map.Position = newCenter;
    }

    // ----- Markers -----
    /// <summary>
    /// Attached property for map markers collection.
    /// / Присоединённое свойство для коллекции маркеров карты.
    /// </summary>
    public static readonly DependencyProperty MarkersProperty =
        DependencyProperty.RegisterAttached(
            "Markers",
            typeof(ObservableCollection<PointLatLng>),
            typeof(GMapBindings),
            new PropertyMetadata(null, OnMarkersChanged));

    public static void SetMarkers(DependencyObject element, ObservableCollection<PointLatLng> value) =>
        element.SetValue(MarkersProperty, value);

    public static ObservableCollection<PointLatLng> GetMarkers(DependencyObject element) =>
        (ObservableCollection<PointLatLng>)element.GetValue(MarkersProperty);

    private static void OnMarkersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not GMapControl map) return;

        if (e.OldValue is ObservableCollection<PointLatLng> oldCollection)
            oldCollection.CollectionChanged -= (_, __) => RefreshMarkers(map);

        if (e.NewValue is ObservableCollection<PointLatLng> newCollection)
            newCollection.CollectionChanged += (_, __) => RefreshMarkers(map);

        RefreshMarkers(map);
    }

    /// <summary>
    /// Refreshes markers on the map based on the attached collection.
    /// / Обновляет маркеры на карте на основе присоединённой коллекции.
    /// </summary>
    /// <param name="map">GMapControl instance / Экземпляр GMapControl</param>
    private static void RefreshMarkers(GMapControl map)
    {
        map.Markers.Clear();
        var markers = GetMarkers(map);
        if (markers == null) return;

        foreach (var point in markers)
        {
            var marker = new GMapMarker(point)
            {
                Shape = new System.Windows.Shapes.Ellipse
                {
                    Width = 14,
                    Height = 14,
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Red
                }
            };
            map.Markers.Add(marker);
        }
    }
}
