namespace GTrack.Core.Events.StationServer;

/// <summary>
/// Event triggered when the Station server crashes or encounters an exception.
/// Contains an <see cref="Exception"/> object with details about the failure.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при сбое или возникновении исключения на сервере станций.
/// Содержит объект <see cref="Exception"/> с подробной информацией об ошибке.
/// </summary>
public class StationServerCrashedEvent : PubSubEvent<Exception> { }