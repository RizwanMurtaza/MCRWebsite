using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using MCRSolicitors.Theme.ContentParts;
using MCRSolicitors.Theme.ViewModels;

namespace MCRSolicitors.Theme.Drivers;

/// <summary>
/// Display driver for TestimonialItemPart - handles admin editing for testimonials
/// </summary>
public class TestimonialItemPartDisplayDriver : ContentPartDisplayDriver<TestimonialItemPart>
{
    public override IDisplayResult Display(TestimonialItemPart part, BuildPartDisplayContext context)
    {
        return Initialize<TestimonialItemPartViewModel>("TestimonialItemPart", model =>
        {
            model.Quote = part.Quote;
            model.ClientName = part.ClientName;
            model.CaseType = part.CaseType;
            model.ClientImageUrl = part.ClientImageUrl;
            model.Rating = part.Rating;
            model.DisplayOrder = part.DisplayOrder;
        }).Location("Detail", "Content:1");
    }

    public override IDisplayResult Edit(TestimonialItemPart part, BuildPartEditorContext context)
    {
        return Initialize<TestimonialItemPartViewModel>("TestimonialItemPart_Edit", model =>
        {
            model.Quote = part.Quote;
            model.ClientName = part.ClientName;
            model.CaseType = part.CaseType;
            model.ClientImageUrl = part.ClientImageUrl;
            model.Rating = part.Rating;
            model.DisplayOrder = part.DisplayOrder;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(TestimonialItemPart part, UpdatePartEditorContext context)
    {
        var viewModel = new TestimonialItemPartViewModel();
        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Quote = viewModel.Quote;
        part.ClientName = viewModel.ClientName;
        part.CaseType = viewModel.CaseType;
        part.ClientImageUrl = viewModel.ClientImageUrl;
        part.Rating = viewModel.Rating;
        part.DisplayOrder = viewModel.DisplayOrder;

        return Edit(part, context);
    }
}
