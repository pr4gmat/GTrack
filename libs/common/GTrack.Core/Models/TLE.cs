namespace GTrack.Core.Models;

/// <summary>
/// Represents a TLE (Two-Line Element) data set for a satellite.
/// Stores the satellite name and two TLE lines.
/// </summary>
///
/// <summary>
/// Представляет данные TLE (двухстрочный элемент) спутника.
/// Хранит имя спутника и две строки TLE.
/// </summary>
public class TLE
{
    public string Name { get; set; }
    public string Line1 { get; set; }
    public string Line2 { get; set; }
}