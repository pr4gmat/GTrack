using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GTrack.Core.Models;
using Microsoft.Extensions.Logging;

namespace GTrack.Houston.Server;

/// <summary>
/// Represents the Houston server which manages TCP client connections.
/// Handles client registration, messaging, and KISS frame encoding/decoding.
/// </summary>
/// <summary>
/// Представляет сервер Houston, который управляет TCP подключениями.
/// Обрабатывает регистрацию клиентов, обмен сообщениями и кодирование/декодирование KISS фреймов.
/// </summary>м
public class GTHServer : IGTHServer
{
    private readonly ILogger<GTHServer> _logger;
    private readonly ConcurrentDictionary<string, HoustonApp> _clientsInfo = new();
    private readonly ConcurrentBag<TcpClient> _clients = new();

    private int _port;
    private IPAddress _ipAddress = IPAddress.Any;
    private TcpListener? _listener;
    private bool _isRunning;

    public bool IsRunning => _isRunning;
    
    public event Action? OnStarted;
    public event Action? OnStopped;

    private readonly byte[] srvHelloMsg =
    {
        0x5e, 0xba, 0x01, 0x20, 0x00, 0x00, 0x4c, 0x00, 0x00, 0x00,
        0x64, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0xc6, 0x59, 0xf3, 0x06, 0x98, 0x01, 0x00, 0x00,
        0x55, 0x00, 0x6e, 0x00, 0x69, 0x00, 0x63, 0x00, 0x61, 0x00,
        0x6e, 0x00, 0x20, 0x00, 0x64, 0x00, 0x65, 0x00, 0x64, 0x00,
        0x69, 0x00, 0x63, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x11, 0xba
    };
    
    public GTHServer(ILogger<GTHServer> logger)
    {
        _logger = logger;
    }

