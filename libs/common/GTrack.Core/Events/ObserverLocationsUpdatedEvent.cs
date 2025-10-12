using GMap.NET;

namespace GTrack.Core.Events;

/// <summary>
/// Event triggered when the list of observer locations is updated.
/// Contains a read-only list of <see cref="PointLatLng"/> objects representing updated positions.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при обновлении списка местоположений наблюдателей.
/// Содержит только для чтения список объектов <see cref="PointLatLng"/>, представляющих обновлённые координаты.
/// </summary>
public class ObserverLocationsUpdatedEvent : PubSubEvent<IReadOnlyList<PointLatLng>> { }