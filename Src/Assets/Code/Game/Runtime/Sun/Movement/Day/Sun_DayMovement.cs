using SadJam;
using UnityEngine;
using System;

namespace Game
{
    public class Sun_DayMovement : DynamicExecutor
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
        public StructComponent<Vector3> StartPoint { get; private set; }
        [field: SerializeField]
        public StructComponent<Vector3> EndPoint { get; private set; }

        [NonSerialized]
        private float _time = 0;
        [NonSerialized]
        private float _progress = 1;
        protected override void DynamicExecutor_OnExecute()
        {
            if (_progress >= 1)
            {
                _time = 0;
            }

            _progress = _time / Config.DayInterval;
            transform.position = new Vector2(Mathf.Lerp(StartPoint.Size.x, EndPoint.Size.x, _progress), StartPoint.Size.y + Config.LapCurve.Evaluate(_progress));

            _time += Delta;
        }
    }
}
