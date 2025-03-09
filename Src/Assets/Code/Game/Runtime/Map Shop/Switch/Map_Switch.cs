using SadJam;
using SadJam.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public abstract class Map_Switch : DynamicExecutor
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

        [field: Space, SerializeField]
        public bool SetAsActive { get; private set; }
        [field: SerializeField]
        public LoadSceneMode LoadSceneMode { get; private set; } = LoadSceneMode.Single;
        [field: Space, SerializeField]
        public AnimationClips TransitionOut { get; private set; }
        [field: SerializeField]
        public AnimationClips TransitionIn { get; private set; }

        public void SwitchToLeft() => Switch(false);
        public void SwitchToRight() => Switch(true);
        private void Switch(bool toRight)
        {
            IEnumerable<Map> chosenList = Config.GetChosenMaps(Owner);
            if (chosenList == null) return;

            IEnumerator<Map> chosenListIterator = chosenList.GetEnumerator();
            if (!chosenListIterator.MoveNext()) return;
            Map chosen = chosenListIterator.Current;

            if (TransitionIn.Clips.Count > 0)
            {
                DontDestroyOnLoad(gameObject);
            }

            int scenesToUnloadCount = 0;
            TransitionOut.Play(this, (bool finished) =>
            {
                if (LoadSceneMode == LoadSceneMode.Single)
                {
                    load();
                    return;
                }

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene loadedMenuScene = SceneManager.GetSceneAt(i);

                    foreach(Map m in Config.Items)
                    {
                        if (m.MenuScene.ScenePath == loadedMenuScene.path)
                        {
                            scenesToUnloadCount++;
                            SceneManager.UnloadSceneAsync(loadedMenuScene).completed += unloadCompleted;
                            break;
                        }
                    }
                }
            });

            void unloadCompleted(AsyncOperation obj)
            {
                obj.completed -= unloadCompleted;

                scenesToUnloadCount--;
                if (scenesToUnloadCount <= 0)
                {
                    load();
                }
            }

            void load()
            {
                int i = Config.Items.IndexOf(chosen);

                if (toRight)
                {
                    if (i >= Config.Items.Count - 1)
                    {
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }
                else
                {
                    if (i <= 0)
                    {
                        i = Config.Items.Count - 1;
                    }
                    else
                    {
                        i--;
                    }
                }

                Map choose = Config.Items[i];

                Scene? preloader = null;
                if (SetAsActive && LoadSceneMode != LoadSceneMode.Single)
                {
                    preloader = SceneManager.CreateScene("**menu preloader**");
                    SceneManager.SetActiveScene((Scene)preloader);
                }

                SceneManager.LoadSceneAsync(choose.MenuScene, LoadSceneMode.Additive).completed += loadCompleted;

                void loadCompleted(AsyncOperation obj)
                {
                    obj.completed -= loadCompleted;

                    Config.Choose(Owner, choose);

                    if (SetAsActive && LoadSceneMode != LoadSceneMode.Single)
                    {
                        Scene s = SceneManager.GetSceneByPath(choose.MenuScene);
                        SceneManager.SetActiveScene(s);
                        SceneManager.MergeScenes((Scene)preloader, s);
                    }

                    if (TransitionIn.Clips.Count > 0)
                    {
                        TransitionIn.Play(this, (bool finished) =>
                        {
                            Execute(Delta);

                            if (LoadSceneMode == LoadSceneMode.Single)
                            {
                                Destroy(gameObject);
                            }
                        });

                        if (gameObject != null)
                        {
                            TransitionOut.Reset();
                        }
                    }
                    else
                    {
                        Execute(Delta);
                    }
                }
            }
        }
    }
}
