using System;

[Flags]
public enum DecayProjectBuildTarget
{
    Invalid = 0,
    Desktop = 1,
    Android = 2,
    Tool = 4,
    All = Desktop | Android | Tool
}