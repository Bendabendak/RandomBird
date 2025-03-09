using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class Renderer_FadeIn : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [field: SerializeField]
        public Renderer Renderer { get; private set; }
        [field: SerializeField]
        public StructComponent<float> Intensity { get; private set; }
        [field: SerializeField]
        public StructComponent<float> Speed { get; private set; }

        private Dictionary<Material, int> _baseRenderQueues = new();

        protected override void Awake()
        {
            base.Awake();

            foreach(Material m in Renderer.materials)
            {
                _baseRenderQueues[m] = m.renderQueue;
            }
        }

        protected override void DynamicExecutor_OnExecute()
        {
            Renderer_Fade.StartFade(this, FadeIn(Speed, Intensity));
        }

        private IEnumerator FadeIn(float speed, float intensity)
        {
            float waitTime = 1f / 30f;
            WaitForSeconds Wait = new WaitForSeconds(waitTime);
            int ticks = 1;

            if (Renderer.materials[0].HasProperty("_Color"))
            {
                float baseAlpha = Renderer.materials[0].color.a;
                float alpha = baseAlpha;
                while (alpha < intensity)
                {
                    alpha = Mathf.Lerp(baseAlpha, intensity, waitTime * ticks * speed);
                    for (int i = 0; i < Renderer.materials.Length; i++)
                    {
                        if (Renderer.materials[i].HasProperty("_Color"))
                        {
                            Renderer.materials[i].color = new Color(
                                Renderer.materials[i].color.r,
                                Renderer.materials[i].color.g,
                                Renderer.materials[i].color.b,
                                alpha
                            );
                        }
                    }

                    ticks++;
                    yield return Wait;
                }
            }

            foreach (KeyValuePair<Material, int> m in _baseRenderQueues)
            {
                m.Key.DisableKeyword("_ALPHABLEND_ON");

                m.Key.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                m.Key.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                m.Key.SetInt("_ZWrite", 1); // re-enable Z Writing
                m.Key.renderQueue = m.Value;
            }
        }
    }
}
