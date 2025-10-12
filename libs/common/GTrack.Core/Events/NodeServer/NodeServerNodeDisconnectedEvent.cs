using GTrack.Core.Models;

namespace GTrack.Core.Events.NodeServer;

/// <summary>
/// Event triggered when a node disconnects from the Node server.
/// Contains a <see cref="Node"/> object representing the disconnected node.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при отключении узла от сервера Node.
/// Содержит объект <see cref="Node"/>, представляющий отключённый узел.
/// </summary>
public class NodeServerNodeDisconnectedEvent : PubSubEvent<Node> { }