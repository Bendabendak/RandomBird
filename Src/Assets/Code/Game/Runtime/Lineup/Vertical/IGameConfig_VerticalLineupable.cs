using SadJam;

namespace Game
{
    public interface IGameConfig_VerticalLineupable : IGameConfig
    {
        public float SpaceBetween { get; }
        public bool WithBounds { get; }
    }
}

