using SadJam;
using UnityEngine;

namespace Game
{
    public class LineupChildrenVertically_ByElementPosition : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_VerticalLineupable Config { get; }

        [field: Space, SerializeField]
        public LineupChildrenVertically.LineUpType LineUp { get; private set; } = LineupChildrenVertically.LineUpType.ToTop;
        [field: Space, SerializeField]
        public bool IgnoreDisablingObjects { get; private set; } = true;

        protected override void StartOnce()
        {
            base.StartOnce();

            LineupChildrenVertically.CacheLineupElements(Config, gameObject, IgnoreDisablingObjects);
        }

        protected override void DynamicExecutor_OnExecute()
        {
            LineupChildrenVertically.LineupByElementPosition(Config, gameObject, LineUp, IgnoreDisablingObjects);
        }
    }
}
