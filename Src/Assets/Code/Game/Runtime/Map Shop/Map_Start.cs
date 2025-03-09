using SadJam;
using SadJam.Components;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Map/Start")]
    public class Map_Start : Scene_SingleChange
    {
        [GameConfigSerializeProperty]
        public Map_Config Config { get; }

        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Map Map { get; }

        protected override void DynamicExecutor_OnExecute()
        {
            IEnumerable<Map> boughtList = Config.GetBoughtMaps(Owner);
            if (boughtList == null) return;

            foreach(Map bought in boughtList)
            {
                if (bought == Map)
                {
                    base.DynamicExecutor_OnExecute();
                    break;
                }
            }
        }
    }
}
