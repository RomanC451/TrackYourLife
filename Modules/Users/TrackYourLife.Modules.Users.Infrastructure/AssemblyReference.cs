using System.Reflection;

namespace TrackYourLife.Modules.Users.Infrastructure;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
