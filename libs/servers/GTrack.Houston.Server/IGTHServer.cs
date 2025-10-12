namespace GTrack.Houston.Server;

/// <summary>
/// Defines the contract for a Houston server that manages TCP client connections.
/// </summary>
/// <summary>
/// Определяет контракт для сервера Houston, который управляет TCP подключениями клиентов.
/// </summary>
public interface IGTHServer
{
    /// <summary>
    /// Indicates whether the server is currently running.
    /// </summary>
    /// <summary>
    /// Показывает, запущен ли сервер в данный момент.
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
}