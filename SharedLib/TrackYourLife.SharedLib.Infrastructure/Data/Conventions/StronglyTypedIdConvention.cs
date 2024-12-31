using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Infrastructure.Data.Conventions;

public class StronglyTypedIdConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context
    )
    {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            entityType
                .GetProperties()
                .Where(p => p.ClrType.IsSubclassOf(typeof(StronglyTypedGuid<>)))
                .ToList()
                .ForEach(p =>
                {
                    Type converterType = typeof(StronglyTypedIdValueConverter<>).MakeGenericType(
                        p.ClrType
                    );

                    var converter = Activator.CreateInstance(converterType) as ValueConverter;

                    p.SetValueConverter(converter);
                });
        }
    }
}
