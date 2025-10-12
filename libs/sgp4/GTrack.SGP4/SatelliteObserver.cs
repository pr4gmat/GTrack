using System.IO;
using GTrack.Core.Models;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.Observation;
using SGPdotNET.TLE;
using SGPdotNET.Util;

namespace GTrack.SGP4;

/// <summary>
/// Observes satellites based on TLE data from a ground station.
/// / Наблюдает спутники на основе данных TLE с наземной станции.
/// </summary>
public class SatelliteObserver : ISatelliteObserver
{
    private readonly Dictionary<string, Satellite> _satellites = new(); // Satellites dictionary / Словарь спутников
    private GroundStation _groundStation; // Ground station instance / Наземная станция
    private double _txFrequencyHz; // Transmission frequency / Частота передачи

    public async Task<IEnumerable<SatelliteObservationResult>> ObserveFromTleDataAsync(
        string name, string line1, string line2,
        double latitudeDeg, double longitudeDeg, double altitudeKm,
        double txFrequencyHz)
    {
        _txFrequencyHz = txFrequencyHz;

        string filePath = "satellite_data.tle";

        await WriteTleToFileAsync(filePath, name, line1, line2);

        var lines = await File.ReadAllLinesAsync(filePath);

        if (lines.Length % 3 != 0)
            throw new InvalidDataException("TLE file must contain a name plus 2 lines for each satellite.");

        var observerLocation = new GeodeticCoordinate(
            Angle.FromDegrees(latitudeDeg),
            Angle.FromDegrees(longitudeDeg),
            altitudeKm
        );

        _groundStation = new GroundStation(observerLocation);

        _satellites.Clear();

        for (int i = 0; i < lines.Length; i += 3)
        {
            string satName = lines[i].Trim();
            string satLine1 = lines[i + 1].Trim();
            string satLine2 = lines[i + 2].Trim();

            try
            {
                var tle = new Tle(satName, satLine1, satLine2);
                var satellite = new Satellite(tle);
                _satellites[satName] = satellite;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing satellite {satName}: {ex.Message}");
            }
        }

        return _satellites.Keys.Select(UpdateSatelliteData);
    }

    private async Task WriteTleToFileAsync(string filePath, string name, string line1, string line2)
    {
        try
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            var data = new string[]
            {
                name,
                line1,
                line2
            };

            await File.AppendAllLinesAsync(filePath, data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing TLE data to file: {ex.Message}");
            throw;
        }
    }

    public SatelliteObservationResult UpdateSatelliteData(string satelliteName)
    {
        if (!_satellites.TryGetValue(satelliteName, out var satellite))
            throw new KeyNotFoundException($"Satellite '{satelliteName}' not found.");

        var time = DateTime.UtcNow;
        var observation = _groundStation.Observe(satellite, time);

        double c = 299792.458; // Speed of light km/s / Скорость света км/с
        double doppler = -(observation.RangeRate / c) * _txFrequencyHz;

        return new SatelliteObservationResult
        {
            Name = satelliteName,
            Time = time,
            Azimuth = observation.Azimuth.Degrees,
            Elevation = observation.Elevation.Degrees,
            Range = observation.Range,
            Doppler = doppler
        };
    }
}