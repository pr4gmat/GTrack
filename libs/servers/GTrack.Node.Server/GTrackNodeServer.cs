using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GTrack.Core.Models;
using Microsoft.Extensions.Logging;

namespace GTrack.Node.Server;

/// <summary>
/// TCP server for GTrack nodes managing client connections and messaging.
/// / TCP сервер для узлов GTrack, управляющий подключениями клиентов и обменом сообщениями.
/// </summary>
public class GTrackNodeServer : IGTrackNodeServer
{
    private readonly ILogger<GTrackNodeServer> _logger; // Logger for events/errors / Логгер для событий/ошибок
    private readonly ConcurrentDictionary<TcpClient, string> _clients = new(); // Connected clients / Подключённые клиенты
    private readonly ConcurrentDictionary<string, Core.Models.Node> _nodes = new(); // Nodes info / Информация о Node

    private TcpListener? _listener; // TCP listener / TCP слушатель
    private bool _isRunning; // Server running flag / Флаг работы сервера

    public bool IsRunning => _isRunning; // Server running state / Состояние работы сервера

    public event Action? OnStarted; // Triggered on server start / Вызывается при запуске
    public event Action? OnStopped; // Triggered on server stop / Вызывается при остановке
    public event Action<Core.Models.Node>? OnMessageReceivedNode; // Node message received / Сообщение от Node
    public event Action<Core.Models.Node>? OnNodeDisconnected; // Node disconnected / Node отключен

    public GTrackNodeServer(ILogger<GTrackNodeServer> logger)
    {
        _logger = logger; // Assign logger / Присваиваем логгер
    }

    /// <summary>
    /// Starts the server and begins accepting client connections asynchronously.
    /// / Запускает сервер и начинает принимать подключения клиентов асинхронно.
    /// </summary>
    public async Task StartAsync(string ip, int port)
    {
        _listener = new TcpListener(IPAddress.Parse(ip), port);
        _listener.Start();
        _isRunning = true;

        OnStarted?.Invoke();
        _logger.LogInformation("[Node server] Запуск на {Ip}:{Port}", ip, port);

        // Periodic "GETSTATIONS" message to all clients / Периодическая рассылка "GETSTATIONS" всем клиентам
        _ = Task.Run(async () =>
        {
            while (_isRunning)
            {
                try
                {
                    foreach (var kvp in _clients.ToList())
                    {
                        var client = kvp.Key;
                        if (client.Connected)
                        {
                            var stream = client.GetStream();
                            await SendMessageAsync(stream, "GETSTATIONS");
                            _logger.LogInformation("[Node server] Периодически отправлено 'GS' клиенту {Client}", kvp.Value);
                        }
                    }
                    await Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Node server] Ошибка в периодической рассылке 'GS'");
                }
            }
        });

