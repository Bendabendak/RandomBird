using SadJam;
using UnityEngine;

namespace Game
{
    public class Progress_Increase : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Progress_Config Config { get; }

        [field: Space, SerializeField]
        public int Increase { get; private set; }
        [field: Space, SerializeField]
        public Vector2Int Limit { get; private set; }
        [field: SerializeField]
        public bool NoLimit { get; private set; } = true;

        protected override void DynamicExecutor_OnExecute()
        {
            if (NoLimit)
            {
                Config.IncreaseProgress(Increase);
            }
            else
            {
                Config.IncreaseProgress(Mathf.Clamp(Increase, Limit.x, Limit.y));
            }
        }
    }
}
