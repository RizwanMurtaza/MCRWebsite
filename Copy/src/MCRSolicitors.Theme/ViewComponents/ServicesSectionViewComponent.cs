using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering services section
/// </summary>
public class ServicesSectionViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ServicesSectionViewModel? model = null)
    {
        model ??= GetDefaultServicesModel();
        return View(model);
    }

    private static ServicesSectionViewModel GetDefaultServicesModel()
    {
        return new ServicesSectionViewModel
        {
            SectionTitle = "Our Legal Services",
            SectionSubtitle = "Comprehensive Solutions for All Your Legal Needs",
            Services = new List<ServiceCardViewModel>
            {
                new()
                {
                    Title = "Family Law",
                    Description = "Expert guidance through divorce, child custody, and family matters with compassion and understanding.",
                    IconClass = "fas fa-heart",
                    Url = "/services/family-law"
                },
                new()
                {
                    Title = "Immigration",
                    Description = "Navigate complex immigration processes with our experienced team. Visas, citizenship, and more.",
                    IconClass = "fas fa-globe",
                    Url = "/services/immigration"
                },
                new()
                {
                    Title = "Conveyancing",
                    Description = "Smooth property transactions from start to finish. Buying, selling, or remortgaging made simple.",
                    IconClass = "fas fa-home",
                    Url = "/services/conveyancing"
                },
                new()
                {
                    Title = "Wills & Probate",
                    Description = "Protect your family's future with expert will writing and estate planning services.",
                    IconClass = "fas fa-file-signature",
                    Url = "/services/wills-probate"
                },
                new()
                {
                    Title = "Personal Injury",
                    Description = "Get the compensation you deserve. No win, no fee representation for injury claims.",
                    IconClass = "fas fa-user-injured",
                    Url = "/services/personal-injury"
                },
                new()
                {
                    Title = "Employment Law",
                    Description = "Workplace disputes, unfair dismissal, and employment contracts handled professionally.",
                    IconClass = "fas fa-briefcase",
                    Url = "/services/employment-law"
                }
            },
            CtaText = "View All Services",
            CtaUrl = "/services"
        };
    }
}
