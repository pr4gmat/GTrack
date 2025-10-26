namespace GTrack.Core.Models;

/// <summary>
/// Represents a single satellite observation result.
/// Contains information such as time, azimuth, elevation, range, and Doppler effect.
/// </summary>
///
/// <summary>
/// Представляет результат наблюдения спутника.
/// Содержит данные: время, азимут, высоту, дальность и эффект Доплера.
/// </summary>
public class SatelliteObservationResult
{
    public string Name { get; set; }
    public DateTime Time { get; set; }
    public double Azimuth { get; set; }
    public double Elevation { get; set; }
    public double Range { get; set; }
    public double Doppler { get; set; }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }
}