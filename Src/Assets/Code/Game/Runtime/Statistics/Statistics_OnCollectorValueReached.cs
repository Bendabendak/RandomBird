using SadJam;
using System.Collections;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Statistics/OnCollectorValueReached")]
    public class Statistics_OnCollectorValueReached : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public enum TriggerExecution
        {
            Equal,
            LeftSide,
            RightSide
        }

        [field: SerializeField]
        public Statistics_Collector Collector { get; private set; }
        [field: SerializeField]
        public Statistics_Key StatusKey { get; private set; }
        [field: Space, SerializeField]
        public float TargetValue { get; private set; }
        [field: SerializeField]
        public TriggerExecution Trigger { get; private set; } = TriggerExecution.Equal;

        protected override void DynamicExecutor_OnExecute()
        {
            StopAllCoroutines();
            StartCoroutine(Cor());
        }

        private IEnumerator Cor()
        {
            float? lastValue = null;

            while (true)
            {
                if (!Collector.GetNumericalStatus(StatusKey, out float stat, out Statistics_Collector.ErrorCodes error))
                {
                    if (error == Statistics_Collector.ErrorCodes.StatusFoundWithDifferentType)
                    {
                        Debug.LogWarning("Status is not numeric! " + StatusKey, gameObject);
                    }

                    continue;
                }

                switch (Trigger)
                {
                    case TriggerExecution.Equal:
                        if (stat == lastValue)
                        {
                            break;
                        }
                        else
                        {
                            lastValue = null;
                        }

                        if (stat == TargetValue)
                        {
                            lastValue = TargetValue;
                            Execute(Time.deltaTime);
                        }
                        break;
                    case TriggerExecution.LeftSide:
                        if (stat <= lastValue)
                        {
                            break;
                        }
                        else
                        {
                            lastValue = null;
                        }

                        if (stat <= TargetValue)
                        {
                            lastValue = TargetValue;
                            Execute(Time.deltaTime);
                        }
                        break;
                    case TriggerExecution.RightSide:
                        if (stat >= lastValue)
                        {
                            break;
                        }
                        else
                        {
                            lastValue = null;
                        }

                        if (stat >= TargetValue)
                        {
                            lastValue = TargetValue;
                            Execute(Time.deltaTime);
                        }
                        break;
                }

                yield return null;
            }
        }
    }
}
