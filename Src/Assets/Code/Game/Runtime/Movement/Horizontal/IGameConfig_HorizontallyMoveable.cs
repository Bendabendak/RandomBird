using SadJam;

namespace Game
{
    public interface IGameConfig_HorizontallyMoveable : IGameConfig
    {
        public float HorizontalSpeed { get; }
    }
}
