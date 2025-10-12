using GTrack.Core.Models;

namespace GTrack.Core.Events.NodeClient;

/// <summary>
/// Event triggered when a Node client receives TLE (Two-Line Element) data for a specific station.
/// Contains a tuple with both the <see cref="Station"/> and <see cref="TLE"/> objects.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при получении клиентом Node данных TLE (двухстрочного элемента) для конкретной станции.
/// Содержит кортеж с объектами <see cref="Station"/> и <see cref="TLE"/>.
/// </summary>
public class NodeClientTleReceivedEvent : PubSubEvent<(Station Station, TLE Tle)> { }