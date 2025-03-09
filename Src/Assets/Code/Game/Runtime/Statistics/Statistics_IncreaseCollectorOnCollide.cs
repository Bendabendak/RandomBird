using SadJam;
using UnityEngine;

namespace Game
{
    public class Statistics_IncreaseCollectorOnCollide : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Statistics_Key StatusKey { get; }
        [field: Space, SerializeField]
        public float Increase { get; private set; }
        [field: Space, SerializeField]
        public bool NoLimit { get; private set; } = true;
        [field: SerializeField]
        public Vector2 Limit { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (CustomData.ContainsKey("collider"))
            {
                if (CustomData["collider"] is Collider2D collider)
                {
                    Statistics_Collector collector = Statistics_Collector.GetCollector(collider.gameObject);
                    if (collector == null) return;

                    if (!collector.GetNumericalStatus(StatusKey, out float score, out Statistics_Collector.ErrorCodes error))
                    {
                        if (error == Statistics_Collector.ErrorCodes.StatusFoundWithDifferentType)
                        {
                            Debug.LogWarning("Status is not numeric! " + StatusKey, gameObject);
                            return;
                        }

                        if (NoLimit)
                        {
                            collector.ChangeStatus(StatusKey, Increase);
                        }
                        else
                        {
                            float clamp = Mathf.Clamp(score + Increase, Limit.x, Limit.y);

                            if (clamp == Limit.x || clamp == Limit.y)
                            {
                                return;
                            }

                            collector.ChangeStatus(StatusKey, clamp);
                        }
                    }
                    else
                    {
                        if (NoLimit)
                        {
                            collector.ChangeStatus(StatusKey, score + Increase);
                        }
                        else
                        {
                            float clamp = Mathf.Clamp(score + Increase, Limit.x, Limit.y);

                            if (clamp == Limit.x || clamp == Limit.y)
                            {
                                return;
                            }

                            collector.ChangeStatus(StatusKey, clamp);
                        }
                    }

                    Execute(Delta);

                    return;
                }
            }

            Debug.LogWarning("Statistic not increased, collider key not found! Make sure you're using proper executor.", gameObject);
        }
    }
}
