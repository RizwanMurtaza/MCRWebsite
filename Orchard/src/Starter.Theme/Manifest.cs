using OrchardCore.DisplayManagement.Manifest;
using OrchardCore.Modules.Manifest;

[assembly: Theme(
    Name = "Starter Theme",
    Author = "MCR Solicitors",
    Website = "https://www.mcrsolicitors.co.uk",
    Version = "1.0.0",
    Description = "A fully configurable starter theme for professional websites. Configure branding, colors, and content via admin settings.",
    Tags = new[] { "Bootstrap", "Starter", "Configurable", "Multi-tenant" }
)]

[assembly: Feature(
    Id = "Starter.Theme",
    Name = "Starter Theme",
    Category = "Themes",
    Description = "Configurable theme with customizable branding, navigation, and sections"
)]
