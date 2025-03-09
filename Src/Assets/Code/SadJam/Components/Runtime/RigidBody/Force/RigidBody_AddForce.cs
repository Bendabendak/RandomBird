using UnityEngine;

namespace SadJam.Components
{
    public class RigidBody_AddForce : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Rigidbody RigidBody { get; protected set; }
        [field: SerializeField]
        public StructComponent<Vector3> Force { get; protected set; }
        [field: SerializeField]
        public ForceMode ForceMode { get; protected set; } = ForceMode.Force;

        protected override void DynamicExecutor_OnExecute()
        {
            RigidBody.AddForce(Force, ForceMode);
        }
    }
}
