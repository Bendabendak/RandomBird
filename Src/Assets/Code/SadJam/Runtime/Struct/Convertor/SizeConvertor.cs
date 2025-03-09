using System.Collections.Generic;

namespace SadJam
{
    /// <typeparam name="T">Output</typeparam>
    public abstract class SizeConvertor<T> : StaticBehaviour where T : struct
    {
        public virtual Label ConversionLabel { get; }

        public abstract T ConvertSize(List<UnityEngine.Component> inputs, string[] customData);
    }
}
