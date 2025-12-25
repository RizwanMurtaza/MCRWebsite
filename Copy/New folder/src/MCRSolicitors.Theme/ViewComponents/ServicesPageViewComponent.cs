using Microsoft.AspNetCore.Mvc;
using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// View component for rendering the full services page with categories
/// </summary>
public class ServicesPageViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ServicesPageViewModel? model = null)
    {
        model ??= GetDefaultServicesPageModel();
        return View(model);
    }

    private static ServicesPageViewModel GetDefaultServicesPageModel()
    {
        return new ServicesPageViewModel
        {
            Title = "Our Legal Services",
            Description = "MCR Solicitors provides comprehensive legal services across Immigration Law, Family Law, and Personal Injury. Our experienced solicitors are here to guide you through every step of your legal journey.",
            Categories = new List<ServiceCategoryViewModel>
            {
                GetImmigrationCategory(),
                GetFamilyLawCategory(),
                GetPersonalInjuryCategory()
            }
        };
    }

    private static ServiceCategoryViewModel GetImmigrationCategory()
    {
        return new ServiceCategoryViewModel
        {
            Id = "immigration",
            Title = "Immigration Law",
            Description = "Expert guidance through all UK immigration matters, from visa applications to citizenship and everything in between.",
            IconClass = "fas fa-globe",
            Url = "/services/immigration",
            Services = new List<ServiceItemViewModel>
            {
                new()
                {
                    Title = "Visa Applications",
                    Description = "Comprehensive support for all UK visa types including work, study, family, and visitor visas.",
                    IconClass = "fas fa-passport",
                    Url = "/services/immigration/visa-applications",
                    Features = new List<string> { "Work Visas", "Student Visas", "Family Visas", "Visitor Visas" },
                    IsFeatured = true
                },
                new()
                {
                    Title = "Indefinite Leave to Remain",
                    Description = "Secure your future in the UK with expert ILR application support and advice.",
                    IconClass = "fas fa-home",
                    Url = "/services/immigration/ilr",
                    Features = new List<string> { "Settlement Applications", "10-Year Route", "5-Year Route" }
                },
                new()
                {
                    Title = "British Citizenship",
                    Description = "Navigate the path to British citizenship with experienced legal guidance.",
                    IconClass = "fas fa-flag",
                    Url = "/services/immigration/citizenship",
                    Features = new List<string> { "Naturalisation", "Registration", "Dual Citizenship" }
                },
                new()
                {
                    Title = "Asylum & Refugee Claims",
                    Description = "Compassionate and professional support for asylum seekers and refugees.",
                    IconClass = "fas fa-shield-alt",
                    Url = "/services/immigration/asylum",
                    Features = new List<string> { "Asylum Applications", "Fresh Claims", "Appeals" },
                    IsFeatured = true
                },
                new()
                {
                    Title = "Spouse & Partner Visas",
                    Description = "Reunite with your loved ones through our specialist spouse visa service.",
                    IconClass = "fas fa-heart",
                    Url = "/services/immigration/spouse-visa",
                    Features = new List<string> { "Spouse Visas", "Fiancé Visas", "Unmarried Partner" }
                },
                new()
                {
                    Title = "Immigration Appeals",
                    Description = "Challenge unfair decisions with our experienced appeals team.",
                    IconClass = "fas fa-gavel",
                    Url = "/services/immigration/appeals",
                    Features = new List<string> { "First-Tier Tribunal", "Upper Tribunal", "Judicial Review" }
                },
                new()
                {
                    Title = "Human Rights Applications",
                    Description = "Protect your human rights with expert legal representation.",
                    IconClass = "fas fa-balance-scale",
                    Url = "/services/immigration/human-rights",
                    Features = new List<string> { "Article 8", "Private Life", "Family Life" }
                },
                new()
                {
                    Title = "EU Settlement Scheme",
                    Description = "Post-Brexit settlement support for EU, EEA and Swiss nationals.",
                    IconClass = "fas fa-star",
                    Url = "/services/immigration/eu-settlement",
                    Features = new List<string> { "Pre-Settled Status", "Settled Status", "Late Applications" }
                }
            }
        };
    }

    private static ServiceCategoryViewModel GetFamilyLawCategory()
    {
        return new ServiceCategoryViewModel
        {
            Id = "family-law",
            Title = "Family Law",
            Description = "Sensitive and professional legal support for all family matters, from divorce to child custody arrangements.",
            IconClass = "fas fa-users",
            Url = "/services/family-law",
            Services = new List<ServiceItemViewModel>
            {
                new()
                {
                    Title = "Divorce & Separation",
                    Description = "Compassionate guidance through divorce proceedings with minimal conflict.",
                    IconClass = "fas fa-file-contract",
                    Url = "/services/family-law/divorce",
                    Features = new List<string> { "No-Fault Divorce", "Financial Settlements", "Mediation" },
                    IsFeatured = true
                },
                new()
                {
                    Title = "Child Custody & Access",
                    Description = "Protecting children's best interests in custody and access arrangements.",
                    IconClass = "fas fa-child",
                    Url = "/services/family-law/child-custody",
                    Features = new List<string> { "Child Arrangements", "Contact Orders", "Residence" },
                    IsFeatured = true
                },
                new()
                {
                    Title = "Financial Settlements",
                    Description = "Fair division of assets and financial matters during separation.",
                    IconClass = "fas fa-coins",
                    Url = "/services/family-law/financial-settlements",
                    Features = new List<string> { "Asset Division", "Pensions", "Maintenance" }
                },
                new()
                {
                    Title = "Domestic Violence",
                    Description = "Urgent protection orders and support for domestic abuse victims.",
                    IconClass = "fas fa-hand-holding-heart",
                    Url = "/services/family-law/domestic-violence",
                    Features = new List<string> { "Non-Molestation Orders", "Occupation Orders", "Emergency Help" }
                },
                new()
                {
                    Title = "Pre-Nuptial Agreements",
                    Description = "Protect your assets with carefully drafted pre-nuptial agreements.",
                    IconClass = "fas fa-ring",
                    Url = "/services/family-law/prenuptial",
                    Features = new List<string> { "Pre-Nups", "Post-Nups", "Cohabitation Agreements" }
                },
                new()
                {
                    Title = "Children Act Proceedings",
                    Description = "Expert representation in complex children law cases.",
                    IconClass = "fas fa-balance-scale-right",
                    Url = "/services/family-law/children-act",
                    Features = new List<string> { "Care Proceedings", "Special Guardianship", "Adoption" }
                }
            }
        };
    }

    private static ServiceCategoryViewModel GetPersonalInjuryCategory()
    {
        return new ServiceCategoryViewModel
        {
            Id = "personal-injury",
            Title = "Personal Injury",
            Description = "No win, no fee representation for accident and injury claims. Get the compensation you deserve.",
            IconClass = "fas fa-user-injured",
            Url = "/services/personal-injury",
            Services = new List<ServiceItemViewModel>
            {
                new()
                {
                    Title = "Road Traffic Accidents",
                    Description = "Comprehensive support for car, motorcycle, and pedestrian accident claims.",
                    IconClass = "fas fa-car-crash",
                    Url = "/services/personal-injury/road-accidents",
                    Features = new List<string> { "Car Accidents", "Motorcycle Claims", "Pedestrian Injuries" },
                    IsFeatured = true
                },
                new()
                {
                    Title = "Workplace Injuries",
                    Description = "Claims for accidents and injuries sustained at work.",
                    IconClass = "fas fa-hard-hat",
                    Url = "/services/personal-injury/workplace",
                    Features = new List<string> { "Factory Accidents", "Construction Injuries", "Office Injuries" }
                },
                new()
                {
                    Title = "Medical Negligence",
                    Description = "Expert handling of clinical and medical negligence claims.",
                    IconClass = "fas fa-hospital",
                    Url = "/services/personal-injury/medical-negligence",
                    Features = new List<string> { "Misdiagnosis", "Surgical Errors", "Birth Injuries" },
                    IsFeatured = true
                },
                new()
                {
                    Title = "Slips, Trips & Falls",
                    Description = "Compensation for injuries caused by hazardous conditions.",
                    IconClass = "fas fa-exclamation-triangle",
                    Url = "/services/personal-injury/slips-trips",
                    Features = new List<string> { "Public Places", "Shops", "Pavements" }
                },
                new()
                {
                    Title = "Criminal Injuries",
                    Description = "CICA claims for victims of violent crime.",
                    IconClass = "fas fa-gavel",
                    Url = "/services/personal-injury/criminal-injuries",
                    Features = new List<string> { "Assault Claims", "CICA Applications", "Victim Support" }
                }
            }
        };
    }
}
