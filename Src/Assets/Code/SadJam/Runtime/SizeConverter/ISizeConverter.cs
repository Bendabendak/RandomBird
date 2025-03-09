/// <typeparam name="T">Output</typeparam>

namespace SadJam
{
    public interface ISizeConverter<T>
    {
        public abstract T Convert(object input, params string[] customData);
    }
}
