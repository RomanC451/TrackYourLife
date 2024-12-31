using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Infrastructure.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder UseStronglyTypedIdsValueConverter(this ModelBuilder modelBuilder)
    {
        modelBuilder
            .Model.GetEntityTypes()
            .ToList()
            .ForEach(entityType =>
            {
                entityType
                    .GetProperties()
                    .ToList()
                    .ForEach(property =>
                    {
                        if (property.ClrType.BaseType?.IsGenericType is false)
                            return;

                        if (
                            property.ClrType.BaseType?.GetGenericTypeDefinition()
                            == typeof(StronglyTypedGuid<>)
                        )
                        {
                            var converterType =
                                typeof(StronglyTypedIdValueConverter<>).MakeGenericType(
                                    property.ClrType
                                );
                            var converter =
                                Activator.CreateInstance(converterType) as ValueConverter;
                            property.SetValueConverter(converter);
                        }
                    });
            });

        return modelBuilder;
    }
}
