using SadJam;
using UnityEngine;

namespace Game
{
    public class LineupChildrenHorizontally_ByElementPosition : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_HorizontalLineupable Config { get; }

        [field: Space, SerializeField]
        public LineupChildrenHorizontally.LineUpType LineUp { get; private set; } = LineupChildrenHorizontally.LineUpType.ToLeft;
        [field: Space, SerializeField]
        public bool IgnoreDisablingObjects { get; private set; } = true;

        protected override void StartOnce()
        {
            base.StartOnce();

            LineupChildrenHorizontally.CacheLineupElements(Config, gameObject, IgnoreDisablingObjects);
        }

        protected override void DynamicExecutor_OnExecute()
        {
            LineupChildrenHorizontally.LineupByElementPosition(Config, gameObject, LineUp, IgnoreDisablingObjects);
        } 
    }
}
