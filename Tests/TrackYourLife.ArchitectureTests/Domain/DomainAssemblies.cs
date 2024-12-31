using System.Reflection;

namespace TrackYourLife.ArchitectureTests.Domain;

internal static class DomainAssemblies
{
    public static Assembly[] Assemblies =>
        [
            Modules.Nutrition.Domain.AssemblyReference.Assembly,
            Modules.Users.Domain.AssemblyReference.Assembly
        ];
}
