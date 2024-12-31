using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.DeleteRecipeDiary;

internal class DeleteRecipeDiaryCommandValidator : AbstractValidator<DeleteRecipeDiaryCommand>
{
    public DeleteRecipeDiaryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmptyId();
    }
}
