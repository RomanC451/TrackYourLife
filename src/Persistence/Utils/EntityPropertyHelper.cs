using Microsoft.EntityFrameworkCore.Metadata;
using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Infrastructure.Utils;

public static class EntityPropertyHelper
{
    public static IEnumerable<IProperty> GetEntityKeyProperties<T>(IEntityType? entityType)
        where T : class, IEntity
    {
        if (entityType == null)
        {
            throw new InvalidOperationException("Entity type not found in context");
        }

        var keyProperties = entityType.FindPrimaryKey()?.Properties;

        if (keyProperties == null)
        {
            throw new InvalidOperationException("Primary key not found for entity type");
        }

        var indexedProperties = entityType.GetIndexes().SelectMany(i => i.Properties).Distinct();
        return keyProperties.Concat(indexedProperties);
    }
}
