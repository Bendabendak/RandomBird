using System;

namespace SadJamEditor
{
    [Serializable]
    public struct ReplaceWith : IEquatable<ReplaceWith>
    {
        public string replace;
        public string replaceWith;

        public ReplaceWith(string replace, string replaceWith)
        {
            this.replace = replace;
            this.replaceWith = replaceWith;
        }

        public override string ToString() => replace + "\n" + replaceWith;

        public override int GetHashCode() => replace.GetHashCode() + replaceWith.GetHashCode();

        public static bool operator ==(ReplaceWith lhs, ReplaceWith rhs) => 
            lhs.replace == rhs.replace && lhs.replaceWith == rhs.replaceWith;
        public static bool operator !=(ReplaceWith lhs, ReplaceWith rhs) =>
            lhs.replace != rhs.replace || lhs.replaceWith != rhs.replaceWith;

        public override bool Equals(object obj) => obj is ReplaceWith other && other == this;

        public bool Equals(ReplaceWith other) => other == this;
    }
}
