using System;

namespace DecayEngine.TypingsGenerator.Model.Flags
{
    [Flags]
    public enum ClassFlags
    {
        None = 0,
        Static = 1,
        Struct = 2,
        Abstract = 4
    }
}