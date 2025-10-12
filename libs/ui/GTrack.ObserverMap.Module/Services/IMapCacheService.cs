using GMap.NET;

namespace GTrack.ObserverMap.Module.Services;

/// <summary>
/// Service interface for caching map tiles and controlling online/offline modes.
/// / Интерфейс сервиса для кеширования тайлов карты и управления режимами онлайн/оффлайн.
/// </summary>
public interface IMapCacheService
{
    /// <summary>
    /// Sets the map to use online mode with caching enabled.
    /// / Включает онлайн режим карты с кешированием.
    /// </summary>
    void UseOnlineWithCache();

    /// <summary>
    /// Sets the map to use offline mode only.
    /// / Включает только оффлайн режим карты.
    /// </summary>
    void UseOffline();

    /// <summary>
    /// Preloads map tiles for the specified area and zoom levels.
    /// / Предзагружает тайлы карты для указанной области и уровней масштабирования.
    /// </summary>
    /// <param name="area">Area to preload / Область для предзагрузки</param>
    /// <param name="minZoom">Minimum zoom level / Минимальный уровень масштабирования</param>
    /// <param name="maxZoom">Maximum zoom level / Максимальный уровень масштабирования</param>
    Task PreloadArea(RectLatLng area, int minZoom, int maxZoom);
}