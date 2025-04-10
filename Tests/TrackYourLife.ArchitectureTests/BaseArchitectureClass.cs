using System.Reflection;
using FluentAssertions;
using TrackYourLife.SharedLib.Domain;
using TrackYourLife.SharedLib.Infrastructure;
using TrackYourLife.SharedLib.Presentation;

namespace TrackYourLife.ArchitectureTests;

public class BaseArchitectureTest
{
    protected BaseArchitectureTest() { }

    protected static readonly Assembly[] CommonAssemblies =
    [
        Modules.Common.Domain.AssemblyReference.Assembly,
        Modules.Common.Application.AssemblyReference.Assembly,
        Modules.Common.Infrastructure.AssemblyReference.Assembly,
        Modules.Common.Presentation.AssemblyReference.Assembly,
    ];

    protected static readonly Assembly[] NutritionAssemblies =
    [
        Modules.Nutrition.Domain.AssemblyReference.Assembly,
        Modules.Nutrition.Application.AssemblyReference.Assembly,
        Modules.Nutrition.Infrastructure.AssemblyReference.Assembly,
        Modules.Nutrition.Presentation.AssemblyReference.Assembly,
    ];

    protected static readonly Assembly[] UsersAssemblies =
    [
        Modules.Users.Domain.AssemblyReference.Assembly,
        Modules.Users.Application.AssemblyReference.Assembly,
        Modules.Users.Infrastructure.AssemblyReference.Assembly,
        Modules.Users.Presentation.AssemblyReference.Assembly,
    ];

    protected static readonly Assembly[][] ModulesAssemblies =
    {
        CommonAssemblies,
        NutritionAssemblies,
        UsersAssemblies,
    };

    public static void ShouldBeSealed(IEnumerable<Type> types)
    {
        types.Should().NotBeEmpty("There should be types to check");
        var failingTypes = new List<Type>();

        foreach (var type in types)
        {
            if (!type.IsSealed)
            {
                failingTypes.Add(type);
            }
        }

        failingTypes.Should().BeEmpty();
    }

    public static void ShouldBeInternal(IEnumerable<Type> types)
    {
        types.Should().NotBeEmpty("There should be types to check");
        var failingTypes = new List<Type>();

        foreach (var t in types)
        {
            if (!t.IsNotPublic)
            {
                failingTypes.Add(t);
            }
        }

        failingTypes.Should().BeEmpty();
    }

    public static void ShouldBeDefinedAsRecords(IEnumerable<Type> types)
    {
        types.Should().NotBeEmpty("There should be types to check");
        var failingTypes = new List<Type>();

        foreach (var type in types)
        {
            var isRecord = Array.Exists(type.GetMethods(), m => m.Name == "<Clone>$");

            if (!isRecord)
            {
                failingTypes.Add(type);
            }
        }
        failingTypes.Should().BeEmpty();
    }

    public static void ShouldHavePostfix(IEnumerable<Type> types, string postfix)
    {
        types.Should().NotBeEmpty("There should be types to check");
        var failingTypes = new List<Type>();

        foreach (var type in types)
        {
            if (!type.Name.EndsWith(postfix))
            {
                failingTypes.Add(type);
            }
        }

        failingTypes.Should().BeEmpty();
    }

    public static void CustomTest(IEnumerable<Type> types, Func<Type, bool> condition)
    {
        types.Should().NotBeEmpty("There should be types to check");
        var failingTypes = new List<Type>();

        foreach (var type in types)
        {
            if (!condition(type))
            {
                failingTypes.Add(type);
            }
        }

        failingTypes.Should().BeEmpty();
    }
}
