using System;

namespace SadJam
{
    public interface IGameConfig
    {
        public Action<string> OnBlend { get; set; }
        public Action<IGameConfig> OnReseted { get; set; }
    }
}
