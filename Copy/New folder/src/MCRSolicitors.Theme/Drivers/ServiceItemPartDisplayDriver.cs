using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using MCRSolicitors.Theme.ContentParts;
using MCRSolicitors.Theme.ViewModels;

namespace MCRSolicitors.Theme.Drivers;

/// <summary>
/// Display driver for ServiceItemPart - handles admin editing for service items
/// </summary>
public class ServiceItemPartDisplayDriver : ContentPartDisplayDriver<ServiceItemPart>
{
    public override IDisplayResult Display(ServiceItemPart part, BuildPartDisplayContext context)
    {
        return Initialize<ServiceItemPartViewModel>("ServiceItemPart", model =>
        {
            model.Title = part.Title;
            model.Description = part.Description;
            model.IconClass = part.IconClass;
            model.Url = part.Url;
            model.CtaText = part.CtaText;
            model.DisplayOrder = part.DisplayOrder;
        }).Location("Detail", "Content:1");
    }

    public override IDisplayResult Edit(ServiceItemPart part, BuildPartEditorContext context)
    {
        return Initialize<ServiceItemPartViewModel>("ServiceItemPart_Edit", model =>
        {
            model.Title = part.Title;
            model.Description = part.Description;
            model.IconClass = part.IconClass;
            model.Url = part.Url;
            model.CtaText = part.CtaText;
            model.DisplayOrder = part.DisplayOrder;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(ServiceItemPart part, UpdatePartEditorContext context)
    {
        var viewModel = new ServiceItemPartViewModel();
        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Title = viewModel.Title;
        part.Description = viewModel.Description;
        part.IconClass = viewModel.IconClass;
        part.Url = viewModel.Url;
        part.CtaText = viewModel.CtaText;
        part.DisplayOrder = viewModel.DisplayOrder;

        return Edit(part, context);
    }
}
