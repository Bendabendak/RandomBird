using System;
using UnityEngine;

namespace SadJam.Components
{
    public class Transform_ChildrenToCircle : MonoBehaviour
    {
        [field: SerializeField]
        public bool Clockwise { get; private set; }
        [field: SerializeField]
        public bool Clockshape { get; private set; }

        [NonSerialized]
        private int _lastCount;
        protected virtual void Update()
        {
            if (transform.childCount != _lastCount)
            {
                int count = 0;
                foreach (Transform t in transform)
                {
                    if (t.gameObject.TryGetComponent(out Transform_ChildElement c))
                    {
                        if (c.Ignore)
                        {
                            continue;
                        }           
                    }
  
                    count++;
                }

                _lastCount = transform.childCount;

                float addUp;

                if (Clockwise)
                {
                    addUp = -360f / count;
                }
                else
                {
                    addUp = 360f / count;
                }

                float angle;
                if (Clockshape)
                {
                    angle = addUp / 2f;
                }
                else 
                {
                    angle = addUp;
                }

                foreach (Transform t in transform)
                {
                    t.eulerAngles = new(t.eulerAngles.x, t.eulerAngles.y, angle);
                    angle += addUp;
                }
            }
        }
    }
}
