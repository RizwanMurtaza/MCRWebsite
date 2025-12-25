using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.Services.Abstractions;

/// <summary>
/// Service interface for navigation data (Interface Segregation Principle)
/// </summary>
public interface INavigationService
{
    Task<NavigationViewModel> GetNavigationAsync();
    Task<List<NavigationItemModel>> GetMenuItemsAsync();
}
