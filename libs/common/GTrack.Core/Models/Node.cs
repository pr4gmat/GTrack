namespace GTrack.Core.Models;

/// <summary>
/// Represents a Node connected to the server.
/// Includes connection details, list of associated stations, and error information.
/// </summary>
///
/// <summary>
/// Представляет узел (Node), подключенный к серверу.
/// Включает детали подключения, список связанных станций и информацию об ошибках.
/// </summary>
public class Node
{
    public string Id { get; set; }
    public bool IsConnected { get; set; }
    public string IP { get; set; }
    public int Port { get; set; }
    public List<Station> Stations { get; set; }
    public DateTime LastConnectedTime { get; set; }
    public string LastError { get; set; }
}