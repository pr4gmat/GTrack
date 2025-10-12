using System.Net.Sockets;
using System.Text;
using GTrack.Core.Models;
using Microsoft.Extensions.Logging;

namespace GTrack.Node.Client;

/// <summary>
/// Implementation of the GTrack Node client responsible for connecting to the Node server,
/// handling communication, and managing received messages.
/// </summary>
/// <summary>
/// Реализация клиента GTrack Node, отвечающего за подключение к серверу Node,
/// обработку сообщений и управление полученными данными.
/// </summary>
public class GTrackNodeClient : IGTrackNodeClient
{
    private readonly ILogger<GTrackNodeClient> _logger;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private Task? _listenTask;
    private bool _running;

    public bool IsConnected => _client?.Connected ?? false;

    public List<Station> Stations { get; } = new();
    public string? NodeId { get; private set; }

    public event Action? OnConnected;
    public event Action? OnDisconnected;
    public event Action<string>? OnMessageReceived;
    public event Action<string>? OnNodeIdReceived;
    public event Action<Station, TLE>? OnTleReceived;

    public GTrackNodeClient(ILogger<GTrackNodeClient> logger)
    {
        _logger = logger;
    }

    public async Task StartAsync(string serverIp, int serverPort)
    {
        _client = new TcpClient();

        try
        {
            await _client.ConnectAsync(serverIp, serverPort);
            _stream = _client.GetStream();
            _running = true;

            _logger.LogInformation("[Node client] Connected to {Ip}:{Port}", serverIp, serverPort);
            OnConnected?.Invoke();

            await SendMessageAsync("MYIDNODE");

            _listenTask = Task.Run(ListenAsync);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Node client] Connection error to {Ip}:{Port}", serverIp, serverPort);
            Disconnect();
        }
    }

    private async Task ListenAsync()
    {
        if (_stream == null) return;

        var buffer = new byte[4096];

        try
        {
            while (_running && _client?.Connected == true)
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                OnMessageReceived?.Invoke(message);

                _logger.LogInformation("[Node client] Received: {Message}", message);

                await HandleMessageAsync(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Node client] ListenAsync error");
        }
        finally
        {
            Disconnect();
            OnDisconnected?.Invoke();
            _logger.LogWarning("[Node client] Connection closed");
        }
    }

    private async Task HandleMessageAsync(string message)
    {
        switch (message)
        {
            case var msg when msg.StartsWith("YOURIDNODE:"):
                NodeId = msg.Substring("YOURIDNODE:".Length);
                OnNodeIdReceived?.Invoke(NodeId);
                _logger.LogInformation("[Node client] Assigned NodeId: {NodeId}", NodeId);
                break;

            case "GETSTATIONS":
                if (Stations.Count > 0)
                {
                    foreach (var station in Stations)
                    {
                        await SendMessageAsync($"STATIONS:{station.Id}");
                        await Task.Delay(10);
                    }
                }
                break;

            case var msg when msg.StartsWith("SENDTLETOSTATION:"):
                string[] parts = msg.Substring("SENDTLETOSTATION:".Length).Split('|');
                if (parts.Length == 4)
                {
                    string stationId = parts[0];
                    string tleName = parts[1];
                    string tleLine1 = parts[2];
                    string tleLine2 = parts[3];

                    if (!string.IsNullOrEmpty(NodeId) && stationId.StartsWith(NodeId))
                    {
                        var station = new Station { Id = stationId };
                        var tle = new TLE { Name = tleName, Line1 = tleLine1, Line2 = tleLine2 };

                        OnTleReceived?.Invoke(station, tle);
                        _logger.LogInformation("[Node client] Received TLE for station {StationId}: {Name}", stationId, tleName);
                    }
                }
                break;
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (_stream == null)
            throw new InvalidOperationException("Client is not connected");

        var data = Encoding.UTF8.GetBytes(message);
        await _stream.WriteAsync(data, 0, data.Length);
        await _stream.FlushAsync();

        _logger.LogInformation("[Node client] Sent: {Message}", message);
    }

    public async Task Stop()
    {
        Disconnect();
    }

    private void Disconnect()
    {
        _running = false;

        try
        {
            _stream?.Dispose();
            _client?.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Node client] Error during disconnect");
        }
        finally
        {
            _client = null;
            _stream = null;
        }
    }
}