using SadJam;
using UnityEngine;

namespace Game
{
    public class Statistics_SaveWithIncreaseFromCollector : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Statistics_Collector Collector { get; private set; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [GameConfigSerializeProperty]
        public Statistics_Key StatusKey { get; }

        [field: Space, SerializeField]
        public Vector2 Limit { get; private set; }
        [field: SerializeField]
        public bool NoLimit { get; private set; } = true;
        [field: Space, SerializeField]
        public bool UpdateWhenNotSaved { get; private set; } = true;
        [field: SerializeField]
        public float DefaultValue { get; private set; } = 0;

        protected override void DynamicExecutor_OnExecute()
        {
            if (!Collector.GetNumericalStatus(StatusKey, out float score, out Statistics_Collector.ErrorCodes error))
            {
                if (error == Statistics_Collector.ErrorCodes.StatusFoundWithDifferentType)
                {
                    Debug.LogWarning("Status is not numeric! " + StatusKey, gameObject);
                }

                return;
            }

            if (!Statistics.LoadNumericalStatus(Owner, StatusKey, out double data, out Statistics.ErrorCodes loadError))
            {
                switch (loadError)
                {
                    case Statistics.ErrorCodes.StatusNotFound:
                        if (UpdateWhenNotSaved)
                        {
                            if (NoLimit)
                            {
                                Statistics.UpdateAndSaveStatistics(Owner, new Statistics.DataEntry(StatusKey, DefaultValue + score));
                            }
                            else
                            {
                                Statistics.UpdateAndSaveStatistics(Owner, new Statistics.DataEntry(StatusKey, Mathf.Clamp(DefaultValue + score, Limit.x, Limit.y)));
                            }
                        }
                        break;
                    default:
                        return;
                }
            }
            else
            {
                if (NoLimit)
                {
                    Statistics.UpdateAndSaveStatistics(Owner, new Statistics.DataEntry(StatusKey, (float)(data + score)));
                }
                else
                {
                    Statistics.UpdateAndSaveStatistics(Owner, new Statistics.DataEntry(StatusKey, Mathf.Clamp((float)(data + score), Limit.x, Limit.y)));
                }
            }
        }
    }
}
