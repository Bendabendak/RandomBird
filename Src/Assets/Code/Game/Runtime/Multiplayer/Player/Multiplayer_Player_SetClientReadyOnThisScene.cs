using System.Collections.Generic;
using SadJam;
using UnityEngine;

namespace Game
{
    public class Multiplayer_Player_SetClientReadyOnThisScene : SetClientReadyOnThisScene
    {
        [GameConfigSerializeProperty]
        public Statistics_Owner HostOwner { get; }

        [GameConfigSerializeProperty]
        public Statistics_Owner LocalOwner { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Map_Config MapConfig { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Key MapsKey { get; }

        protected override void OnEnable()
        {
            base.OnEnable();

            Statistics.OnChanged -= OnStatisticsChanged;
            Statistics.OnChanged += OnStatisticsChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Statistics.OnChanged -= OnStatisticsChanged;
        }

        protected override void SetClientReady()
        {
            if (AreOnTheSameMap())
            {
                base.SetClientReady();
            }
            else
            {
                SetClientNotReady();
            }
        }

        private Map GetFirstChosenMap(IEnumerable<Map> maps)
        {
            IEnumerator<Map> iterator = maps.GetEnumerator();

            if (!iterator.MoveNext()) return null;

            return iterator.Current;
        }

        private bool AreOnTheSameMap()
        {
            if (HostOwner == LocalOwner) return true;

            return GetFirstChosenMap(MapConfig.GetChosenMaps(HostOwner)) == GetFirstChosenMap(MapConfig.GetChosenMaps(LocalOwner));
        }

        private void OnStatisticsChanged(string ownerId, Statistics.DataEntry data)
        {
            if ((ownerId == LocalOwner.Id || ownerId == HostOwner.Id) && data.Verify(MapsKey))
            {
                SetClientReady();
            }
        }
    }
}
