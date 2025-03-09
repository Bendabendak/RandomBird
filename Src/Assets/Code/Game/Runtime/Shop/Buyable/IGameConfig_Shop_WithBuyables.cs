using System.Collections.Generic;

namespace Game
{
    public interface IGameConfig_Shop_WithBuyables : IGameConfig_Shop
    {
        public new IEnumerable<IGameConfig_Shop_Buyable> Items { get; }

        public bool Buy(Statistics.Owner owner, IGameConfig_Shop_Buyable item);
        public IEnumerable<IGameConfig_Shop_Buyable> GetBought(Statistics.Owner owner);
    }
}
