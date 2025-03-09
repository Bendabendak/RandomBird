using SadJam;
using UnityEngine;

namespace Game
{
    public class DealScore : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public IGameConfig_ScoreDealer Config { get; }

        [GameConfigSerializeProperty]
        public Statistics_Key StatusKey { get; private set; }

        [field: Space, SerializeField]
        public Statistics_Collector Collector { get; private set; }

        [field: Space, SerializeField]
        public bool NoLimit { get; private set; } = true;
        [field: SerializeField]
        public Vector2 Limit { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            if (!Collector.GetNumericalStatus(StatusKey, out float score, out Statistics_Collector.ErrorCodes error))
            {
                if (error == Statistics_Collector.ErrorCodes.StatusFoundWithDifferentType)
                {
                    Debug.LogWarning("Status is not numeric! " + StatusKey, gameObject);
                    return;
                }

                if (NoLimit)
                {
                    Collector.ChangeStatus(StatusKey, Config.ScoreToDeal);
                }
                else
                {
                    float clamp = Mathf.Clamp(Config.ScoreToDeal, Limit.x, Limit.y);

                    if (clamp == Limit.x || clamp == Limit.y)
                    {
                        return;
                    }

                    Collector.ChangeStatus(StatusKey, clamp);
                }
            }
            else
            {
                if (NoLimit)
                {
                    Collector.ChangeStatus(StatusKey, score + Config.ScoreToDeal);
                }
                else
                {
                    float clamp = Mathf.Clamp(score + Config.ScoreToDeal, Limit.x, Limit.y);

                    if (clamp == Limit.x || clamp == Limit.y)
                    {
                        return;
                    }

                    Collector.ChangeStatus(StatusKey, clamp);
                }
            }

            Execute(Delta);
        }
    }
}


