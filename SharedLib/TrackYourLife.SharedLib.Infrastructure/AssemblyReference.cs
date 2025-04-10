using System.Reflection;

namespace TrackYourLife.SharedLib.Infrastructure;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
