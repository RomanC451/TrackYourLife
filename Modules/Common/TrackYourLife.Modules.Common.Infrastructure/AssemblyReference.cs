﻿using System.Reflection;

namespace TrackYourLife.Modules.Common.Infrastructure;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
