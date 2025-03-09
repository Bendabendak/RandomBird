using SadJam;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Statistics_LoadStatusFrom : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public List<GameConfig_Selection<Statistics_Key>> KeysToLoad { get; private set; }

        [GameConfigSerializeProperty]
        public Statistics_Owner OwnerToLoadFrom { get; }

        [GameConfigSerializeProperty]
        public Statistics_Owner OwnerToLoadTo { get; }

        protected override void DynamicExecutor_OnExecute()
        {
            foreach (GameConfig_Selection<Statistics_Key> key in KeysToLoad)
            {
                Statistics_Key keyConfig = key.Config;
                if (keyConfig == null) continue;

                if (!Statistics.LoadStatus(OwnerToLoadFrom, keyConfig, out object status, out Statistics.ErrorCodes error))
                {
                    Debug.LogWarning($"Unable to load status { keyConfig.Id } from { OwnerToLoadFrom.Id }! Error: {error}", gameObject);
                    continue;
                }

                if (!Statistics.UpdateAndSaveStatistics(OwnerToLoadTo, out error, new Statistics.DataEntry(keyConfig, status)))
                {
                    Debug.LogWarning($"Unable to update status {keyConfig.Id} to {OwnerToLoadTo.Id}! Error: {error}", gameObject);
                }
            }
        }
    }
}
