using System.Collections;
using UnityEngine;

namespace SadJam.Components
{
    public class Renderer_FadeOut : DynamicExecutor
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

        protected override void DynamicExecutor_OnExecute()
        {
            Renderer_Fade.StartFade(this, FadeOut(Speed, Intensity));
        }

        private IEnumerator FadeOut(float speed, float intensity)
        {
            float waitTime = 1f / 30f;
            WaitForSeconds Wait = new WaitForSeconds(waitTime);
            int ticks = 1;

            for (int i = 0; i < Renderer.materials.Length; i++)
            {
                Renderer.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha); // affects both "Transparent" and "Fade" options
                Renderer.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha); // affects both "Transparent" and "Fade" options
                Renderer.materials[i].SetInt("_ZWrite", 0); // disable Z writing

                Renderer.materials[i].EnableKeyword("_ALPHABLEND_ON");

                Renderer.materials[i].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }

            if (Renderer.materials[0].HasProperty("_Color"))
            {
                float baseAlpha = Renderer.materials[0].color.a;
                float alpha = baseAlpha;
                while (alpha > intensity)
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
        }
    }
}
