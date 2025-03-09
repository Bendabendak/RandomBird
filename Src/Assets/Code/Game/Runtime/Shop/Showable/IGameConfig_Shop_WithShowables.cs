using System.Collections.Generic;

namespace Game
{
    public interface IGameConfig_Shop_WithShowables : IGameConfig_Shop
    {
        public new IEnumerable<IGameConfig_Shop_Showable> Items { get; }
    }
}
