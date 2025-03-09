using SadJam;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Map/LoadChosen")]
    public class Map_LoadChosen : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Map_Config Config { get; }

        [field: Space]
        [GameConfigSerializeProperty]
        public Statistics_Owner Owner { get; }

        protected override void DynamicExecutor_OnExecute()
        {
            IEnumerable<Map> chosenList = Config.GetChosenMaps(Owner);
            if (chosenList == null) return;

            IEnumerator<Map> chosenListIterator = chosenList.GetEnumerator();
            if (!chosenListIterator.MoveNext()) return;
            Map chosen = chosenListIterator.Current;

            Scene preloader = SceneManager.CreateScene($"**menu preloader {System.Guid.NewGuid()}**");
            SceneManager.SetActiveScene(preloader);

            SceneManager.LoadSceneAsync(chosen.MenuScene, LoadSceneMode.Additive).completed += completed;

            void completed(AsyncOperation obj)
            {
                Scene chosenScene = SceneManager.GetSceneByPath(chosen.MenuScene);
                if (!preloader.IsValid())
                {
                    if (chosenScene.IsValid())
                    {
                        SceneManager.UnloadSceneAsync(chosenScene);
                    }
                }
                else
                {
                    if (!chosenScene.IsValid())
                    {
                        SceneManager.UnloadSceneAsync(preloader);
                    }
                    else
                    {
                        SceneManager.SetActiveScene(chosenScene);
                        SceneManager.MergeScenes(preloader, chosenScene);
                    }
                }

                Execute(Delta);
            }
        }
    }
}
