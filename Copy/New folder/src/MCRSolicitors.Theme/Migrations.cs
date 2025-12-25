using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using MCRSolicitors.Theme.ContentParts;

namespace MCRSolicitors.Theme;

/// <summary>
/// Migrations to register Content Parts and Content Types
/// These make content editable from the Orchard Core admin panel
/// </summary>
public class Migrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public Migrations(IContentDefinitionManager contentDefinitionManager)
    {
        _contentDefinitionManager = contentDefinitionManager;
    }

    public async Task<int> CreateAsync()
    {
        // =====================================================
        // HOMEPAGE CONTENT PARTS
        // =====================================================

        // Register HeroPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(HeroPart), part => part
            .Attachable()
            .WithDescription("A hero section with title, description, and call-to-action buttons")
        );

        // Register ServicesPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(ServicesPart), part => part
            .Attachable()
            .WithDescription("A services section container")
        );

        // Register ServiceItemPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(ServiceItemPart), part => part
            .Attachable()
            .WithDescription("An individual service item")
        );

        // Register StatsPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(StatsPart), part => part
            .Attachable()
            .WithDescription("A statistics section container")
        );

        // Register StatItemPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(StatItemPart), part => part
            .Attachable()
            .WithDescription("An individual statistic item")
        );

        // Register FeaturesPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(FeaturesPart), part => part
            .Attachable()
            .WithDescription("A features/why choose us section")
        );

        // Register FeatureItemPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(FeatureItemPart), part => part
            .Attachable()
            .WithDescription("An individual feature item")
        );

        // Register TestimonialsPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(TestimonialsPart), part => part
            .Attachable()
            .WithDescription("A testimonials section container")
        );

        // Register TestimonialItemPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(TestimonialItemPart), part => part
            .Attachable()
            .WithDescription("An individual testimonial")
        );

        // Register CtaPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(CtaPart), part => part
            .Attachable()
            .WithDescription("A call-to-action section")
        );

        // Register SiteSettingsPart for Site Settings
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(SiteSettingsPart), part => part
            .Attachable()
            .WithDescription("Site-wide settings including branding and contact info")
        );

        // =====================================================
        // PAGE CONTENT PARTS
        // =====================================================

        // Register ServiceDetailPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(ServiceDetailPart), part => part
            .Attachable()
            .WithDescription("Detailed service page content")
        );

        // Register ServiceContentSectionPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(ServiceContentSectionPart), part => part
            .Attachable()
            .WithDescription("Content section within a service page")
        );

        // Register FaqItemPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(FaqItemPart), part => part
            .Attachable()
            .WithDescription("Frequently asked question")
        );

        // Register TeamMemberPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(TeamMemberPart), part => part
            .Attachable()
            .WithDescription("Team member profile")
        );

        // Register CompanyValuePart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(CompanyValuePart), part => part
            .Attachable()
            .WithDescription("Company value or core principle")
        );

        // Register AccreditationPart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(AccreditationPart), part => part
            .Attachable()
            .WithDescription("Accreditation or certification badge")
        );

        // Register OfficePart
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(OfficePart), part => part
            .Attachable()
            .WithDescription("Office location details")
        );

        // =====================================================
        // CONTENT TYPES - Homepage Sections
        // =====================================================

        // Hero Section Content Type
        await _contentDefinitionManager.AlterTypeDefinitionAsync("HeroSection", type => type
            .DisplayedAs("Hero Section")
            .Creatable()
            .Listable()
            .WithPart(nameof(HeroPart))
        );

        // Service Item Content Type (for homepage cards)
        await _contentDefinitionManager.AlterTypeDefinitionAsync("ServiceItem", type => type
            .DisplayedAs("Service Card")
            .Creatable()
            .Listable()
            .Draftable()
            .WithPart(nameof(ServiceItemPart))
        );

        // Stat Item Content Type
        await _contentDefinitionManager.AlterTypeDefinitionAsync("StatItem", type => type
            .DisplayedAs("Statistic")
            .Creatable()
            .Listable()
            .WithPart(nameof(StatItemPart))
        );

        // Feature Item Content Type
        await _contentDefinitionManager.AlterTypeDefinitionAsync("FeatureItem", type => type
            .DisplayedAs("Feature")
            .Creatable()
            .Listable()
            .WithPart(nameof(FeatureItemPart))
        );

        // Testimonial Content Type
        await _contentDefinitionManager.AlterTypeDefinitionAsync("Testimonial", type => type
            .DisplayedAs("Testimonial")
            .Creatable()
            .Listable()
            .Draftable()
            .WithPart(nameof(TestimonialItemPart))
        );

        // CTA Section Content Type
        await _contentDefinitionManager.AlterTypeDefinitionAsync("CtaSection", type => type
            .DisplayedAs("Call to Action Section")
            .Creatable()
            .Listable()
            .WithPart(nameof(CtaPart))
        );

        // =====================================================
        // CONTENT TYPES - Services Pages
        // =====================================================

        // Service Detail Page Content Type
        await _contentDefinitionManager.AlterTypeDefinitionAsync("ServicePage", type => type
            .DisplayedAs("Service Page")
            .Creatable()
            .Listable()
            .Draftable()
            .Securable()
            .WithPart(nameof(ServiceDetailPart))
        );

        // Service Content Section
        await _contentDefinitionManager.AlterTypeDefinitionAsync("ServiceSection", type => type
            .DisplayedAs("Service Content Section")
            .Creatable()
            .Listable()
            .WithPart(nameof(ServiceContentSectionPart))
        );

        // FAQ Item
        await _contentDefinitionManager.AlterTypeDefinitionAsync("FaqItem", type => type
            .DisplayedAs("FAQ")
            .Creatable()
            .Listable()
            .WithPart(nameof(FaqItemPart))
        );

        // =====================================================
        // CONTENT TYPES - About Page
        // =====================================================

        // Team Member
        await _contentDefinitionManager.AlterTypeDefinitionAsync("TeamMember", type => type
            .DisplayedAs("Team Member")
            .Creatable()
            .Listable()
            .Draftable()
            .WithPart(nameof(TeamMemberPart))
        );

        // Company Value
        await _contentDefinitionManager.AlterTypeDefinitionAsync("CompanyValue", type => type
            .DisplayedAs("Company Value")
            .Creatable()
            .Listable()
            .WithPart(nameof(CompanyValuePart))
        );

        // Accreditation
        await _contentDefinitionManager.AlterTypeDefinitionAsync("Accreditation", type => type
            .DisplayedAs("Accreditation")
            .Creatable()
            .Listable()
            .WithPart(nameof(AccreditationPart))
        );

        // =====================================================
        // CONTENT TYPES - Contact Page
        // =====================================================

        // Office Location
        await _contentDefinitionManager.AlterTypeDefinitionAsync("Office", type => type
            .DisplayedAs("Office Location")
            .Creatable()
            .Listable()
            .WithPart(nameof(OfficePart))
        );

        return 1;
    }
}
