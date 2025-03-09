using System.Collections.Generic;
using SadJam;

namespace Game
{
    public interface IGameConfig_Shop_Buyable : IGameConfig
    {
        public string Id { get; }
        public int Price { get; }
    }
}