    public async Task StartAsync(string ip, int port)
    {
        _ipAddress = IPAddress.Parse(ip);
        _port = port;
        _listener = new TcpListener(_ipAddress, _port);
        _listener.Start();
        _isRunning = true;

        _logger.LogInformation($"[Houston server] Ожидание подключений на {_ipAddress}:{_port}...");

        OnStarted?.Invoke();
        
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
                catch (InvalidOperationException)
                {
                    break;
                }

                if (!_isRunning) break;

                _clients.Add(client);
                _logger.LogInformation($"[Houston server] Подключился клиент: {client.Client.RemoteEndPoint}");
                _ = HandleClientAsync(client);
            }
        }
        finally
        {
            OnStopped?.Invoke();
            _logger.LogInformation("[Houston server] Цикл ожидания завершён.");
        }
    }

    public async Task StopAsync()
    {
        _isRunning = false;
        _listener?.Stop();

        foreach (var client in _clients)
        {
            client.Close();
        }

        _logger.LogInformation("[Houston server] Сервер остановлен.");
        await Task.CompletedTask;
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        var remote = client.Client.RemoteEndPoint?.ToString() ?? "неизвестен";

        using NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];

        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                byte[] received = buffer[..bytesRead];
                
                byte[] decodedData = UnwrapKissFrame(received);
                if (decodedData.Length > 0)
                {
                    _logger.LogInformation($"[Houston server|KISS decoded] От {remote}: {BitConverter.ToString(decodedData)}");
                    received = decodedData;
                }

                bool isAppInfoMsg = bytesRead >= 0x38 &&
                                    received.Length >= 0x38 &&
                                    received[0] == 0x5e && received[1] == 0xba &&
                                    received[2] == 0x01 && received[3] == 0x10;

                if (isAppInfoMsg)
                {
                    int version = BitConverter.ToInt32(received, 0x0A);
                    string desc = Encoding.ASCII.GetString(received, 0x0E, 32).TrimEnd('\0');

                    string clientRole = received[0x24] switch
                    {
                        0x00 => "Manager",
                        0x01 => "Monitor",
                        0x02 => "Viewer",
                        _ => $"Unknown({received[0x24]})"
                    };

                    await stream.WriteAsync(srvHelloMsg, 0, srvHelloMsg.Length);
                    _logger.LogInformation($"[Houston server|Handshake] Ответ отправлен {remote}");

                    var appModel = new HoustonApp
                    {
                        Id = _clientsInfo.Count + 1,
                        Name = $"{desc} (ver: {version})",
                        Address = remote,
                        Status = clientRole,
                        IsConnected = true
                    };

                    _clientsInfo.AddOrUpdate(remote, appModel, (_, __) => appModel);
                    _logger.LogInformation($"[Houston server|AppModel] Зарегистрирован клиент: {appModel}");
                    continue;
                }

                string hex = BitConverter.ToString(received);
                _logger.LogInformation($"[Houston server|RX] От {remote}: {hex}");
                _logger.LogInformation($"[Houston server|RX] Байт: [{string.Join(", ", received)}]");

                if (TryParseHoustonCommand(received, out var houstonPayload))
                {
                    byte[] kissFrame = WrapInKissFrame(houstonPayload);
                    await stream.WriteAsync(kissFrame, 0, kissFrame.Length);
                    _logger.LogInformation($"[Houston server|TX] Отправлено в KISS {remote}: {BitConverter.ToString(kissFrame)}");

                    byte[] decoded = UnwrapKissFrame(kissFrame);
                    _logger.LogInformation($"[Houston server|TX decoded] Раскодированное: {BitConverter.ToString(decoded)}");
                }
                else
                {
                    _logger.LogInformation($"[Houston server|Парсинг] Не удалось разобрать сообщение от {remote}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Houston server|Ошибка] {remote}: {ex.Message}");
        }
        finally
        {
            client.Close();
            _logger.LogInformation($"[Houston server] Клиент отключён: {remote}");

            if (_clientsInfo.TryGetValue(remote, out var appModel))
            {
                appModel.IsConnected = false;
                _clientsInfo[remote] = appModel;

                var disconnectedClients = _clientsInfo.Values.Where(c => !c.IsConnected).ToList();
                _logger.LogInformation("[Houston server|AppModel] Отключённые клиенты:");
                foreach (var dc in disconnectedClients)
                    _logger.LogInformation(dc.ToString());

                _clientsInfo.TryRemove(remote, out _);
            }
            else
            {
                _logger.LogInformation($"[Houston server|AppModel] Клиент {remote} не найден.");
            }
        }
    }

    public async Task SendToAllClientsAsync(byte[] data)
    {
        foreach (var client in _clients.ToList())
        {
            if (!client.Connected) continue;

            try
            {
                NetworkStream stream = client.GetStream();
                await stream.WriteAsync(data, 0, data.Length);

                _logger.LogInformation($"[Houston server|TX] Рассылка: {BitConverter.ToString(data)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Houston server|Ошибка] Не удалось отправить: {ex.Message}");
            }
        }
    }

    private static bool TryParseHoustonCommand(byte[] data, out byte[] payload)
    {
        payload = Array.Empty<byte>();
        if (data.Length < 12) return false;
        if (data[0] != 0x5E || data[1] != 0xBA) return false;
        if (data[^2] != 0x11 || data[^1] != 0xBA) return false;

        int payloadStartIndex = 10;
        int payloadLength = data.Length - 2 - payloadStartIndex;
        if (payloadLength <= 0) return false;

        payload = data.Skip(payloadStartIndex).Take(payloadLength).ToArray();
        return true;
    }

    private static byte[] WrapInKissFrame(byte[] payload)
    {
        List<byte> kissFrame = new() { 0xC0, 0x00 };

        foreach (var b in payload)
        {
            if (b == 0xC0)
            {
                kissFrame.Add(0xDB);
                kissFrame.Add(0xDC);
            }
            else if (b == 0xDB)
            {
                kissFrame.Add(0xDB);
                kissFrame.Add(0xDD);
            }
            else
            {
                kissFrame.Add(b);
            }
        }

        kissFrame.Add(0xC0);
        return kissFrame.ToArray();
    }

    private static byte[] UnwrapKissFrame(byte[] kissFrame)
    {
        if (kissFrame == null || kissFrame.Length < 3) return Array.Empty<byte>();
        if (kissFrame[0] != 0xC0 || kissFrame[^1] != 0xC0) return Array.Empty<byte>();

        var dataWithEscapes = kissFrame.Skip(2).Take(kissFrame.Length - 3).ToArray();
        List<byte> decoded = new();

        for (int i = 0; i < dataWithEscapes.Length; i++)
        {
            if (dataWithEscapes[i] == 0xDB)
            {
                if (i + 1 < dataWithEscapes.Length)
                {
                    byte next = dataWithEscapes[i + 1];
                    if (next == 0xDC)
                    {
                        decoded.Add(0xC0);
                        i++;
                    }
                    else if (next == 0xDD)
                    {
                        decoded.Add(0xDB);
                        i++;
                    }
                    else
                    {
                        decoded.Add(dataWithEscapes[i]);
                    }
                }
                else
                {
                    decoded.Add(dataWithEscapes[i]);
                }
            }
            else
            {
                decoded.Add(dataWithEscapes[i]);
            }
        }

        return decoded.ToArray();
    }
}