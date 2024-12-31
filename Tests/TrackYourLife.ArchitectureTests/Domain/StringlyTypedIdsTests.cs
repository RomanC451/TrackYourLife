using System.Reflection;
using System.Text.Json.Serialization;
using FluentAssertions;
using NetArchTest.Rules;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.ArchitectureTests.Domain;

public class StringlyTypedIdsTests : BaseArchitectureTest
{
    private static IEnumerable<Type> IdsTypes =>
        Types
            .InAssemblies(DomainAssemblies.Assemblies)
            .That()
            .Inherit(typeof(StronglyTypedGuid<>))
            .GetTypes();

    [Fact]
    public void Ids_ShouldBeSealed() => ShouldBeSealed(IdsTypes);

    [Fact]
    public void Ids_ShouldBeDefinedAsRecords() => ShouldBeDefinedAsRecords(IdsTypes);

    [Fact]
    public void Ids_ShouldHaveIdPostfix() => ShouldHavePostfix(IdsTypes, "Id");

    [Fact]
    public void Ids_ShouldHaveStronglyTypedGuidJsonConverter() =>
        CustomTest(
            IdsTypes,
            (t) =>
            {
                var jsonConverterAttribute = t.GetCustomAttribute<JsonConverterAttribute>();

                if (
                    jsonConverterAttribute?.ConverterType
                    != typeof(StronglyTypedGuidJsonConverter<>).MakeGenericType(t)
                )
                {
                    return false;
                }
                return true;
            }
        );
}
