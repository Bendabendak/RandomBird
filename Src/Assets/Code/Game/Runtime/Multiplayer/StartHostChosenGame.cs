using Mirror;
using SadJam;
using SadJam.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StartHostChosenGame : DynamicExecutor
    {
        [GameConfigSerializeProperty]
        public Map_Config Config { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner LocalOwner { get; }

        [field: Space, SerializeField]
        public AnimationClips TransitionOut { get; private set; }
        [field: SerializeField]
        public AnimationClips TransitionIn { get; private set; }

        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void DynamicExecutor_OnExecute()
        {
            base.DynamicExecutor_OnExecute();

            IEnumerable<Map> chosenList;
            if (!NetworkClient.active)
            {
                chosenList = Config.GetChosenMaps(LocalOwner);
            }
            else
            {
                chosenList = Config.GetChosenMaps(new() { Id = "0", IsNetworkOwner = true });
            }

            if (chosenList == null) return;
            IEnumerator<Map> chosenListIterator = chosenList.GetEnumerator();
            if (!chosenListIterator.MoveNext()) return;
            Map chosen = chosenListIterator.Current;

            Scene_SingleChange.ChangeScene(chosen.GameScene, this, TransitionOut, TransitionIn, OnSceneChanged);
        }

        private void OnSceneChanged()
        {
            Execute(Time.deltaTime);
        }
    }
}
