namespace GTrack.Station.Client;

/// <summary>
/// Represents NSRD telemetry data from a station.
/// </summary>
/// <summary>
/// Представляет телеметрические данные NSRD станции.
/// </summary>
public record NsrdData(
    string StationId,
    TimeSpan TimeUtc,
    string Name,
    double Azimuth,
    double Elevation,
    double Range,
    double Doppler
)
{
    /// <summary>
    /// Tries to parse a raw NSRD message into an NsrdData object.
    /// </summary>
    /// <summary>
    /// Пытается распарсить сырое сообщение NSRD в объект NsrdData.
    /// </summary>
    public static NsrdData? TryParse(string message)
    {
        if (!message.StartsWith("NSRD:")) return null;
        string content = message.Substring("NSRD:".Length);

        var parts = content.Split('|');
        if (parts.Length != 7) return null;

        string stationId = parts[0];
        if (!TimeSpan.TryParse(parts[1].Replace(" UTC", ""), out var timeUtc)) return null;
        string name = parts[2];

        if (!double.TryParse(parts[3].TrimEnd('°'), out var azimuth)) return null;
        if (!double.TryParse(parts[4].TrimEnd('°'), out var elevation)) return null;
        if (!double.TryParse(parts[5].Replace(" km", ""), out var range)) return null;
        if (!double.TryParse(parts[6].Replace(" Hz", ""), out var doppler)) return null;

        return new NsrdData(stationId, timeUtc, name, azimuth, elevation, range, doppler);
    }
}