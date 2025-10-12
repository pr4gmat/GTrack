using GTrack.Core.Models;

namespace GTrack.Core.Events.StationServer;

/// <summary>
/// Event triggered when the Station server receives data or a signal from a station.
/// Contains a <see cref="Station"/> instance representing the source of the received data.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при получении сервером станций данных или сигнала от станции.
/// Содержит объект <see cref="Station"/>, представляющий источник полученных данных.
/// </summary>
public class StationServerStationReceivedEvent : PubSubEvent<Station> { }