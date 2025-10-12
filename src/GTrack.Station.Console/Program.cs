using System.Net;
using GTrack.Station.Client;

internal class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: <program> <IP Address> <Port>");
            return;
        }

        string ipAddressInput = args[0];
        string portInput = args[1];

        if (!IPAddress.TryParse(ipAddressInput, out var ipAddress))
        {
            Console.WriteLine($"Invalid IP address: {ipAddressInput}");
            return;
        }

        if (!int.TryParse(portInput, out int port) || port < 1 || port > 65535)
        {
            Console.WriteLine($"Invalid port number: {portInput}");
            return;
        }

        var client = new GTrackStationClient();

        try
        {
            await client.StartAsync(ipAddress.ToString(), port);

            while (client.IsConnected)
            {
                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Startup Error] {ex.Message}");
        }
    }
}