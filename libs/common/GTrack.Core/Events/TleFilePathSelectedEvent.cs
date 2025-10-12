using GTrack.Core.Models;

namespace GTrack.Core.Events;

/// <summary>
/// Event triggered when a TLE file path is selected by the user or system.
/// Contains a <see cref="TLE"/> instance representing the selected TLE data.
/// </summary>
///
/// <summary>
/// Событие, вызываемое при выборе пути к файлу TLE пользователем или системой.
/// Содержит объект <see cref="TLE"/>, представляющий выбранные данные TLE.
/// </summary>
public class TleFilePathSelectedEvent : PubSubEvent<TLE> { }