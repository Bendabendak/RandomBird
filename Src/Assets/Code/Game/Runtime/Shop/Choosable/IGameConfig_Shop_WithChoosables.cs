using System.Collections.Generic;

namespace Game
{
    public interface IGameConfig_Shop_WithChoosables : IGameConfig_Shop
    {
        public new IEnumerable<IGameConfig_Shop_Choosable> Items { get; }

        public bool Choose(Statistics.Owner owner, IGameConfig_Shop_Choosable item);
        public IEnumerable<IGameConfig_Shop_Choosable> GetChosen(Statistics.Owner owner);
    }
}
