using System.Collections.Generic;
using Tymski;
using TypeReferences;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Scene/OnSceneLoadedLocal")]
    public class OnSceneLoadedLocal : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public List<SceneReference> TargetScenes { get; private set; }
        [field: Space, SerializeField]
        public bool ExecuteOnStart { get; private set; } = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        protected override void Start()
        {
            base.Start();

            if (ExecuteOnStart)
            {
                Scene activeScene = SceneManager.GetActiveScene();

                foreach (SceneReference sceneReference in TargetScenes)
                {
                    if (sceneReference.ScenePath == activeScene.path)
                    {
                        Execute(Time.deltaTime);
                        break;
                    }
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            foreach (SceneReference sceneReference in TargetScenes)
            {
                if (sceneReference.ScenePath == arg1.path)
                {
                    Execute(Time.deltaTime);
                    break;
                }
            }
        }
    }
}
