using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/SadJam/Point/OnPointReachedHorizontal")]
    public class OnPointReachedHorizontal : DynamicExecutor
    {
        public enum DeadZoneType
        {
            None,
            Left,
            Right
        }

        [field: SerializeField]
        public StructComponent<Vector3> Point { get; private set; }
        [field: Space, SerializeField]
        public float Zone { get; private set; }
        [field: SerializeField]
        public DeadZoneType DeadZone { get; private set; } = DeadZoneType.None;

        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            if (PointReached(transform.position.x))
            {
                Execute(Delta);
            }
        }

        private static Dictionary<DeadZoneType, Func<float, float, float, bool>> _posIsOutsidePointMap = new(3)
        {
            {
                DeadZoneType.None,
                (pos, point, zone) => pos > point + zone || pos < point - zone
            },
            {
                DeadZoneType.Left,
                (pos, point, zone) => pos > point + zone
            },
            {
                DeadZoneType.Right,
                (pos, point, zone) => pos < point - zone
            }
        };

        private static Dictionary<DeadZoneType, Func<float, float, float, bool>> _posIsInsidePointMap = new(3)
        {
            {
                DeadZoneType.None,
                (pos, point, zone) => pos <= point + zone && pos >= point - zone
            },
            {
                DeadZoneType.Left,
                (pos, point, zone) => pos <= point + zone
            },
            {
                DeadZoneType.Right,
                (pos, point, zone) => pos >= point - zone
            }
        };

        [NonSerialized]
        private bool? _isInsidePoint = null;
        private bool PointReached(float pos)
        {
            float pointPos = Point.Size.x;

            if (_isInsidePoint == null)
            {
                bool isInside = _posIsInsidePointMap[DeadZone](pos, pointPos, Zone);
                _isInsidePoint = isInside;
                return isInside;
            }

            if (_isInsidePoint == true)
            {
                if (_posIsOutsidePointMap[DeadZone](pos, pointPos, Zone))
                {
                    _isInsidePoint = false;

                    return false;
                }
            }
            else
            {
                if (_posIsInsidePointMap[DeadZone](pos, pointPos, Zone))
                {
                    _isInsidePoint = true;

                    return true;
                }
            }

            return false;
        }
    }
}
