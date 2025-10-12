using GMap.NET;
using GMap.NET.MapProviders;

namespace GTrack.ObserverMap.Module.Services;

/// <summary>
/// Implements map caching and online/offline mode management.
/// / Реализует кеширование карты и управление режимами онлайн/оффлайн.
/// </summary>
public class MapCacheService : IMapCacheService
{
    /// <summary>
    /// Sets the map to online mode with caching enabled.
    /// / Включает онлайн режим карты с кешированием.
    /// </summary>
    public void UseOnlineWithCache()
    {
        GMaps.Instance.Mode = AccessMode.ServerAndCache;
        GMapProvider.Language = LanguageType.Russian;
    }

    /// <summary>
    /// Sets the map to offline mode only.
    /// / Включает только оффлайн режим карты.
    /// </summary>
    public void UseOffline()
    {
        GMaps.Instance.Mode = AccessMode.CacheOnly;
    }

    /// <summary>
    /// Preloads map tiles for a specific area and zoom levels.
    /// / Предзагружает тайлы карты для указанной области и уровней масштабирования.
    /// </summary>
    /// <param name="area">Area to preload / Область для предзагрузки</param>
    /// <param name="minZoom">Minimum zoom level / Минимальный уровень масштабирования</param>
    /// <param name="maxZoom">Maximum zoom level / Максимальный уровень масштабирования</param>
    public async Task PreloadArea(RectLatLng area, int minZoom, int maxZoom)
    {
        await Task.Run(() =>
        {
            var provider = GMapProviders.GoogleMap;

            for (int zoom = minZoom; zoom <= maxZoom; zoom++)
            {
                var tiles = provider.Projection.GetAreaTileList(area, zoom, 0);

                foreach (var tile in tiles)
                {
                    try
                    {
                        Exception ex;
                        var img = GMaps.Instance.GetImageFrom(provider, tile, zoom, out ex);
                        img?.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
        });
    }
}