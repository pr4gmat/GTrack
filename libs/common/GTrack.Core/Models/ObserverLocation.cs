namespace GTrack.Core.Models;

/// <summary>
/// Represents an observer's location with bindable properties.
/// Supports data binding for UI updates.
/// </summary>
///
/// <summary>
/// Представляет местоположение наблюдателя с поддержкой привязки данных.
/// Поддерживает обновления UI через биндинги.
/// </summary>
public class ObserverLocation : BindableBase
{
    private string _name;
    public string Name { get => _name; set => SetProperty(ref _name, value); }

    private double _latitude;
    public double Latitude { get => _latitude; set => SetProperty(ref _latitude, value); }

    private double _longitude;
    public double Longitude { get => _longitude; set => SetProperty(ref _longitude, value); }

    private double _height;
    public double Height { get => _height; set => SetProperty(ref _height, value); }
}