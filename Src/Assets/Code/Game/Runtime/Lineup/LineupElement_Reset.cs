using SadJam;
using UnityEngine;

namespace Game
{
    public class LineupElement_Reset : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public LineupElement Element { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            Element.Linedup = false;
        }
    }
}
