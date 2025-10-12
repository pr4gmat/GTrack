using GTrack.Core.Models;

namespace GTrack.Core.Events.StationServer;

/// <summary>
/// Event triggered when a station disconnects from the Station server.
/// Contains a <see cref="Station"/> instance representing the disconnected station.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при отключении станции от сервера станций.
/// Содержит объект <see cref="Station"/>, представляющий отключённую станцию.
/// </summary>
public class StationServerStationDisconnectedEvent : PubSubEvent<Station> { }