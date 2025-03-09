using SadJam;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
    public class Player_ChangeHorizontalDirection : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public enum DirectionType
        {
            Forward,
            Backward
        }

        [field: SerializeField]
        public DirectionType Direction { get; private set; }

        [field: Space, SerializeField]
        public Transform Target { get; private set; }
        [field: SerializeField]
        public StructComponent<Vector3> NewPosition { get; private set; }

        private static Dictionary<DirectionType, Action<Player_ChangeHorizontalDirection>> _directionChangeMap = new(2)
        {
            {
                DirectionType.Forward,
                (t) =>
                {
                    t.Target.localScale = new(1, t.Target.localScale.y, t.Target.localScale.z);
                }
            },
            {
                DirectionType.Backward,
                (t) =>
                {
                    t.Target.localScale = new(-1, t.Target.localScale.y, t.Target.localScale.z);
                }
            }
        };

        protected override void DynamicExecutor_OnExecute()
        {
            _directionChangeMap[Direction](this);

            if (NewPosition != null)
            {
                Target.transform.position = NewPosition;
            }
        }
    }
}
