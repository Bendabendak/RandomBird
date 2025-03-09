using System.Collections;
using UnityEngine;

namespace SadJam
{
    public static class StaticCoroutine
    {
        private class StaticCoroutineRunner : MonoBehaviour
        {

        }

        private static StaticCoroutineRunner _runner;
        private static StaticCoroutineRunner Runner 
        { 
            get 
            {
                if (_runner == null)
                {
                    SpawnRunner();
                }

                return _runner;
            } 
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (_runner == null)
            {
                SpawnRunner();
            }
        }

        private static void SpawnRunner()
        {
            _runner = new GameObject("**StaticCoroutine Runner**").AddComponent<StaticCoroutineRunner>();
            UnityEngine.Object.DontDestroyOnLoad(_runner.gameObject);
        }

        public static Coroutine Start(IEnumerator enumerator)
        {
            return Runner.StartCoroutine(enumerator);
        }

        public static void Stop(Coroutine coroutine) 
        {
            Runner.StopCoroutine(coroutine);
        }

        public static void Stop(IEnumerator enumerator)
        {
            Runner.StopCoroutine(enumerator);
        }
    }
}
