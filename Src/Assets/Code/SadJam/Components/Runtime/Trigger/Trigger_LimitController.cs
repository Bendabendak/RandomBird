using System;
using UnityEngine;

namespace SadJam.Components
{
    public class Trigger_LimitController
    {
        public int Limit { get; set; } = 0;

        [NonSerialized]
        private Collider2D _lastCollider;
        [NonSerialized]
        private int _numOfCollisions = -1;
        public bool LimitReached(Collider2D collider)
        {
            if (Limit > 0)
            {
                if (_lastCollider == collider)
                {
                    _numOfCollisions++;
                }
                else
                {
                    _numOfCollisions = 0;
                }
            }

            _lastCollider = collider;

            if (_numOfCollisions < Limit)
            {
                return false;
            }

            return true;
        }

        public void Clear()
        {
            _lastCollider = null;
            _numOfCollisions = -1;
        }

        public Trigger_LimitController() { }

        public Trigger_LimitController(int limit)
        {
            this.Limit = limit;
        }
    }
}
