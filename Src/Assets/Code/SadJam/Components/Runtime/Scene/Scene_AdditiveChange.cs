using System;
using Tymski;
using TypeReferences;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Scene/SceneAdditiveChange")]
    public class Scene_AdditiveChange : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public SceneReference ChangeFrom { get; private set; }
        [field: SerializeField]
        public SceneReference ChangeTo { get; private set; }

        [field: Space, SerializeField]
        public bool SetAsActive { get; private set; }

        [field: Space, SerializeField]
        public AnimationClips TransitionOut { get; private set; }
        [field: SerializeField]
        public AnimationClips TransitionIn { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            ChangeScene(ChangeTo, ChangeFrom, SetAsActive, this, TransitionOut, TransitionIn, OnChangeDone);
        }

        private void OnChangeDone()
        {
            Execute(Delta);
        }

        public static void ChangeScene(string sceneChangeToPath, string sceneChangeFromPath, bool setAsActive) => ChangeScene(sceneChangeToPath, sceneChangeFromPath, setAsActive, null, null, null, null);
        public static void ChangeScene(string sceneChangeToPath, string sceneChangeFromPath, bool setAsActive, Action done) => ChangeScene(sceneChangeToPath, sceneChangeFromPath, setAsActive, null, null, null, done);
        public static void ChangeScene(string sceneChangeToPath, string sceneChangeFromPath, bool setAsActive, MonoBehaviour caller, AnimationClips transitionOut = null, AnimationClips transitionIn = null, Action done = null)
        {
            if (transitionIn != null && transitionIn.Clips.Count > 0)
            {
                DontDestroyOnLoad(caller.gameObject);
            }

            Scene? preloader = null;
            if (setAsActive)
            {
                preloader = SceneManager.CreateScene($"**menu preloader {System.Guid.NewGuid()}**");
                SceneManager.SetActiveScene((Scene)preloader);
            }

            if (transitionOut != null)
            {
                transitionOut.Play(caller, (bool finished) =>
                {
                    SceneManager.UnloadSceneAsync(sceneChangeFromPath).completed += unloadCompleted;
                });
            }
            else
            {
                SceneManager.UnloadSceneAsync(sceneChangeFromPath).completed += unloadCompleted;
            }

            void unloadCompleted(AsyncOperation obj)
            {
                obj.completed -= unloadCompleted;

                load();
            }

            void load()
            {
                SceneManager.LoadSceneAsync(sceneChangeToPath, LoadSceneMode.Additive).completed += loadCompleted;

                void loadCompleted(AsyncOperation o)
                {
                    o.completed -= loadCompleted;

                    if (setAsActive)
                    {
                        Scene s = SceneManager.GetSceneByPath(sceneChangeToPath);
                        Scene p = (Scene)preloader;

                        if (!p.IsValid())
                        {
                            if (s.IsValid())
                            {
                                SceneManager.UnloadSceneAsync(s);
                            }
                        }
                        else
                        {
                            if (!s.IsValid())
                            {
                                SceneManager.UnloadSceneAsync(p);
                            }
                            else
                            {
                                SceneManager.SetActiveScene(s);
                                SceneManager.MergeScenes(p, s);
                            }
                        }
                    }

                    if (transitionIn != null && transitionIn.Clips.Count > 0)
                    {
                        if (transitionOut != null)
                        {
                            transitionOut.Reset();
                        }

                        transitionIn.Play(caller, (bool finished) =>
                        {
                            done?.Invoke();

                            Destroy(caller.gameObject);
                        });
                    }
                    else
                    {
                        done?.Invoke();
                    }
                }
            }
        }
    }
}
