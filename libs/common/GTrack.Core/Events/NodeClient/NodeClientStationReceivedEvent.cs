using GTrack.Core.Models;

namespace GTrack.Core.Events.NodeClient;

/// <summary>
/// Event triggered when a Node client receives station data.
/// Contains a <see cref="Station"/> instance representing the received information.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при получении клиентом Node данных станции.
/// Содержит объект <see cref="Station"/>, представляющий полученную информацию.
/// </summary>
public class NodeClientStationReceivedEvent : PubSubEvent<Station> { }