using System;
using System.Collections;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Particle/OnEnd")]
    public class Particle_WaitTillEnd : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public ParticleSystem ParticleSystem { get; private set; }

        private Coroutine _lastCor;
        protected override void DynamicExecutor_OnExecute()
        {
            if (_lastCor != null)
            {
                StopCoroutine(_lastCor);
            }

            _lastCor = StartCoroutine(WaitCor(() =>
            {
                Execute(Delta);
            }));
        }

        private IEnumerator WaitCor(Action done)
        {
            yield return new WaitForSeconds(ParticleSystem.main.duration);

            done?.Invoke();
        }
    }
}
