using SadJam;
using UnityEngine;

namespace Game
{
    public class Statistics_SaveHigherCollector : DynamicExecutor
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
        public Statistics_Key CollectorStatusKey { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Key SavedStatusKey { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: Space, SerializeField]
        public bool UpdateWhenNotSaved { get; private set; } = true;

        protected override void DynamicExecutor_OnExecute()
        {
            if (!Collector.GetNumericalStatus(CollectorStatusKey, out float score, out Statistics_Collector.ErrorCodes error))
            {
                if (error == Statistics_Collector.ErrorCodes.StatusFoundWithDifferentType)
                {
                    Debug.LogWarning("Status is not numeric! " + CollectorStatusKey.Id, gameObject);
                }
                return;
            }

            if (!Statistics.LoadNumericalStatus(Owner, SavedStatusKey, out double data, out Statistics.ErrorCodes loadError))
            {
                switch (loadError)
                {
                    case Statistics.ErrorCodes.StatusNotFound:
                        if (UpdateWhenNotSaved)
                        {
                            Statistics.UpdateAndSaveStatistics(Owner, new Statistics.DataEntry(SavedStatusKey, score));
                        }
                        break;
                    default:
                        return;
                }
            }

            if (data >= score)
            {
                return;
            }

            Statistics.UpdateAndSaveStatistics(Owner, new Statistics.DataEntry(SavedStatusKey, score));
        }
    }
}