        // Accept incoming clients / Принятие подключений клиентов
        try
        {
            while (_isRunning)
            {
                TcpClient client;
                try
                {
                    client = await _listener.AcceptTcpClientAsync();
                }
                catch (ObjectDisposedException)
                {
                    break;
                }

                var ep = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
                _clients.TryAdd(client, ep);

                _logger.LogInformation("[Node server] Клиент подключен: {Client}", ep);

                var node = new Core.Models.Node
                {
                    Id = ep,
                    IP = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString(),
                    Port = ((IPEndPoint)client.Client.RemoteEndPoint!).Port,
                    IsConnected = true,
                    LastConnectedTime = DateTime.UtcNow,
                    Stations = new List<Station>()
                };

                _nodes.AddOrUpdate(ep, node, (_, __) => node);
                OnMessageReceivedNode?.Invoke(node);

                _ = HandleClientAsync(client, node);
            }
        }
        finally
        {
            OnStopped?.Invoke();
            _logger.LogInformation("[Node server] Сервер остановлен");
        }
    }

    /// <summary>
    /// Stops the server and closes all active client connections.
    /// / Останавливает сервер и закрывает все подключения клиентов.
    /// </summary>
    public async Task StopAsync()
    {
        _isRunning = false;
        _listener?.Stop();

        foreach (var client in _clients.Keys.ToList())
        {
            client.Close();
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Handles communication with a connected node asynchronously.
    /// / Асинхронно обрабатывает связь с подключённым Node.
    /// </summary>
    private async Task HandleClientAsync(TcpClient client, Core.Models.Node node)
    {
        var remote = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
        using var stream = client.GetStream();
        byte[] buffer = new byte[4096];

        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                _logger.LogInformation("[Node server] Прием от {Remote}: {Message}", remote, message);

                switch (message)
                {
                    case "MYIDNODE":
                        string id = GenerateNodeId();
                        node.Id = id;
                        string nodeIdMsg = $"YOURIDNODE:{id}";
                        await SendMessageAsync(stream, nodeIdMsg);
                        break;

                    case var msg when msg.StartsWith("STATIONS:"):
                        string stationId = msg.Substring("STATIONS:".Length);
                        if (node.Stations.All(s => s.Id != stationId))
                        {
                            node.Stations.Add(new Station { Id = stationId });
                        }
                        break;
                }

                node.LastConnectedTime = DateTime.UtcNow;
                _nodes[remote] = node;
                OnMessageReceivedNode?.Invoke(node);
            }
        }
        catch (Exception ex)
        {
            node.IsConnected = false;
            node.LastError = ex.Message;
            OnMessageReceivedNode?.Invoke(node);
            _logger.LogError(ex, "[Node server] {Remote}", remote);
        }
        finally
        {
            client.Close();
            _clients.TryRemove(client, out _);
            _nodes.TryRemove(remote, out _);
            OnMessageReceivedNode?.Invoke(node);
            OnNodeDisconnected?.Invoke(node);
            _logger.LogWarning("[Node server] Клиент отключен и Node удален: {Remote}", remote);
        }
    }

    /// <summary>
    /// Sends a UTF-8 message to a client stream.
    /// / Отправляет UTF-8 сообщение на клиентский поток.
    /// </summary>
    private async Task SendMessageAsync(NetworkStream stream, string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
        await stream.FlushAsync();

        _logger.LogInformation("[Node server] Отправил: {Message}", message);
    }

    /// <summary>
    /// Sends TLE data to a specific station through its node.
    /// / Отправляет TLE для конкретной станции через её Node.
    /// </summary>
    public async Task SendTleAsync(Core.Models.Station station, Core.Models.TLE tle)
    {
        if (station == null) throw new ArgumentNullException(nameof(station));
        if (tle == null) throw new ArgumentNullException(nameof(tle));

        string message = $"SENDTLETOSTATION:{station.Id}|{tle.Name}|{tle.Line1}|{tle.Line2}";

        try
        {
            var clientKvp = _clients.FirstOrDefault(c =>
            {
                if (_nodes.TryGetValue(c.Value, out var node))
                {
                    return node.Stations.Any(s => s.Id == station.Id);
                }
                return false;
            });

            if (clientKvp.Key == null || !clientKvp.Key.Connected)
            {
                _logger.LogWarning("[Node server] Не удалось найти активного клиента для станции {StationId}", station.Id);
                return;
            }

            var stream = clientKvp.Key.GetStream();
            await SendMessageAsync(stream, message);

            _logger.LogInformation("[Node server] Отправлено TLE для станции {StationId}: {Message}", station.Id, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Node server] Ошибка при отправке TLE для станции {StationId}", station.Id);
            throw;
        }
    }

    /// <summary>
    /// Generates a random 6-character Node ID.
    /// / Генерирует случайный 6-символьный ID для Node.
    /// </summary>
    private string GenerateNodeId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var id = new char[6];
        for (int i = 0; i < 6; i++)
            id[i] = chars[random.Next(chars.Length)];
        return new string(id);
    }
}
