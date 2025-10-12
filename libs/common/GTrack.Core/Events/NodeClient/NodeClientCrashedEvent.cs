namespace GTrack.Core.Events.NodeClient;

/// <summary>
/// Event triggered when a Node client crashes or encounters an unexpected error.
/// Contains an <see cref="Exception"/> object with details about the failure.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при сбое или непредвиденной ошибке клиента Node.
/// Содержит объект <see cref="Exception"/> с подробностями ошибки.
/// </summary>
public class NodeClientCrashedEvent : PubSubEvent<Exception> { }