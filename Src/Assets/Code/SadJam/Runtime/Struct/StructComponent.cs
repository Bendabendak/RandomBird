namespace SadJam
{
    public abstract class StructComponent<T> : SadJam.Component where T : struct
    {
        public virtual T Size { get; set; }

        public static implicit operator T(StructComponent<T> size) => size.Size;

        public override string ToString() => Size.ToString();
    }
}
