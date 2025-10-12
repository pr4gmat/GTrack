namespace GTrack.Core.Events.HoustonServer;

/// <summary>
/// Event triggered when the Houston server crashes.
/// Carries an <see cref="Exception"/> instance containing crash details.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при сбое (краше) сервера Houston.
/// Содержит объект <see cref="Exception"/>, включающий подробности ошибки.
/// </summary>
public class HoustonServerCrashedEvent : PubSubEvent<Exception> { }