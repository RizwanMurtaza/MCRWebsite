using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering call-to-action sections
/// </summary>
public class CtaSectionViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(CtaSectionViewModel? model = null)
    {
        model ??= GetDefaultCtaModel();
        return View(model);
    }

    private static CtaSectionViewModel GetDefaultCtaModel()
    {
        return new CtaSectionViewModel
        {
            Title = "Ready to Discuss Your Case?",
            Description = "Get in touch today for a free, no-obligation consultation with one of our expert solicitors.",
            PrimaryButton = new HeroCtaButton
            {
                Text = "Book Free Consultation",
                Url = "/contact",
                Style = ButtonStyle.Primary
            },
            SecondaryButton = new HeroCtaButton
            {
                Text = "Call 0161 804 7777",
                Url = "tel:01618047777",
                Style = ButtonStyle.Outline
            },
            Style = CtaStyle.Banner,
            ShowContactInfo = true,
            PhoneNumber = "0161 804 7777",
            Email = "info@mcrsolicitors.co.uk"
        };
    }
}
