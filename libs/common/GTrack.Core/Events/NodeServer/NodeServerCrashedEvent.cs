namespace GTrack.Core.Events.NodeServer;

/// <summary>
/// Event triggered when the Node server crashes or encounters an exception.
/// Contains an <see cref="Exception"/> with detailed crash information.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при сбое или возникновении исключения на сервере Node.
/// Содержит объект <see cref="Exception"/> с подробной информацией о сбое.
/// </summary>
public class NodeServerCrashedEvent : PubSubEvent<Exception> { }