using System.Reflection;

namespace TrackYourLife.Common.Persistence;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
