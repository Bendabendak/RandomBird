using SadJam;
using UnityEngine;

namespace Game
{
    public class SunDummy_Movement : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Sun_Config Config { get; }

        [field: Space, SerializeField]
        public float Progress { get; private set; } = 0.3f;

        [field: Space, SerializeField]
        public StructComponent<Vector3> StartPoint { get; private set; }
        [field: SerializeField]
        public StructComponent<Vector3> EndPoint { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            transform.position = new Vector2(Mathf.Lerp(StartPoint.Size.x, EndPoint.Size.x, Progress), StartPoint.Size.y + Config.LapCurve.Evaluate(Progress));
        }
    }
}
