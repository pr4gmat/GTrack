using GTrack.Control.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace GTrack.Control.Desktop.ViewModels;

/// <summary>
/// ViewModel for the main workspace region
/// ViewModel для основной рабочей области приложения
/// </summary>
public class WorkspaceViewModel : BindableBase, INavigationAware
{
    private readonly IRegionManager _regionManager;
    private readonly ILogger<WorkspaceViewModel> _logger;

    /// <summary>
    /// Command to navigate to different views within the workspace
    /// Команда для навигации между различными представлениями внутри рабочей области
    /// </summary>
    public DelegateCommand<string> NavigateCommand { get; }

    /// <summary>
    /// Constructor
    /// Конструктор WorkspaceViewModel
    /// </summary>
    public WorkspaceViewModel(IRegionManager regionManager, ILogger<WorkspaceViewModel> logger)
    {
        _regionManager = regionManager;
        _logger = logger;

        // Initialize navigation command
        // Инициализация команды навигации
        NavigateCommand = new DelegateCommand<string>(Navigate);
    }

    /// <summary>
    /// Handles navigation to the specified target view
    /// Выполняет навигацию к указанному представлению
    /// </summary>
    /// <param name="target">Target view name / Имя целевого представления</param>
    private void Navigate(string target)
    {
        if (!string.IsNullOrWhiteSpace(target))
        {
            _regionManager.RequestNavigate("WorkspaceRegion", target);
        }
    }

    /// <summary>
    /// Called when this ViewModel is navigated to
    /// Вызывается при навигации к этому ViewModel
    /// </summary>
    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        // Navigate to the default GTrackControlView
        // Переход к представлению GTrackControlView по умолчанию
        _regionManager.RequestNavigate("WorkspaceRegion", nameof(GTrackControlView));
    }

    /// <summary>
    /// Determines whether this instance can be reused during navigation
    /// Определяет, может ли этот ViewModel использоваться повторно при навигации
    /// </summary>
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;

    /// <summary>
    /// Called when this ViewModel is navigated from
    /// Вызывается при навигации от этого ViewModel
    /// </summary>
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}