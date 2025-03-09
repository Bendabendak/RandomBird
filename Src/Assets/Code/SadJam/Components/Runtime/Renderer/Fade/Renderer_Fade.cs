using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Renderer_Fade
{
    public static Dictionary<GameObject, Dictionary<MonoBehaviour, Coroutine>> RunningFades { get; private set; } = new();

    public static void StopFade(MonoBehaviour from)
    {
        if (RunningFades.ContainsKey(from.gameObject))
        {
            from.StopCoroutine(RunningFades[from.gameObject][from]);

            RunningFades[from.gameObject].Remove(from);
        }
    }

    public static void StartFade(MonoBehaviour from, IEnumerator routine)
    {
        StopAllFades(from);

        Coroutine coroutine = from.StartCoroutine(routine);

        if (RunningFades.ContainsKey(from.gameObject))
        {
            RunningFades[from.gameObject].Add(from, coroutine);
        }
        else
        {
            RunningFades[from.gameObject] = new() { { from, coroutine } };
        }

        from.StartCoroutine(RunFade(from, coroutine));
    }

    public static void StopAllFades(MonoBehaviour from)
    {
        if (RunningFades.ContainsKey(from.gameObject))
        {
            foreach (KeyValuePair<MonoBehaviour, Coroutine> c in RunningFades[from.gameObject])
            {
                c.Key.StopCoroutine(c.Value);
            }

            RunningFades.Remove(from.gameObject);
        }
    }

    private static IEnumerator RunFade(MonoBehaviour from, Coroutine coroutine)
    {
        yield return coroutine;

        RunningFades[from.gameObject].Remove(from);
    }
}
