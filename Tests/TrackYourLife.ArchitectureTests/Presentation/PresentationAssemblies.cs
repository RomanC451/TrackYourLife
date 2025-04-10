using System.Reflection;

namespace TrackYourLife.ArchitectureTests.Presentation;

internal static class PresentationAssemblies
{
    public static Assembly[] Assemblies =>
        [
            Modules.Common.Presentation.AssemblyReference.Assembly,
            Modules.Nutrition.Presentation.AssemblyReference.Assembly,
            Modules.Users.Presentation.AssemblyReference.Assembly,
        ];
}
