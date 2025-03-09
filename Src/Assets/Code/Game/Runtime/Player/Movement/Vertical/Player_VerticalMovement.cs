using SadJam;
using SadJam.StateMachine;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game 
{
    public class Player_VerticalMovement : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Player_Config Config { get; }

        [field: Space, SerializeField]
        public Rigidbody2D Rigidbody { get; private set; }

        [field: Space, SerializeField]
        public UnityEvent OnJump { get; private set; }
        [field: SerializeField]
        public UnityEvent OnFall { get; private set; }

        [NonSerialized]
        private bool _down = false;
        protected override void DynamicExecutor_OnExecute()
        {
            if (Rigidbody.velocity.y != 0)
            {
                if (Rigidbody.velocity.y * Rigidbody.gravityScale < 0)
                {
                    if (_down)
                    {
                        OnFall.Invoke();
                        _down = false;
                    }
                }
                else
                {
                    if (!_down)
                    {
                        _down = true;
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Jump();
            }
            else if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Jump();
                }
            }
        }

        private void Jump()
        {
            if (Rigidbody.gravityScale > 0)
            {
                Rigidbody.velocity = new(Rigidbody.velocity.x, Config.JumpThrust);
            }
            else
            {
                Rigidbody.velocity = new(Rigidbody.velocity.x, - Config.JumpThrust);
            }

            OnJump.Invoke();

            Execute(Delta);
        }
    }
}
