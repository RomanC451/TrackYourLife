using System.Data;
using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.ArchitectureTests.Domain;

public class EntitiesTests : BaseArchitectureTest
{
    private static IEnumerable<Type> EntitiesTypes =>
        Types.InAssemblies(DomainAssemblies.Assemblies).That().Inherit(typeof(Entity<>)).GetTypes();

    [Fact]
    public void Entities_ShouldBeSealed() => ShouldBeSealed(EntitiesTypes);

    [Fact]
    public void Entities_ShouldHavePrivateConstructors() =>
        CustomTest(
            EntitiesTypes,
            (t) =>
            {
                var constructors = t.GetConstructors(
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

                if (constructors.Count(c => c.IsPrivate) != 2)
                {
                    return false;
                }
                return true;
            }
        );

    [Fact]
    public void Entities_ShouldHaveStaticCreateMethodReturningResult() =>
        CustomTest(
            EntitiesTypes,
            (t) =>
            {
                var resultType = typeof(Result<>).MakeGenericType(t);
                var createMethod = t.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);

                if (createMethod is null || createMethod.ReturnType != resultType)
                {
                    return false;
                }
                return true;
            }
        );

    [Fact]
    public void EntitiesFields_ShouldHavePrivateSettersOrNoSetters() =>
        CustomTest(
            EntitiesTypes,
            (t) =>
            {
                var properties = t.GetProperties();

                foreach (var property in properties)
                {
                    if (property.SetMethod is not null && !property.SetMethod.IsPrivate)
                    {
                        return false;
                    }
                }

                return true;
            }
        );

    [Fact]
    public void EntitiesUpdateMethods_ShouldReturnResults() =>
        CustomTest(
            EntitiesTypes,
            (t) =>
            {
                var updateMethods = t.GetMethods().Where(m => m.Name.StartsWith("Update"));

                foreach (var method in updateMethods)
                {
                    if (method.ReturnType != typeof(Result))
                    {
                        return false;
                    }
                }

                return true;
            }
        );
}
