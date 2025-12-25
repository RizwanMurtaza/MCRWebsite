using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering the About Us section
/// </summary>
public class AboutSectionViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(AboutPageViewModel? model = null)
    {
        model ??= GetDefaultAboutModel();
        return View(model);
    }

    private static AboutPageViewModel GetDefaultAboutModel()
    {
        return new AboutPageViewModel
        {
            Title = "About MCR Solicitors",
            IntroText = @"MCR Solicitors is a leading law firm based in the heart of Manchester, providing expert legal services to individuals and families across the UK. Founded on the principles of accessibility, expertise, and compassion, we are committed to delivering exceptional legal outcomes for every client we serve.

Our team of experienced solicitors specializes in Immigration Law, Family Law, and Personal Injury claims. We understand that legal matters can be stressful and overwhelming, which is why we take a client-first approach, ensuring you receive clear guidance and support throughout your legal journey.

As an SRA-regulated firm, we uphold the highest standards of professional conduct and ethics. Our commitment to excellence has earned us the trust of thousands of clients and numerous industry accreditations.",
            Values = new List<CompanyValue>
            {
                new()
                {
                    IconClass = "fas fa-handshake",
                    Title = "Client-First Approach",
                    Description = "We put our clients at the center of everything we do, ensuring personalized service and clear communication."
                },
                new()
                {
                    IconClass = "fas fa-award",
                    Title = "Excellence",
                    Description = "We strive for excellence in every case, combining expertise with dedication to achieve the best outcomes."
                },
                new()
                {
                    IconClass = "fas fa-heart",
                    Title = "Compassion",
                    Description = "We approach every case with empathy and understanding, supporting clients through challenging times."
                },
                new()
                {
                    IconClass = "fas fa-shield-alt",
                    Title = "Integrity",
                    Description = "We operate with complete transparency and honesty, upholding the highest ethical standards."
                }
            },
            Stats = new CompanyStats
            {
                YearsExperience = 15,
                HappyClients = 5000,
                CasesWon = 4500,
                SolicitorCount = 12
            }
        };
    }
}
