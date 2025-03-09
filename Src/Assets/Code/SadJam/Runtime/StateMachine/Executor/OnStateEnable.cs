using System;
using TypeReferences;
using UnityEngine;

namespace SadJam.StateMachine
{
    [ClassTypeAddress("Executor/StateMachine/OnStateEnable")]
    public class OnStateEnable : StateListener
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

        protected override void OnEnableState()
        {
            _lastRunningState = RunningStateType.Running;

            Execute(Time.deltaTime);
        }

        [NonSerialized]
        private bool _enabledFromOnEnable = false;
        [NonSerialized]
        private RunningStateType _lastRunningState = RunningStateType.Idle;
        protected override void Start()
        {
            base.Start();

            if ((UpdateOnStart || _enabledFromOnEnable) && IsEnabledButNotExecuted())
            {
                _lastRunningState = RunningStateType.Running;

                Execute(Time.deltaTime);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (UpdateOnEnable && IsEnabledButNotExecuted())
            {
                _enabledFromOnEnable = true;
            }
            else
            {
                _enabledFromOnEnable = false;
            }
        }

        private bool IsEnabledButNotExecuted() => _lastRunningState == RunningStateType.Idle && RunningState == RunningStateType.Running;

        protected override void OnDisable()
        {
            base.OnDisable();

            _enabledFromOnEnable = false;
            _lastRunningState = RunningStateType.Idle;
        }
    }
}
