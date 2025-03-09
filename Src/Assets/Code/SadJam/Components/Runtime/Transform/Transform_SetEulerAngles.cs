using UnityEngine;

namespace SadJam.Components
{
    public class Transform_SetEulerAngles : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public StructComponent<Vector3> Rotation { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (Rotation.Size == null) return;

            transform.eulerAngles = Rotation.Size.ReplaceNaN(transform.eulerAngles);
        }
    }
}
