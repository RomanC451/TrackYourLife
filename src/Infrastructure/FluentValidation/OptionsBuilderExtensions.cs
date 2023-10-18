using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TrackYourLifeDotnet.Infrastructure.FluentValidation;

public static class OptionsBuilderFluentValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder
    )
        where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            provider => new FluentValidateOptions<TOptions>(provider, optionsBuilder.Name)
        );
        return optionsBuilder;
    }
}
