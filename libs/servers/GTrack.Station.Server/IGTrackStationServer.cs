using System.Net;

namespace GTrack.Station.Server;

/// <summary>
/// Defines the contract for a GTrack station server managing connections and messages.
/// / Определяет контракт для сервера станций GTrack, управляющего подключениями и сообщениями.
/// </summary>
public interface IGTrackStationServer
{
    /// <summary>
    /// Indicates whether the server is currently running.
    /// / Показывает, работает ли сервер в данный момент.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Event triggered when the server starts.
    /// / Событие, вызываемое при запуске сервера.
    /// </summary>
    event Action? OnStarted;

    /// <summary>
    /// Event triggered when the server stops.
    /// / Событие, вызываемое при остановке сервера.
    /// </summary>
    event Action? OnStopped;

    /// <summary>
    /// Event triggered when a message is received from a station.
    /// / Событие, вызываемое при получении сообщения от станции.
    /// </summary>
    event Action<Core.Models.Station>? OnMessageReceived;

    /// <summary>
    /// Event triggered when a station disconnects.
    /// / Событие, вызываемое при отключении станции.
    /// </summary>
    event Action<Core.Models.Station>? OnStationDisconnected;

    /// <summary>
    /// Starts the server listening on the specified IP and port with a node ID.
    /// / Запускает сервер, прослушивая указанный IP и порт с ID узла.
    /// </summary>
    Task StartAsync(IPAddress ipAddress, int port, string nodeId);

    /// <summary>
    /// Stops the server and disconnects all stations.
    /// / Останавливает сервер и отключает все станции.
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Sends a message to a specific station by ID.
    /// / Отправляет сообщение конкретной станции по её ID.
    /// </summary>
    Task SendToStationAsync(string stationId, string message);

    /// <summary>
    /// Broadcasts a message to all connected stations.
    /// / Рассылает сообщение всем подключённым станциям.
    /// </summary>
    Task BroadcastAsync(string message);
}