using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using MCRSolicitors.Theme.ContentParts;
using MCRSolicitors.Theme.ViewModels;

namespace MCRSolicitors.Theme.Drivers;

/// <summary>
/// Display driver for FeatureItemPart - handles admin editing for feature items
/// </summary>
public class FeatureItemPartDisplayDriver : ContentPartDisplayDriver<FeatureItemPart>
{
    public override IDisplayResult Display(FeatureItemPart part, BuildPartDisplayContext context)
    {
        return Initialize<FeatureItemPartViewModel>("FeatureItemPart", model =>
        {
            model.Title = part.Title;
            model.Description = part.Description;
            model.IconClass = part.IconClass;
            model.DisplayOrder = part.DisplayOrder;
        }).Location("Detail", "Content:1");
    }

    public override IDisplayResult Edit(FeatureItemPart part, BuildPartEditorContext context)
    {
        return Initialize<FeatureItemPartViewModel>("FeatureItemPart_Edit", model =>
        {
            model.Title = part.Title;
            model.Description = part.Description;
            model.IconClass = part.IconClass;
            model.DisplayOrder = part.DisplayOrder;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(FeatureItemPart part, UpdatePartEditorContext context)
    {
        var viewModel = new FeatureItemPartViewModel();
        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Title = viewModel.Title;
        part.Description = viewModel.Description;
        part.IconClass = viewModel.IconClass;
        part.DisplayOrder = viewModel.DisplayOrder;

        return Edit(part, context);
    }
}
