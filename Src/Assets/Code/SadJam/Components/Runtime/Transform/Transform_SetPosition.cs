using UnityEngine;

namespace SadJam.Components
{
    public class Transform_SetPosition : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public StructComponent<Vector3> Position { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (Position.Size == null) return;

            transform.position = Position.Size.ReplaceNaN(transform.position);
        }
    }
}
