using System;
using Tymski;
using TypeReferences;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Scene/SceneSingleChange")]
    public class Scene_SingleChange : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.BridgeExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public SceneReference ChangeTo { get; private set; }
        [field: Space, SerializeField]
        public AnimationClips TransitionOut { get; private set; }
        [field: SerializeField]
        public AnimationClips TransitionIn { get; private set; }

        protected override void DynamicExecutor_OnExecute()
        {
            ChangeScene(ChangeTo, this, TransitionOut, TransitionIn, OnChangeDone);
        }

        private void OnChangeDone()
        {
            Execute(Delta);
        }

        public static void ChangeScene(string sceneChangeToPath) => ChangeScene(sceneChangeToPath, null, null, null, null);
        public static void ChangeScene(string sceneChangeToPath, Action done) => ChangeScene(sceneChangeToPath, null, null, null, done);
        public static void ChangeScene(string sceneChangeToPath, MonoBehaviour caller, AnimationClips transitionOut = null, AnimationClips transitionIn = null, Action done = null)
        {
            if (transitionIn != null && transitionIn.Clips.Count > 0)
            {
                DontDestroyOnLoad(caller.gameObject);
            }

            if (transitionOut != null)
            {
                transitionOut.Play(caller, load);
            }
            else
            {
                load(true);
            }

            void load(bool finished)
            {
                SceneManager.LoadSceneAsync(sceneChangeToPath, LoadSceneMode.Single).completed += loadCompleted;

                void loadCompleted(AsyncOperation o)
                {
                    o.completed -= loadCompleted;

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
