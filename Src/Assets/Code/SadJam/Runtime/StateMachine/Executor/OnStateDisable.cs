using System;
using TypeReferences;
using UnityEngine;

namespace SadJam.StateMachine
{
    [ClassTypeAddress("Executor/StateMachine/OnStateDisable")]
    public class OnStateDisable : StateListener
    {
        [field: SerializeField]
        public bool UpdateOnEnable { get; private set; } = false;
        [field: SerializeField]
        public bool UpdateOnStart { get; private set; } = false;

        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void OnDisableState()
        {
            _lastRunningState = RunningStateType.Running;

            Execute(Time.deltaTime);
        }

        [NonSerialized]
        private bool _disabledFromOnEnable = false;
        [NonSerialized]
        private RunningStateType _lastRunningState = RunningStateType.Idle;
        protected override void Start()
        {
            base.Start();

            if ((UpdateOnStart || _disabledFromOnEnable) && IsDisabledButNotExecuted())
            {
                _lastRunningState = RunningStateType.Idle;

                Execute(Time.deltaTime);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (UpdateOnEnable && IsDisabledButNotExecuted())
            {
                _disabledFromOnEnable = true;
            }
            else
            {
                _disabledFromOnEnable = false;
            }
        }

        private bool IsDisabledButNotExecuted() => _lastRunningState == RunningStateType.Running && RunningState == RunningStateType.Idle;

        protected override void OnDisable()
        {
            base.OnDisable();

            _disabledFromOnEnable = false;
            _lastRunningState = RunningStateType.Idle;
        }
    }
}
