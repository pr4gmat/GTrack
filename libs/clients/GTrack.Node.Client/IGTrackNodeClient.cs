using GTrack.Core.Models;

namespace GTrack.Node.Client;

/// <summary>
/// Interface defining the client that connects to the GTrack Node server.
/// Handles connection state, message exchange, and received data.
/// </summary>
/// <summary>
/// Интерфейс, определяющий клиент для подключения к серверу GTrack Node.
/// Отвечает за состояние соединения, обмен сообщениями и получение данных.
/// </summary>
public interface IGTrackNodeClient
{
    /// <summary>
    /// Indicates whether the client is currently connected to the server.
    /// </summary>
    /// <summary>
    /// Показывает, подключён ли клиент к серверу в данный момент.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Collection of stations associated with this node client.
    /// </summary>
    /// <summary>
    /// Коллекция станций, связанных с этим узлом-клиентом.
    /// </summary>
    List<Station> Stations { get; }

    /// <summary>
    /// Unique identifier of the node assigned by the server.
    /// </summary>
    /// <summary>
    /// Уникальный идентификатор узла, назначенный сервером.
    /// </summary>
    string? NodeId { get; }

    /// <summary>
    /// Triggered when the client successfully connects to the server.
    /// </summary>
    /// <summary>
    /// Вызывается при успешном подключении клиента к серверу.
    /// </summary>
    event Action? OnConnected;

    /// <summary>
    /// Triggered when the client disconnects from the server.
    /// </summary>
    /// <summary>
    /// Вызывается при отключении клиента от сервера.
    /// </summary>
    event Action? OnDisconnected;

    /// <summary>
    /// Triggered when a message is received from the server.
    /// </summary>
    /// <summary>
    /// Вызывается при получении сообщения от сервера.
    /// </summary>
    event Action<string>? OnMessageReceived;

    /// <summary>
    /// Triggered when the server assigns an ID to the node.
    /// </summary>
    /// <summary>
    /// Вызывается, когда сервер назначает идентификатор узлу.
    /// </summary>
    event Action<string>? OnNodeIdReceived;

    /// <summary>
    /// Triggered when TLE data is received for a specific station.
    /// </summary>
    /// <summary>
    /// Вызывается при получении данных TLE для конкретной станции.
    /// </summary>
    event Action<Station, TLE>? OnTleReceived;

    /// <summary>
    /// Starts the client and connects to the specified Node server.
    /// </summary>
    /// <summary>
    /// Запускает клиент и подключается к указанному серверу Node.
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