using SadJam;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class Statistics_IncreaseCollectorOverTime : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Statistics_Key StatusKey { get; private set; }
        [field: SerializeField]
        public Statistics_Collector Collector { get; private set; }
        [field: Space, SerializeField]
        public float Increase { get; private set; }
        [field: Space, SerializeField]
        public Vector2 Limit { get; private set; }
        [field: SerializeField]
        public bool NoLimit { get; private set; } = true;
        [field: Space, SerializeField]
        public float Interval { get; private set; }
        [field: SerializeField]
        public bool UnscaledTime { get; private set; } = false;

        protected override void DynamicExecutor_OnExecute()
        {
            StopAllCoroutines();
            StartCoroutine(Cor());
        }

        private IEnumerator Cor()
        {
            WaitForSeconds s = new(Interval);
            WaitForSecondsRealtime sR = new(Interval);

            while (true)
            {
                if (!Collector.GetNumericalStatus(StatusKey, out float score, out Statistics_Collector.ErrorCodes error))
                {
                    if (error == Statistics_Collector.ErrorCodes.StatusFoundWithDifferentType)
                    {
                        Debug.LogWarning("Status is not numeric! " + StatusKey.Id, gameObject);
                        continue;
                    }

                    if (NoLimit)
                    {
                        Collector.ChangeStatus(StatusKey, Increase);
                    }
                    else
                    {
                        Collector.ChangeStatus(StatusKey, Mathf.Clamp(Increase, Limit.x, Limit.y));
                    }
                }
                else
                {
                    if (NoLimit)
                    {
                        Collector.ChangeStatus(StatusKey, (float)(score + Increase));
                    }
                    else
                    {
                        Collector.ChangeStatus(StatusKey, Mathf.Clamp((float)(score + Increase), Limit.x, Limit.y));
                    }
                }

                if (UnscaledTime)
                {
                    yield return sR;
                }
                else
                {
                    yield return s;
                }
            }
        }
    }
}
