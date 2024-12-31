using System.Reflection;
using FluentAssertions;

namespace TrackYourLife.ArchitectureTests;

public class BaseArchitectureTest
{
    protected BaseArchitectureTest() { }

    protected static Assembly[] AllAssemblies =
    [
        //Common
        Modules.Common.Application.AssemblyReference.Assembly,
        Modules.Common.Domain.AssemblyReference.Assembly,
        Modules.Common.Infrastructure.AssemblyReference.Assembly,
        Modules.Common.Presentation.AssemblyReference.Assembly,
        //Nutrition
        Modules.Nutrition.Application.AssemblyReference.Assembly,
        Modules.Nutrition.Domain.AssemblyReference.Assembly,
        Modules.Nutrition.Infrastructure.AssemblyReference.Assembly,
        Modules.Nutrition.Presentation.AssemblyReference.Assembly,
        //Users
        Modules.Users.Application.AssemblyReference.Assembly,
        Modules.Users.Domain.AssemblyReference.Assembly,
        Modules.Users.Infrastructure.AssemblyReference.Assembly,
        Modules.Users.Presentation.AssemblyReference.Assembly,
    ];

    public static void ShouldBeSealed(IEnumerable<Type> types)
    {
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

    public static void ShouldBeInternal(IEnumerable<Type> type)
    {
        var failingTypes = new List<Type>();

        foreach (var t in type)
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
        var failingTypes = new List<Type>();

        foreach (var type in types)
        {
            var isRecord = Array.Exists(type.GetMethods(),m => m.Name == "<Clone>$");

            if (!isRecord)
            {
                failingTypes.Add(type);
            }
        }
        failingTypes.Should().BeEmpty();
    }

    public static void ShouldHavePostfix(IEnumerable<Type> types, string postfix)
    {
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
