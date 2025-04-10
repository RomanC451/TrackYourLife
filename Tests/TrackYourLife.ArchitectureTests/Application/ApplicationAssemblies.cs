using System.Reflection;

namespace TrackYourLife.ArchitectureTests.Application;

internal static class ApplicationAssemblies
{
    public static Assembly[] Assemblies =>
        [
            Modules.Common.Application.AssemblyReference.Assembly,
            Modules.Nutrition.Application.AssemblyReference.Assembly,
            Modules.Users.Application.AssemblyReference.Assembly,
        ];
}
