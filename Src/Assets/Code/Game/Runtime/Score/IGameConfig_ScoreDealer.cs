using SadJam;

namespace Game
{
    public interface IGameConfig_ScoreDealer : IGameConfig
    {
        public int ScoreToDeal { get; }
    }
}
