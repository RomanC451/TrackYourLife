using System.Reflection;

namespace TrackYourLifeDotnet.Persistence;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
