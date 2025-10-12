namespace GTrack.Core.Models;

/// <summary>
/// Represents a single target tracking result with bindable string properties.
/// Stores time, name, azimuth, elevation, range, and Doppler values as strings for UI display.
/// </summary>
///
/// <summary>
/// Представляет результат отслеживания цели с биндуемыми строковыми свойствами.
/// Хранит время, имя, азимут, высоту, дальность и эффект Доплера в виде строк для отображения в UI.
/// </summary>
public class TargetTracking : BindableBase
{
    private string _time;
    public string Time { get => _time; set => SetProperty(ref _time, value); }

    private string _name;
    public string Name { get => _name; set => SetProperty(ref _name, value); }

    private string _azimuth;
    public string Azimuth { get => _azimuth; set => SetProperty(ref _azimuth, value); }

    private string _elevation;
    public string Elevation { get => _elevation; set => SetProperty(ref _elevation, value); }

    private string _range;
    public string Range { get => _range; set => SetProperty(ref _range, value); }

    private string _doppler;
    public string Doppler { get => _doppler; set => SetProperty(ref _doppler, value); }
}