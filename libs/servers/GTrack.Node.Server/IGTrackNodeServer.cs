namespace GTrack.Node.Server;

/// <summary>
/// Defines the contract for a GTrack node server managing client connections and messaging.
/// </summary>
/// <summary>
/// Определяет контракт для сервера узлов GTrack, управляющего подключениями клиентов и обменом сообщениями.
/// </summary>
public interface IGTrackNodeServer
{
    /// <summary>
    /// Indicates whether the server is currently running.
    /// </summary>
    /// <summary>
    /// Показывает, работает ли сервер в данный момент.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Event triggered when the server starts.
    /// </summary>
    /// <summary>
    /// Событие, вызываемое при запуске сервера.
    /// </summary>
    event Action? OnStarted;

    /// <summary>
    /// Event triggered when the server stops.
    /// </summary>
    /// <summary>
    /// Событие, вызываемое при остановке сервера.
    /// </summary>
    event Action? OnStopped;

    /// <summary>
    /// Event triggered when a message is received from a node.
    /// </summary>
    /// <summary>
    /// Событие, вызываемое при получении сообщения от Node.
    /// </summary>
    event Action<Core.Models.Node>? OnMessageReceivedNode;

    /// <summary>
    /// Event triggered when a node disconnects.
    /// </summary>
    /// <summary>
    /// Событие, вызываемое при отключении Node.
    /// </summary>
    event Action<Core.Models.Node>? OnNodeDisconnected;

    /// <summary>
    /// Starts the server asynchronously, listening on the specified IP and port.
    /// </summary>
    /// <summary>
    /// Асинхронно запускает сервер, прослушивая указанный IP и порт.
    /// </summary>
    Task StartAsync(string ip, int port);

    /// <summary>
    /// Stops the server asynchronously and closes all client connections.
    /// </summary>
    /// <summary>
    /// Асинхронно останавливает сервер и закрывает все подключения клиентов.
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Sends TLE data to a specific station via its node.
    /// </summary>
    /// <summary>
    /// Отправляет данные TLE для конкретной станции через её Node.
    /// </summary>
    Task SendTleAsync(Core.Models.Station station, Core.Models.TLE tle);
}