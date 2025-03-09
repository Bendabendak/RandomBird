using SadJam;
using System.Collections.Generic;

namespace Game
{
    public interface IGameConfig_Shop : IGameConfig
    {
        public IEnumerable<IGameConfig> Items { get; }
    }
}
