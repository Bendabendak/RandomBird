using System;
using System.Globalization;

namespace SadJam
{
    public struct Direction2 : IEquatable<Direction2>
    {
        public sbyte value { get; set; }

        public static Direction2 forward => new Direction2() { value = 1 };
        public static Direction2 backward => new Direction2() { value = -1 };
        public static Direction2 up => forward;
        public static Direction2 down => backward;

        public override int GetHashCode() => value;
        public override string ToString() => ToString(DirectionType.Horizontal);

        public string ToString(DirectionType format)
        {
            switch (format)
            {
                default:
                case DirectionType.Horizontal:
                    return value >= 0 ? "forward" : "backward";
                case DirectionType.Vertical:
                    return value >= 0 ? "up" : "down";
            }
        }

        public static implicit operator sbyte(Direction2 o) => o.value;

        public static bool operator ==(Direction2 lhs, Direction2 rhs) => lhs.value == rhs.value;
        public static bool operator !=(Direction2 lhs, Direction2 rhs) => lhs.value != rhs.value;

        public override bool Equals(object obj) => obj is Direction2 other && other == this;
        public bool Equals(Direction2 other) => other == this;

        public static sbyte Parse(string s, IFormatProvider provider) => sbyte.Parse(s, provider);
        public static sbyte Parse(string s, NumberStyles style, IFormatProvider provider) => sbyte.Parse(s, style, provider);
        public static sbyte Parse(string s, NumberStyles style) => sbyte.Parse(s, style);
        public static sbyte Parse(string s) => sbyte.Parse(s);
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out sbyte result) =>
            sbyte.TryParse(s, style, provider, out result);
        public static bool TryParse(string s, out sbyte result) => sbyte.TryParse(s, out result);
    }
}
