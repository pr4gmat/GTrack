namespace GTrack.Core.Models;

/// <summary>
/// Represents a Station with connection details and error information.
/// </summary>
///
/// <summary>
/// Представляет станцию (Station) с деталями подключения и информацией об ошибках.
/// </summary>
public class Station
{
    public string Id { get; set; }
    public bool IsConnected { get; set; } = false;
    public string IP { get; set; }
    public int Port { get; set; }
    public DateTime LastConnectedTime { get; set; }
    public string LastError { get; set; }
}