using GTrack.Core.Models;

namespace GTrack.Core.Events;

/// <summary>
/// Event triggered when a new observer location is added to the system.
/// Contains an <see cref="ObserverLocation"/> instance representing the added location.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при добавлении нового местоположения наблюдателя в систему.
/// Содержит объект <see cref="ObserverLocation"/>, представляющий добавленное местоположение.
/// </summary>
public class ObserverLocationAddedEvent : PubSubEvent<ObserverLocation> { }