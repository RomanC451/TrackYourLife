using System.Reflection;

namespace TrackYourLife.ArchitectureTests.Infrastructure;

internal static class InfrastructureAssemblies
{
    public static Assembly[] Assemblies =>
        [
            Modules.Common.Infrastructure.AssemblyReference.Assembly,
            Modules.Users.Infrastructure.AssemblyReference.Assembly,
            Modules.Nutrition.Infrastructure.AssemblyReference.Assembly,
        ];
}
