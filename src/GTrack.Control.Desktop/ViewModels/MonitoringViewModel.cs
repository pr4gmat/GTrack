using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GTrack.Control.Desktop.ViewModels;

/// <summary>
/// ViewModel for monitoring log messages
/// ViewModel для отображения и мониторинга лог-сообщений
/// </summary>
public class MonitoringViewModel : BindableBase, INavigationAware, IDisposable
{
    private readonly ILogger<MonitoringViewModel> _logger;

    private string _logMessagesAsText = string.Empty;
    /// <summary>
    /// All log messages concatenated into a single string for display
    /// Все лог-сообщения в виде одной строки для отображения
    /// </summary>
    public string LogMessagesAsText
    {
        get => _logMessagesAsText;
        private set => SetProperty(ref _logMessagesAsText, value);
    }

    /// <summary>
    /// Observable collection of individual log messages
    /// Коллекция отдельных лог-сообщений, обновляется в реальном времени
    /// </summary>
    public ObservableCollection<string> LogMessages { get; }

    /// <summary>
    /// Constructor
    /// Конструктор ViewModel
    /// </summary>
    public MonitoringViewModel(ILogger<MonitoringViewModel> logger, ObservableCollection<string> logMessages)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        LogMessages = logMessages ?? throw new ArgumentNullException(nameof(logMessages));

        // Initialize concatenated log text
        // Инициализация текста логов
        var sb = new StringBuilder();
        foreach (var msg in LogMessages)
            sb.AppendLine(msg);
        LogMessagesAsText = sb.ToString();

        // Subscribe to collection changes to update LogMessagesAsText
        // Подписка на изменения коллекции для обновления LogMessagesAsText
        LogMessages.CollectionChanged += LogMessages_CollectionChanged;
    }

    /// <summary>
    /// Handles updates when log messages collection changes
    /// Обработка обновлений при изменении коллекции логов
    /// </summary>
    private void LogMessages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems != null)
                {
                    foreach (string msg in e.NewItems)
                        LogMessagesAsText += msg + Environment.NewLine;
                }
                break;

            case NotifyCollectionChangedAction.Reset:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
            default:
                // Rebuild full log text for other actions
                // Перестраиваем весь текст логов для остальных действий
                var sb = new StringBuilder();
                foreach (var item in LogMessages)
                    sb.AppendLine(item);
                LogMessagesAsText = sb.ToString();
                break;
        }
    }

    /// <summary>
    /// Unsubscribe from events when disposing
    /// Отписка от событий при освобождении ViewModel
    /// </summary>
    public void Dispose()
    {
        LogMessages.CollectionChanged -= LogMessages_CollectionChanged;
    }

    // Navigation-aware methods
    // Методы для навигации
    public void OnNavigatedTo(NavigationContext navigationContext) { }
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}