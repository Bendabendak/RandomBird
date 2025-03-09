using SadJam;
using UnityEngine;

namespace Game 
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerDead_Movement : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public float CollisionForce { get; private set; }

        private Rigidbody2D _rb;
        protected override void Awake()
        {
            base.Awake();

            _rb = GetComponent<Rigidbody2D>();
        }

        protected override void DynamicExecutor_OnExecute()
        {
            if (transform.localScale.x < 0)
            {
                transform.Rotate(0, 0, 1440 * Delta);
            }
            else 
            {
                transform.Rotate(0, 0, 1440 * -1 * Delta);
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision) 
        {
            _rb.velocity = Quaternion.AngleAxis(45f, Vector3.forward) * collision.contacts[0].normal * CollisionForce;
        }
    }
}