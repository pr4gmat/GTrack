using GTrack.Core.Models;

namespace GTrack.SGP4;

/// <summary>
/// Defines methods for observing satellites based on TLE data.
/// / Определяет методы для наблюдения за спутниками на основе данных TLE.
/// </summary>
public interface ISatelliteObserver
{
    /// <summary>
    /// Observes a satellite from its TLE data and a ground station location asynchronously.
    /// / Выполняет наблюдение за спутником по данным TLE и координатам наземной станции асинхронно.
    /// </summary>
    /// <param name="name">Satellite name / Имя спутника</param>
    /// <param name="line1">First line of TLE / Первая строка TLE</param>
    /// <param name="line2">Second line of TLE / Вторая строка TLE</param>
    /// <param name="latitudeDeg">Observer latitude in degrees / Широта наблюдателя в градусах</param>
    /// <param name="longitudeDeg">Observer longitude in degrees / Долгота наблюдателя в градусах</param>
    /// <param name="altitudeKm">Observer altitude in kilometers / Высота наблюдателя в километрах</param>
    /// <param name="txFrequencyHz">Transmission frequency in Hz / Частота передачи в Гц</param>
    /// <returns>Collection of satellite observation results / Коллекция результатов наблюдения спутника</returns>
    Task<IEnumerable<SatelliteObservationResult>> ObserveFromTleDataAsync(
        string name, string line1, string line2,
        double latitudeDeg, double longitudeDeg, double altitudeKm,
        double txFrequencyHz);

    /// <summary>
    /// Updates satellite data for the specified satellite.
    /// / Обновляет данные спутника для указанного спутника.
    /// </summary>
    /// <param name="satelliteName">Name of the satellite / Имя спутника</param>
    /// <returns>Updated observation result / Обновленный результат наблюдения</returns>
    SatelliteObservationResult UpdateSatelliteData(string satelliteName);
}