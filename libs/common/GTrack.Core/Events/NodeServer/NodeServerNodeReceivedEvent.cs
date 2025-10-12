using GTrack.Core.Models;

namespace GTrack.Core.Events.NodeServer;

/// <summary>
/// Event triggered when the Node server receives data or a connection request from a node.
/// Contains a <see cref="Node"/> object representing the node involved in the event.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при получении сервером Node данных или запроса на подключение от узла.
/// Содержит объект <see cref="Node"/>, представляющий участвующий в событии узел.
/// </summary>
public class NodeServerNodeReceivedEvent : PubSubEvent<Node> { }