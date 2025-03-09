using SadJam;

namespace Game
{
    public class Game_Load : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false, 
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_GameManager Config { get; }

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            GameManager.ChangeConfig(Config);
        }
    }
}