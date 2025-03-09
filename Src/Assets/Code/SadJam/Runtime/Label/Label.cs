using UnityEngine;
using System;

namespace SadJam
{
    [Serializable]
    public struct Label : IEquatable<Label>
    {
        public string text;
        public GUIStyle style;

        public Label(string text)
        {
            this.text = text;

            style = new GUIStyle();
            style.normal.textColor = Color.white;
        }

        public Label(string text, Color textColor)
        {
            this.text = text;

            style = new GUIStyle();
            style.normal.textColor = textColor;
        }

        public Label(string text, GUIStyle style)
        {
            this.text = text;
            this.style = style;
        }

        public override string ToString() => text;
        public override int GetHashCode() => text.GetHashCode() + style.GetHashCode();

        public static bool operator ==(Label lhs, Label rhs) => lhs.text == rhs.text && lhs.style == rhs.style;
        public static bool operator !=(Label lhs, Label rhs) => lhs.text != rhs.text || lhs.style != rhs.style;

        public override bool Equals(object obj) => obj is Label other && other == this;
        public bool Equals(Label other) => other == this;
    }
}
