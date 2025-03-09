using SadJam;

namespace SadJamEditor
{
    public enum ConversionType
    {
        None,
        Normal,
        Split
    }

    /// <typeparam name="T">Input</typeparam>
    public abstract class ConversionType<T> : StaticBehaviour
    {
        public abstract ConversionType conversionType { get; }

        /// <summary>
        /// The ending must be unique!
        /// </summary>
        public abstract string uniqueAddress { get; }

        public abstract object OnSelection(T input);
    }
}
