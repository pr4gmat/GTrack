namespace GTrack.Core.Services;

/// <summary>
/// Interface representing a log sink that can emit log messages.
/// </summary>
///
/// <summary>
/// Интерфейс, представляющий приёмник логов, способный выводить сообщения.
/// </summary>
public interface ILogSink
{
    /// <summary>
    /// Emits a log message to the sink.
    /// </summary>
    ///
    /// <summary>
    /// Отправляет сообщение лога в приёмник.
    /// </summary>
    /// <param name="message">The message to emit.</param>
    /// <param name="message">Сообщение для отправки.</param>
    void Emit(string message);
}