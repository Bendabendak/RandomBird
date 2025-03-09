using SadJam;

namespace Game
{
    public interface IGameConfig_GameManager : IGameConfig
    {
        public int Seed { get; set; }
        public bool RandomSeed { get; }

        public int TargetFPS { get; }
    }
}
