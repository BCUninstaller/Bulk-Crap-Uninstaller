/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using Klocman.Localising;
using Klocman.Resources;

namespace Klocman.Tools
{
    public enum MachineType
    {
        [LocalisedName(typeof(CommonStrings), nameof(CommonStrings.Unknown))]
        Unknown,
        X86,
        X64,
        Ia64,
        ARM64
    }
}