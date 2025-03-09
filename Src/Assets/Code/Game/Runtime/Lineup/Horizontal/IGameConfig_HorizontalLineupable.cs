using SadJam;

namespace Game
{
    public interface IGameConfig_HorizontalLineupable : IGameConfig
    {
        public float SpaceBetween { get; }
        public bool WithBounds { get; }
    }
}
