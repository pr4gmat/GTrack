using GTrack.Core.Models;

namespace GTrack.Node.Desktop.ViewModels;

/// <summary>
/// ViewModel for the ObserverAdd dialog.
/// ViewModel для диалога добавления новой точки наблюдения.
/// </summary>
public class ObserverAddViewModel : BindableBase, IDialogAware
{
    // The observer location being created or edited
    // Точка наблюдения, которая создается или редактируется
    private ObserverLocation _observerLocation = new();
    private DialogCloseListener _requestClose;

    public ObserverLocation ObserverLocation
    {
        get => _observerLocation;
        set => SetProperty(ref _observerLocation, value);
    }

    /// <summary>
    /// Command to add a new observer location and close the dialog.
    /// Команда для добавления новой точки наблюдения и закрытия диалога.
    /// </summary>
    public DelegateCommand AddCommand { get; }

    public ObserverAddViewModel()
    {
        // Initialize the AddCommand and set its CanExecute condition
        // Инициализация команды AddCommand и привязка условия CanExecute
        AddCommand = new DelegateCommand(OnAdd, CanAdd)
            .ObservesProperty(() => ObserverLocation.Name);
    }

    /// <summary>
    /// Executes the add operation and closes the dialog with OK result.
    /// Выполняет добавление точки и закрывает диалог с результатом OK.
    /// </summary>
    private void OnAdd()
    {
        if (string.IsNullOrWhiteSpace(ObserverLocation.Name)) return;

        var result = new DialogResult(ButtonResult.OK)
        {
            Parameters = new DialogParameters
            {
                { "observer", ObserverLocation } // Pass the new observer location
                // Передаем новую точку наблюдения
            }
        };
        
        RequestClose.Invoke(result);
    }
    
    /// <summary>
    /// Determines if the AddCommand can be executed (Name must not be empty).
    /// Проверяет возможность выполнения команды AddCommand (Имя не должно быть пустым).
    /// </summary>
    private bool CanAdd()
    {
        return !string.IsNullOrWhiteSpace(ObserverLocation?.Name);
    }

    // IDialogAware interface implementation
    // Реализация интерфейса IDialogAware

    public bool CanCloseDialog() => true; // Dialog can always be closed
    public void OnDialogClosed() { }      // No action needed on close
    public void OnDialogOpened(IDialogParameters parameters) { } // No parameters expected

    /// <summary>
    /// Event to request closing the dialog.
    /// Событие для запроса закрытия диалога.
    /// </summary>
    public DialogCloseListener RequestClose { get; }
}