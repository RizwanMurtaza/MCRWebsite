using OrchardCore.DisplayManagement.Manifest;
using OrchardCore.Modules.Manifest;

[assembly: Theme(
    Name = "MCR Solicitors Theme",
    Author = "MCR Solicitors",
    Website = "https://www.mcrsolicitors.co.uk",
    Version = "1.0.0",
    Description = "Professional theme for MCR Solicitors - Manchester's trusted legal services provider",
    Tags = new[] { "Bootstrap", "Legal", "Solicitors", "Professional" }
)]

[assembly: Feature(
    Id = "MCRSolicitors.Theme",
    Name = "MCR Solicitors Theme",
    Category = "Themes",
    Description = "Professional theme for MCR Solicitors website with modular components",
    Dependencies = new[] { "OrchardCore.Contents", "OrchardCore.Title" }
)]
