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

internal static class EventHandlerApplicationAssemblies
{
    public static Assembly[] Assemblies =>
        [
            Modules.Common.Application.AssemblyReference.Assembly,
            Modules.Nutrition.Application.AssemblyReference.Assembly,
            Modules.Trainings.Application.AssemblyReference.Assembly,
            Modules.Users.Application.AssemblyReference.Assembly,
            Modules.Youtube.Application.AssemblyReference.Assembly,
        ];
}
