using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GTrack.Station.Server;

/// <summary>
/// TCP server for GTrack stations managing client connections.
/// / TCP сервер для станций GTrack, управляющий подключениями клиентов.
/// </summary>
public class GTrackStationServer : IGTrackStationServer
{
    private readonly ILogger<GTrackStationServer> _logger; // Logger for events/errors / Логгер для событий/ошибок
    private TcpListener? _listener; // TCP listener / TCP слушатель
    private bool _isRunning; // Server running flag / Флаг работы сервера

    private readonly ConcurrentDictionary<string, (Core.Models.Station Station, TcpClient Client)> _stations = new(); // Connected stations / Подключённые станции
    private readonly object _lock = new(); // Lock for ID generation / Лок для генерации ID
    private int _stationCounter = 0; // Counter for station IDs / Счётчик станций

    private string _nodeId = string.Empty; // Node ID / ID узла

    public bool IsRunning => _isRunning; // Server running state / Состояние работы сервера

    public event Action? OnStarted; // Triggered on server start / Вызывается при запуске
    public event Action? OnStopped; // Triggered on server stop / Вызывается при остановке
    public event Action<Core.Models.Station>? OnMessageReceived; // Station message received / Сообщение от станции
    public event Action<Core.Models.Station>? OnStationDisconnected; // Station disconnected / Станция отключена

    public GTrackStationServer(ILogger<GTrackStationServer> logger)
    {
        _logger = logger; // Assign logger / Присваиваем логгер
    }

    public async Task StartAsync(IPAddress ipAddress, int port, string nodeId)
    {
        if (_isRunning) return;

        _nodeId = nodeId;
        _listener = new TcpListener(ipAddress, port);
        _listener.Start();
        _isRunning = true;

        _logger.LogInformation("[Station server] Запущен на {Endpoint}", _listener.LocalEndpoint);
        OnStarted?.Invoke();

        try
        {
            while (_isRunning)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _logger.LogInformation("[Station server] Станция подключена: {Client}", client.Client.RemoteEndPoint);
                _ = HandleClientAsync(client);
            }
        }
        catch (ObjectDisposedException) { }
        finally
        {
            _logger.LogInformation("[Station server] Остановлен");
            OnStopped?.Invoke();
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[1024];

            var ep = client.Client.RemoteEndPoint as IPEndPoint;
            var station = new Core.Models.Station
            {
                Id = GenerateStationId(),
                IP = ep?.Address.ToString() ?? "unknown",
                Port = ep?.Port ?? 0
            };

            _stations[station.Id] = (station, client);

            try
            {
                while (_isRunning && client.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    _logger.LogInformation("[Station server] Получено: {Message}", message);

                    if (message == "MYIDSTATION")
                    {
                        string stationIdMsg = $"YOURIDSTATION:{station.Id}";
                        OnMessageReceived?.Invoke(station);
                        await SendMessageAsync(stream, stationIdMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Station server] Ошибка соединения");
            }
            finally
            {
                _stations.TryRemove(station.Id, out _);
                _logger.LogWarning("[Station server] Станция отключена: {Client}", client.Client.RemoteEndPoint);

                OnStationDisconnected?.Invoke(station);
            }
        }
    }

    private async Task SendMessageAsync(NetworkStream stream, string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
        await stream.FlushAsync();

        _logger.LogInformation("[Station server] Отправлено: {Message}", message);
    }

    public Task StopAsync()
    {
        if (!_isRunning) return Task.CompletedTask;

        _isRunning = false;
        _listener?.Stop();
        _listener = null;

        return Task.CompletedTask;
    }

    public async Task SendToStationAsync(string stationId, string message)
    {
        if (_stations.TryGetValue(stationId, out var entry))
        {
            if (entry.Client.Connected)
            {
                await SendMessageAsync(entry.Client.GetStream(), message);
            }
        }
    }

    public async Task BroadcastAsync(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        var tasks = _stations.Values.Select(async entry =>
        {
            try
            {
                await SendMessageAsync(entry.Client.GetStream(), message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[Station server] Не удалось отправить сообщение клиенту {StationId}: {Error}",
                    entry.Station.Id, ex.Message);

                _stations.TryRemove(entry.Station.Id, out _);
            }
        });

        await Task.WhenAll(tasks);
    }

    private string GenerateStationId()
    {
        lock (_lock)
        {
            var usedNumbers = _stations.Keys
                .Select(id =>
                {
                    var parts = id.Split('_');
                    return int.TryParse(parts.Last(), out var n) ? n : 0;
                })
                .Where(n => n > 0)
                .ToHashSet();

            int number = 1;
            while (usedNumbers.Contains(number))
                number++;

            return $"{_nodeId}_{number}";
        }
    }
}
