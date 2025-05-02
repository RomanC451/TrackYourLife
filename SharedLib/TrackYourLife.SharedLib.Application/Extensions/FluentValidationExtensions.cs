using FluentValidation;

using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Application.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, StronglyTypedGuid<TId>> NotEmptyId<T, TId>(
        this IRuleBuilder<T, StronglyTypedGuid<TId>> ruleBuilder
    )
        where TId : StronglyTypedGuid<TId>, new()
    {
        return ruleBuilder
            .Must(id => id != null && id.Value != Guid.Empty)
            .WithMessage("The ID must not be empty.");
    }
}
