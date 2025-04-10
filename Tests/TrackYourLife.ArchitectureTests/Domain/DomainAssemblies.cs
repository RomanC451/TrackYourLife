using System.Reflection;
using TrackYourLife.SharedLib.Domain;
using TrackYourLife.SharedLib.Infrastructure;
using TrackYourLife.SharedLib.Presentation;

namespace TrackYourLife.ArchitectureTests.Domain;

internal static class DomainAssemblies
{
    public static Assembly[] Assemblies =>
        [
            Modules.Common.Domain.AssemblyReference.Assembly,
            Modules.Nutrition.Domain.AssemblyReference.Assembly,
            Modules.Users.Domain.AssemblyReference.Assembly,
        ];
}
