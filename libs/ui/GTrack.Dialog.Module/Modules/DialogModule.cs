using GTrack.Dialog.Module.ViewModels;
using GTrack.Dialog.Module.Views;

namespace GTrack.Dialog.Module.Modules;

/// <summary>
/// Registers dialog views and view models in the DI container.
/// / Регистрирует диалоговые представления и модели представлений в DI контейнере.
/// </summary>
public class DialogModule : IModule
{
    /// <summary>
    /// Registers dialog types in the container.
    /// / Регистрирует диалоговые типы в контейнере.
    /// </summary>
    /// <param name="containerRegistry">Container registry / Реестр контейнера</param>
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterDialogWindow<DialogView>(); // Dialog window / Диалоговое окно
        containerRegistry.RegisterDialog<DialogMessageView, DialogMessageViewModel>(); // Dialog message / Диалоговое сообщение
    }

    /// <summary>
    /// Called after module initialization.
    /// / Вызывается после инициализации модуля.
    /// </summary>
    /// <param name="containerProvider">Container provider / Провайдер контейнера</param>
    public void OnInitialized(IContainerProvider containerProvider) { }
}