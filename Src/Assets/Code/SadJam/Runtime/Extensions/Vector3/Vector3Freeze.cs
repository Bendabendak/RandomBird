using System;

namespace SadJam
{
    [Flags]
    public enum Vector3Freeze
    {
        None = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2
    }
}
