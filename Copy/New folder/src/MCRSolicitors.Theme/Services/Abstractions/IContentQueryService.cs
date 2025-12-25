using MCRSolicitors.Theme.Models;

namespace MCRSolicitors.Theme.Services.Abstractions;

/// <summary>
/// Service for querying content from Orchard Core CMS
/// All content is managed from the admin panel
/// </summary>
public interface IContentQueryService
{
    /// <summary>
    /// Get the hero section content
    /// </summary>
    Task<HeroViewModel?> GetHeroAsync(string? contentItemId = null);

    /// <summary>
    /// Get all services for the services section
    /// </summary>
    Task<ServicesSectionViewModel> GetServicesSectionAsync();

    /// <summary>
    /// Get all service items
    /// </summary>
    Task<List<ServiceCardViewModel>> GetServicesAsync();

    /// <summary>
    /// Get stats section content
    /// </summary>
    Task<StatsSectionViewModel> GetStatsSectionAsync();

    /// <summary>
    /// Get all stat items
    /// </summary>
    Task<List<StatItem>> GetStatsAsync();

    /// <summary>
    /// Get features section content
    /// </summary>
    Task<FeatureSectionViewModel> GetFeaturesSectionAsync();

    /// <summary>
    /// Get all feature items
    /// </summary>
    Task<List<FeatureItem>> GetFeaturesAsync();

    /// <summary>
    /// Get testimonials section content
    /// </summary>
    Task<TestimonialSectionViewModel> GetTestimonialsSectionAsync();

    /// <summary>
    /// Get all testimonial items
    /// </summary>
    Task<List<TestimonialItem>> GetTestimonialsAsync();

    /// <summary>
    /// Get CTA section content
    /// </summary>
    Task<CtaSectionViewModel?> GetCtaSectionAsync(string? contentItemId = null);
}
