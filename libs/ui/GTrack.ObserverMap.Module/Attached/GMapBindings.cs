using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using GTrack.Core.Models;

namespace GTrack.ObserverMap.Module.Attached;

/// <summary>
/// Attached properties for GMapControl: center, markers (observers), satellite markers.
/// / Присоединённые свойства для GMapControl: центр, маркеры наблюдателей и маркеры спутников.
/// </summary>
public static class GMapBindings
{
    // ----- Center -----
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

    // ----- Observer Markers -----
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
            oldCollection.CollectionChanged -= (_, __) => RefreshObserverMarkers(map);

        if (e.NewValue is ObservableCollection<PointLatLng> newCollection)
            newCollection.CollectionChanged += (_, __) => RefreshObserverMarkers(map);

        RefreshObserverMarkers(map);
    }

    private static void RefreshObserverMarkers(GMapControl map)
    {
        // Удаляем только красные маркеры (наблюдателей)
        var observerMarkers = map.Markers.Where(m => m.Tag == null).ToList();
        foreach (var m in observerMarkers)
            map.Markers.Remove(m);

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
                },
                Tag = null // красный маркер = наблюдатель
            };
            map.Markers.Add(marker);
        }
    }

    // ----- Satellite Markers -----
    public static readonly DependencyProperty SatelliteMarkersProperty =
        DependencyProperty.RegisterAttached(
            "SatelliteMarkers",
            typeof(ObservableCollection<SatelliteObservationResult>),
            typeof(GMapBindings),
            new PropertyMetadata(null, OnSatelliteMarkersChanged));

    public static void SetSatelliteMarkers(DependencyObject element, ObservableCollection<SatelliteObservationResult> value) =>
        element.SetValue(SatelliteMarkersProperty, value);

    public static ObservableCollection<SatelliteObservationResult> GetSatelliteMarkers(DependencyObject element) =>
        (ObservableCollection<SatelliteObservationResult>)element.GetValue(SatelliteMarkersProperty);

    private static void OnSatelliteMarkersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not GMapControl map) return;

        if (e.OldValue is ObservableCollection<SatelliteObservationResult> oldCollection)
            oldCollection.CollectionChanged -= (_, __) => RefreshSatelliteMarkers(map);

        if (e.NewValue is ObservableCollection<SatelliteObservationResult> newCollection)
            newCollection.CollectionChanged += (_, __) => RefreshSatelliteMarkers(map);

        RefreshSatelliteMarkers(map);
    }

    private static void RefreshSatelliteMarkers(GMapControl map)
    {
        // Удаляем только синие маркеры спутников
        var satelliteMarkers = map.Markers.Where(m => m.Tag is SatelliteObservationResult).ToList();
        foreach (var m in satelliteMarkers)
            map.Markers.Remove(m);

        var markers = GetSatelliteMarkers(map);
        if (markers == null) return;

        foreach (var sat in markers)
        {
            var marker = new GMapMarker(new PointLatLng(sat.Latitude, sat.Longitude))
            {
                Shape = new System.Windows.Shapes.Ellipse
                {
                    Width = 14,
                    Height = 14,
                    Stroke = System.Windows.Media.Brushes.Blue,
                    StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Blue
                },
                Tag = sat // чтобы понимать, что это спутник
            };
            map.Markers.Add(marker);
        }
    }
}