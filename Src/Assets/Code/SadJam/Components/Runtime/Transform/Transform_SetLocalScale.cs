using UnityEngine;

namespace SadJam.Components
{
    public class Transform_SetLocalScale : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public StructComponent<Vector3> Scale { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (Scale.Size == null) return;
            
            transform.localScale = Scale.Size.ReplaceNaN(transform.localScale);
        }
    }
}
