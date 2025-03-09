using System.Collections.Generic;
using TypeReferences;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SadJam 
{
    [DefaultExecutionOrder(1000)]
    [ClassTypeAddress("Executor/Scene/Loaded")]
    [CustomStaticExecutor("olSHAs2QxE-Sm8tAOSpgMg")]
    public class Scene_Loaded : StaticExecutor
    {
        protected override void Start()
        {
            base.Start();

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                SceneLoaded(activeScene, LoadSceneMode.Single);
            }

            SceneManager.sceneLoaded += SceneLoaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SceneManager.sceneLoaded -= SceneLoaded;
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Execute(Time.deltaTime, new KeyValuePair<string, object>("scene", arg0), new KeyValuePair<string, object>("mode", arg1));
        }
    }
}
