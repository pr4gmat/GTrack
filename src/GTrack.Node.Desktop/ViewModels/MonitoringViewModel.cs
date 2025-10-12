using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GTrack.Node.Desktop.ViewModels;

/// <summary>
/// ViewModel for monitoring log messages in the Node application.
/// ViewModel для отображения логов приложения Node.
/// </summary>
public class MonitoringViewModel : BindableBase, INavigationAware, IDisposable
{
    private readonly ILogger<MonitoringViewModel> _logger;

    // Combined log messages as a single string for display
    // Объединенные сообщения лога в одну строку для отображения
    private string _logMessagesAsText = string.Empty;
    public string LogMessagesAsText
    {
        get => _logMessagesAsText;
        private set => SetProperty(ref _logMessagesAsText, value);
    }

    // Observable collection of log messages
    // Наблюдаемая коллекция сообщений лога
    public ObservableCollection<string> LogMessages { get; }

    /// <summary>
    /// Constructor initializes the ViewModel and subscribes to log changes.
    /// Конструктор инициализирует ViewModel и подписывается на изменения лога.
    /// </summary>
    public MonitoringViewModel(ILogger<MonitoringViewModel> logger, ObservableCollection<string> logMessages)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        LogMessages = logMessages ?? throw new ArgumentNullException(nameof(logMessages));

        // Initialize the combined log text
        // Инициализация объединенного текста лога
        var sb = new StringBuilder();
        foreach (var msg in LogMessages)
            sb.AppendLine(msg);
        LogMessagesAsText = sb.ToString();

        // Subscribe to collection changes
        // Подписка на изменения коллекции
        LogMessages.CollectionChanged += LogMessages_CollectionChanged;
    }

    /// <summary>
    /// Handles updates when log messages collection changes.
    /// Обрабатывает обновления при изменении коллекции сообщений лога.
    /// </summary>
    private void LogMessages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                // Append new messages to the combined text
                // Добавляем новые сообщения в объединенный текст
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
                // Rebuild the combined text from the collection
                // Перестраиваем объединенный текст из коллекции
                var sb = new StringBuilder();
                foreach (var item in LogMessages)
                    sb.AppendLine(item);
                LogMessagesAsText = sb.ToString();
                break;
        }
    }

    /// <summary>
    /// Unsubscribe from collection changes to prevent memory leaks.
    /// Отписка от изменений коллекции для предотвращения утечек памяти.
    /// </summary>
    public void Dispose()
    {
        LogMessages.CollectionChanged -= LogMessages_CollectionChanged;
    }

    // INavigationAware interface implementation
    // Реализация интерфейса INavigationAware
    public void OnNavigatedTo(NavigationContext navigationContext) { }
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}