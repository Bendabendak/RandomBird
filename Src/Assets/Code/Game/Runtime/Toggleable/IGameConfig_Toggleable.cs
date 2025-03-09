using SadJam;

namespace Game
{
    public interface IGameConfig_Toggleable : IGameConfig
    {
        public bool Enabled { get; }
    }
}