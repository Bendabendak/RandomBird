using UnityEngine;

namespace SadJam
{
    public class DebugOnlyInitializer : StaticBehaviour
    {
        protected override void Start()
        {
            base.Start();
#if UNITY_EDITOR
            Load();

            GlobalSettings.OnSave -= Load;
            GlobalSettings.OnSave += Load;
#endif
        }

        public static void Load()
        {
            if ((bool)GlobalSettings.Get("Debug").Value)
            {
                foreach (UnityEngine.Object c in GameObject.FindObjectsOfType<UnityEngine.Object>(true))
                {
                    if(c is DebugOnlyComponent d)
                    {
                        d.gameObject.hideFlags = HideFlags.None;
                        continue;
                    }

                    if(c.GetType().IsDefined(typeof(DebugOnly), true))
                    {
                        c.hideFlags = HideFlags.None;
                    }
                }
            }
            else
            {
                foreach (UnityEngine.Object c in GameObject.FindObjectsOfType<UnityEngine.Object>(true))
                {
                    if (c is DebugOnlyComponent d)
                    {
                        d.gameObject.hideFlags = HideFlags.HideInHierarchy;
                        continue;
                    }

                    if (c.GetType().IsDefined(typeof(DebugOnly), true))
                    {
                        c.hideFlags = HideFlags.HideInInspector;
                    }
                }
            }
        }
    }
}
