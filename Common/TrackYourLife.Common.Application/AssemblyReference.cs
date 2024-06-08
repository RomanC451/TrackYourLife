using System.Reflection;

namespace TrackYourLife.Common.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
