using System;

namespace SadJam
{
    [Flags]
    public enum Vector2Freeze
    {
        None = 0,
        X = 1 << 0,
        Y = 1 << 1
    }
}
