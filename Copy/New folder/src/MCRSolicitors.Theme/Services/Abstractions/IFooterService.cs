using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.Services.Abstractions;

/// <summary>
/// Service interface for footer data (Interface Segregation Principle)
/// </summary>
public interface IFooterService
{
    Task<FooterViewModel> GetFooterAsync();
    Task<List<FooterLinkSection>> GetLinkSectionsAsync();
    Task<List<AccreditationBadge>> GetAccreditationsAsync();
}
