using GTrack.ObserverMap.Module.Services;
using GTrack.ObserverMap.Module.ViewModels;
using GTrack.ObserverMap.Module.Views;

namespace GTrack.ObserverMap.Module.Modules;

/// <summary>
/// Module for observer map registration and DI setup.
/// / Модуль для регистрации карты наблюдателя и настройки DI.
/// </summary>
public class ObserverMapModule : IModule
{
    /// <summary>
    /// Registers services and navigation types in the container.
    /// / Регистрирует сервисы и типы навигации в контейнере.
    /// </summary>
    /// <param name="containerRegistry">Container registry / Реестр контейнера</param>
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<IMapCacheService, MapCacheService>(); // Map cache service / Сервис кеша карты
        containerRegistry.RegisterForNavigation<MapView, MapViewModel>(); // Navigation / Навигация
    }

    /// <summary>
    /// Called after module initialization.
    /// / Вызывается после инициализации модуля.
    /// </summary>
    /// <param name="containerProvider">Container provider / Провайдер контейнера</param>
    public void OnInitialized(IContainerProvider containerProvider) { }
}