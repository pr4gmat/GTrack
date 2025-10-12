namespace GTrack.Station.Client;

/// <summary>
/// Interface defining a client for connecting to the GTrack Station server.
/// Handles connection state and message sending/receiving.
/// </summary>
/// <summary>
/// Интерфейс, определяющий клиент для подключения к серверу GTrack Station.
/// Отвечает за состояние соединения и отправку/приём сообщений.
/// </summary>
public interface IGTrackStationClient
{
    /// <summary>
    /// Indicates whether the client is currently connected to the server.
    /// </summary>
    /// <summary>
    /// Показывает, подключён ли клиент к серверу в данный момент.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Unique identifier of the station assigned by the server.
    /// </summary>
    /// <summary>
    /// Уникальный идентификатор станции, назначенный сервером.
    /// </summary>
    string? StationId { get; }

    /// <summary>
    /// Starts the client and connects to the specified Station server.
    /// </summary>
    /// <summary>
    /// Запускает клиент и подключается к указанному серверу Station.
    /// </summary>
    Task StartAsync(string serverIp, int serverPort);

    /// <summary>
    /// Sends a message to the connected server.
    /// </summary>
    /// <summary>
    /// Отправляет сообщение на подключённый сервер.
    /// </summary>
    Task SendMessageAsync(string message);

    /// <summary>
    /// Stops the client and closes the connection.
    /// </summary>
    /// <summary>
    /// Останавливает клиент и закрывает соединение.
    /// </summary>
    Task Stop();
}