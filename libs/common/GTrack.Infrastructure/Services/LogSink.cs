using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using GTrack.Core.Services;
using Serilog.Core;
using Serilog.Events;

namespace GTrack.Infrastructure.Services;

/// <summary>
/// Log sink that collects log messages in an ObservableCollection and supports Serilog integration.
/// Thread-safe and updates UI via Dispatcher if needed.
/// </summary>
/// <summary>
/// Приёмник логов, который собирает сообщения в ObservableCollection и поддерживает интеграцию с Serilog.
/// Потокобезопасный, обновляет UI через Dispatcher при необходимости.
/// </summary>
public class LogSink : ILogSink, ILogEventSink
{
    private readonly ObservableCollection<string> _logCollection;
    private readonly object _lock = new();
    private readonly Dispatcher _dispatcher;

    public LogSink(ObservableCollection<string> logCollection)
    {
        _logCollection = logCollection ?? throw new ArgumentNullException(nameof(logCollection));
        _dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        BindingOperations.EnableCollectionSynchronization(_logCollection, _lock);
    }

    public void Emit(string message) => AddMessageSafe(message);

    public void Emit(LogEvent logEvent)
    {
        var msg = $"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss} [{logEvent.Level}] {logEvent.RenderMessage()}";
        AddMessageSafe(msg);
    }

    private void AddMessageSafe(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_dispatcher.CheckAccess())
            _logCollection.Add(message);
        else
            _dispatcher.BeginInvoke(new Action(() => _logCollection.Add(message)), DispatcherPriority.Background);
    }
}