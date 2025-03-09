using SadJam;
using UnityEngine;

namespace Game
{
    public class Transform_RotateWithTarget : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Transform Target { get; private set; }
        [field: Space, SerializeField]
        public Vector3 Offset { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            transform.right = Target.right;
            transform.localEulerAngles += Offset;
        }
    }
}
