using SadJam;
using UnityEngine;

namespace Game
{
    public class GameConfig_HorizontalMovement : DynamicExecutor
    {
        public enum DirectionType
        {
            Forward = 1,
            Backward = -1
        }

        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_HorizontallyMoveable Config { get; }

        [field: Space, SerializeField]
        public Transform Target { get; private set; }

        [field: Space, SerializeField]
        public DirectionType Direction { get; private set; } = DirectionType.Forward;

        protected override void DynamicExecutor_OnExecute()
        {
            Target.Translate(new(Config.HorizontalSpeed * ((float)Direction) * Time.smoothDeltaTime * -1f, 0));
        }
    }
}

