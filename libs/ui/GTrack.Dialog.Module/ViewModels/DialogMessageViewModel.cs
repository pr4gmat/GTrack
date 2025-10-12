namespace GTrack.Dialog.Module.ViewModels;

/// <summary>
/// ViewModel for dialog messages handling display and closing.
/// / Модель представления для сообщений диалога, управляет отображением и закрытием.
/// </summary>
public class DialogMessageViewModel : BindableBase, IDialogAware
{
    private string _message; // Dialog message / Сообщение диалога

    /// <summary>
    /// Gets or sets the dialog message.
    /// / Получает или задает сообщение диалога.
    /// </summary>
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    /// <summary>
    /// Command to close the dialog.
    /// / Команда для закрытия диалога.
    /// </summary>
    public DelegateCommand CloseDialogCommand { get; }

    /// <summary>
    /// Event to request dialog closure.
    /// / Событие для запроса закрытия диалога.
    /// </summary>
    public DialogCloseListener RequestClose { get; }

    public DialogMessageViewModel()
    {
        CloseDialogCommand = new DelegateCommand(() =>
            RequestClose.Invoke(new DialogResult(ButtonResult.OK)));
    }

    /// <summary>
    /// Determines whether the dialog can be closed.
    /// / Определяет, можно ли закрыть диалог.
    /// </summary>
    public bool CanCloseDialog() => true;

    /// <summary>
    /// Called when the dialog is closed.
    /// / Вызывается при закрытии диалога.
    /// </summary>
    public void OnDialogClosed() { }

    /// <summary>
    /// Called when the dialog is opened.
    /// / Вызывается при открытии диалога.
    /// </summary>
    /// <param name="parameters">Dialog parameters / Параметры диалога</param>
    public void OnDialogOpened(IDialogParameters parameters)
    {
        Message = parameters.GetValue<string>("message");
    }
}