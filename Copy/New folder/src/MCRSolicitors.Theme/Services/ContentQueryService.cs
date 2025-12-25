using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using YesSql;
using MCRSolicitors.Theme.ContentParts;
using MCRSolicitors.Theme.Models;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.Services;

/// <summary>
/// Service that queries content from Orchard Core CMS database
/// All content is created and managed from the admin panel
/// </summary>
public class ContentQueryService : IContentQueryService
{
    private readonly IContentManager _contentManager;
    private readonly ISession _session;

    public ContentQueryService(IContentManager contentManager, ISession session)
    {
        _contentManager = contentManager;
        _session = session;
    }

    public async Task<HeroViewModel?> GetHeroAsync(string? contentItemId = null)
    {
        ContentItem? contentItem;

        if (!string.IsNullOrEmpty(contentItemId))
        {
            contentItem = await _contentManager.GetAsync(contentItemId);
        }
        else
        {
            // Get the first published HeroSection
            contentItem = await _session
                .Query<ContentItem, ContentItemIndex>()
                .Where(x => x.ContentType == "HeroSection" && x.Published)
                .FirstOrDefaultAsync();
        }

        if (contentItem == null)
            return null;

        var heroPart = contentItem.As<HeroPart>();
        if (heroPart == null)
            return null;

        return new HeroViewModel
        {
            Title = heroPart.Title,
            Subtitle = heroPart.Subtitle,
            Description = heroPart.Description,
            BackgroundImageUrl = heroPart.BackgroundImageUrl,
            ShowTrustIndicators = heroPart.ShowTrustIndicators,
            PrimaryButton = new HeroCtaButton
            {
                Text = heroPart.PrimaryButtonText,
                Url = heroPart.PrimaryButtonUrl,
                Style = ButtonStyle.Primary
            },
            SecondaryButton = !string.IsNullOrEmpty(heroPart.SecondaryButtonText)
                ? new HeroCtaButton
                {
                    Text = heroPart.SecondaryButtonText!,
                    Url = heroPart.SecondaryButtonUrl ?? "#",
                    Style = ButtonStyle.Outline
                }
                : null
        };
    }

    public async Task<ServicesSectionViewModel> GetServicesSectionAsync()
    {
        var services = await GetServicesAsync();

        return new ServicesSectionViewModel
        {
            SectionTitle = "Our Legal Services",
            SectionSubtitle = "Expert Legal Representation",
            Services = services,
            ColumnsPerRow = 3,
            CtaText = "View All Services",
            CtaUrl = "/services"
        };
    }

    public async Task<List<ServiceCardViewModel>> GetServicesAsync()
    {
        var contentItems = await _session
            .Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "ServiceItem" && x.Published)
            .ListAsync();

        var services = new List<ServiceCardViewModel>();

        foreach (var item in contentItems)
        {
            var part = item.As<ServiceItemPart>();
            if (part != null)
            {
                services.Add(new ServiceCardViewModel
                {
                    Title = part.Title,
                    Description = part.Description,
                    IconClass = part.IconClass,
                    Url = part.Url,
                    CtaText = part.CtaText
                });
            }
        }

        // Sort by display order
        return services.OrderBy(s => s.Title).ToList();
    }

    public async Task<StatsSectionViewModel> GetStatsSectionAsync()
    {
        var stats = await GetStatsAsync();

        return new StatsSectionViewModel
        {
            Title = "Trusted by Thousands",
            Stats = stats,
            Layout = StatsLayout.Horizontal
        };
    }

    public async Task<List<StatItem>> GetStatsAsync()
    {
        var contentItems = await _session
            .Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "StatItem" && x.Published)
            .ListAsync();

        var stats = new List<StatItem>();

        foreach (var item in contentItems)
        {
            var part = item.As<StatItemPart>();
            if (part != null)
            {
                stats.Add(new StatItem
                {
                    Value = part.Value,
                    Suffix = part.Suffix ?? string.Empty,
                    Prefix = part.Prefix ?? string.Empty,
                    Label = part.Label,
                    IconClass = part.IconClass
                });
            }
        }

        return stats.ToList();
    }

    public async Task<FeatureSectionViewModel> GetFeaturesSectionAsync()
    {
        var features = await GetFeaturesAsync();

        return new FeatureSectionViewModel
        {
            SectionTitle = "Why Choose MCR Solicitors",
            SectionSubtitle = "Your Success Is Our Priority",
            Features = features,
            Layout = FeatureLayout.Grid
        };
    }

    public async Task<List<FeatureItem>> GetFeaturesAsync()
    {
        var contentItems = await _session
            .Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "FeatureItem" && x.Published)
            .ListAsync();

        var features = new List<FeatureItem>();

        foreach (var item in contentItems)
        {
            var part = item.As<FeatureItemPart>();
            if (part != null)
            {
                features.Add(new FeatureItem
                {
                    Title = part.Title,
                    Description = part.Description,
                    IconClass = part.IconClass
                });
            }
        }

        return features.ToList();
    }

    public async Task<TestimonialSectionViewModel> GetTestimonialsSectionAsync()
    {
        var testimonials = await GetTestimonialsAsync();

        return new TestimonialSectionViewModel
        {
            SectionTitle = "What Our Clients Say",
            SectionSubtitle = "Trusted by Thousands Across Manchester",
            Testimonials = testimonials,
            Layout = TestimonialLayout.Grid
        };
    }

    public async Task<List<TestimonialItem>> GetTestimonialsAsync()
    {
        var contentItems = await _session
            .Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "Testimonial" && x.Published)
            .ListAsync();

        var testimonials = new List<TestimonialItem>();

        foreach (var item in contentItems)
        {
            var part = item.As<TestimonialItemPart>();
            if (part != null)
            {
                testimonials.Add(new TestimonialItem
                {
                    Quote = part.Quote,
                    ClientName = part.ClientName,
                    CaseType = part.CaseType,
                    ClientImage = part.ClientImageUrl,
                    Rating = part.Rating
                });
            }
        }

        return testimonials.ToList();
    }

    public async Task<CtaSectionViewModel?> GetCtaSectionAsync(string? contentItemId = null)
    {
        ContentItem? contentItem;

        if (!string.IsNullOrEmpty(contentItemId))
        {
            contentItem = await _contentManager.GetAsync(contentItemId);
        }
        else
        {
            contentItem = await _session
                .Query<ContentItem, ContentItemIndex>()
                .Where(x => x.ContentType == "CtaSection" && x.Published)
                .FirstOrDefaultAsync();
        }

        if (contentItem == null)
            return null;

        var ctaPart = contentItem.As<CtaPart>();
        if (ctaPart == null)
            return null;

        return new CtaSectionViewModel
        {
            Subtitle = ctaPart.Subtitle,
            Title = ctaPart.Title,
            Description = ctaPart.Description ?? string.Empty,
            PrimaryButton = new HeroCtaButton
            {
                Text = ctaPart.PrimaryButtonText,
                Url = ctaPart.PrimaryButtonUrl,
                Style = ButtonStyle.Primary
            },
            SecondaryButton = !string.IsNullOrEmpty(ctaPart.SecondaryButtonText)
                ? new HeroCtaButton
                {
                    Text = ctaPart.SecondaryButtonText!,
                    Url = ctaPart.SecondaryButtonUrl ?? "#",
                    Style = ButtonStyle.Outline
                }
                : null,
            ShowContactInfo = ctaPart.ShowContactInfo,
            PhoneNumber = ctaPart.PhoneNumber,
            Email = ctaPart.Email
        };
    }
}
