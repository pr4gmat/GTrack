static async Task Main()
{
    var observerService = new SatelliteObserver();

    try
    {
        var results = await observerService.ObserveFromTleFileAsync(
            filePath: "tle.txt",
            latitudeDeg: 55.9995,
            longitudeDeg: 53.0217,
            altitudeKm: 0,
            txFrequencyHz: 437.5e6
        );

        var firstSatelliteName = results.FirstOrDefault()?.Name;

        while (true)
        {
            var obs = observerService.UpdateSatelliteData(firstSatelliteName);

            Console.Clear();
            Console.WriteLine($"[{obs.Name}] @ {obs.Time:HH:mm:ss} UTC");
            Console.WriteLine($"Azimuth:   {obs.Azimuth:F2}°");
            Console.WriteLine($"Elevation: {obs.Elevation:F2}°");
            Console.WriteLine($"Range:     {obs.Range:F2} km");
            Console.WriteLine($"Doppler:   {obs.Doppler:F2} Hz\n");

            await Task.Delay(1);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Ошибка при наблюдении: " + ex.Message);
    }
}