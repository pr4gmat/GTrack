using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GTrack.Station.Client;

/// <summary>
/// Implementation of a GTrack Station client responsible for connecting to the Station server,
/// handling communication, and managing received NSRD messages.
/// </summary>
/// <summary>
/// Реализация клиента GTrack Station, отвечающего за подключение к серверу Station,
/// обработку сообщений и управление полученными NSRD данными.
/// </summary>
public class GTrackStationClient : IGTrackStationClient
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    private Task? _listenTask;
    private bool _running;

    public bool IsConnected => _client?.Connected ?? false;
    public string? StationId { get; private set; }

    public async Task StartAsync(string serverIp, int serverPort)
    {
        _client = new TcpClient();

        try
        {
            await _client.ConnectAsync(serverIp, serverPort);
            _stream = _client.GetStream();
            _running = true;

            Console.WriteLine($"[Station client] Connected to {serverIp}:{serverPort}");

            await SendMessageAsync("MYIDSTATION");

            _listenTask = Task.Run(ListenAsync);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Station client] Connection error to {serverIp}:{serverPort}: {ex.Message}");
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

                Console.WriteLine($"[Station client] Received: {message}");

                await HandleMessageAsync(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Station client] ListenAsync error: {ex.Message}");
        }
        finally
        {
            Disconnect();
            Console.WriteLine("[Station client] Connection closed");
        }
    }

    private async Task HandleMessageAsync(string message)
    {
        switch (message)
        {
            case var msg when msg.StartsWith("YOURIDSTATION:"):
                StationId = msg.Substring("YOURIDSTATION:".Length);
                Console.WriteLine($"[Station client] StationId: {StationId}");
                break;

            case var msg when msg.StartsWith("NSRD:"):
                try
                {
                    var nsrd = NsrdData.TryParse(msg);
                    if (nsrd != null && nsrd.StationId == StationId)
                    {
                        Console.WriteLine($"[Station client] NSRD for {nsrd.StationId}:");
                        Console.WriteLine($"  Time (UTC): {nsrd.TimeUtc}");
                        Console.WriteLine($"  Name: {nsrd.Name}");
                        Console.WriteLine($"  Azimuth: {nsrd.Azimuth}°");
                        Console.WriteLine($"  Elevation: {nsrd.Elevation}°");
                        Console.WriteLine($"  Range: {nsrd.Range} km");
                        Console.WriteLine($"  Doppler: {nsrd.Doppler} Hz");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Station client] NSRD parsing error: {ex.Message}");
                }
                break;
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (_stream == null) throw new InvalidOperationException("Client is not connected");

        var data = Encoding.UTF8.GetBytes(message);
        await _stream.WriteAsync(data, 0, data.Length);
        await _stream.FlushAsync();

        Console.WriteLine($"[Station client] Sent: {message}");
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
            Console.WriteLine($"[Station client] Disconnect error: {ex.Message}");
        }
        finally
        {
            _client = null;
            _stream = null;
        }
    }
}