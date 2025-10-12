using GTrack.Node.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace GTrack.Node.Desktop.ViewModels;

/// <summary>
/// ViewModel for workspace navigation in the Node Desktop application.
/// ViewModel для управления навигацией в рабочей области Node Desktop приложения.
/// </summary>
public class WorkspaceViewModel : BindableBase, INavigationAware
{
    private readonly IRegionManager _regionManager;
    private readonly ILogger<WorkspaceViewModel> _logger;
    
    /// <summary>
    /// Command for navigating between views.
    /// Команда для навигации между представлениями.
    /// </summary>
    public DelegateCommand<string> NavigateCommand { get; }

    /// <summary>
    /// Constructor initializing dependencies and commands.
    /// Конструктор инициализирует зависимости и команды.
    /// </summary>
    public WorkspaceViewModel(IRegionManager regionManager, ILogger<WorkspaceViewModel> logger)
    {
        _regionManager = regionManager;
        _logger = logger;
        
        NavigateCommand = new DelegateCommand<string>(Navigate);
    }

    /// <summary>
    /// Executes navigation to the specified target view.
    /// Выполняет навигацию к указанному представлению.
    /// </summary>
    /// <param name="target">Target view name / Название целевого представления</param>
    private void Navigate(string target)
    {
        if (!string.IsNullOrWhiteSpace(target))
        {
            _regionManager.RequestNavigate("WorkspaceRegion", target);
        }
    }

    /// <summary>
    /// Called when the view is navigated to.
    /// Вызывается при переходе на представление.
    /// </summary>
    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        // Navigate to the default Node view on workspace load.
        // Переход к представлению Node по умолчанию при загрузке рабочей области.
        _regionManager.RequestNavigate("WorkspaceRegion", nameof(GTrackNodeView));
    }

    /// <summary>
    /// Determines if the current instance can be reused for navigation.
    /// Определяет, может ли текущий экземпляр использоваться повторно для навигации.
    /// </summary>
    public bool IsNavigationTarget(NavigationContext navigationContext) => true;

    /// <summary>
    /// Called when navigating away from the view.
    /// Вызывается при уходе с представления.
    /// </summary>
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}