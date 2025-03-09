using TypeReferences;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SadJam.Components
{
    [ClassTypeAddress("Executor/Scene/OnUnloaded")]
    public class Scene_OnUnloaded : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        protected override void Awake()
        {
            base.Awake();

            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        }

        private void SceneManager_sceneUnloaded(Scene arg0)
        {
            SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
            Execute(Time.deltaTime);
        }
    }
}
