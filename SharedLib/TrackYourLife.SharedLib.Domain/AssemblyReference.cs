using System.Reflection;

namespace TrackYourLife.SharedLib.Domain;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
