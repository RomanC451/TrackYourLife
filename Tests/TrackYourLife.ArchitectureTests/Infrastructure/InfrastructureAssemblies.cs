using System.Reflection;
using TrackYourLife.Modules.Nutrition.Presentation;

namespace TrackYourLife.ArchitectureTests.Infrastructure;

internal static class InfrastructureAssemblies
{
    public static Assembly[] Assemblies =>
        [AssemblyReference.Assembly, Modules.Users.Infrastructure.AssemblyReference.Assembly];
}
