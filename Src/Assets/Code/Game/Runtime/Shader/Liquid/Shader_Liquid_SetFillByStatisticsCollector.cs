using SadJam;
using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Game
{
    public class Shader_Liquid_SetFillByStatisticsCollector : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public enum Rotation { Up, Down, Right, Left }

        [field: SerializeField]
        public Statistics_Key StatusKey { get; private set; }
        [field: SerializeField]
        public Statistics_Collector Collector { get; private set; }

        [field: Space, SerializeField]
        public Renderer Renderer { get; private set; }
        [field: SerializeField]
        public Rotation RotationType { get; private set; } = Rotation.Up;

        [field: Space, SerializeField]
        public Vector2 FillRange { get; private set; } = new Vector2(0, 1);
        [field: SerializeField]
        public float FillSpeed { get; private set; } = 1;
        [field: SerializeField]
        public MinMaxCurve FillLerpCurve { get; private set; }

        [NonSerialized]
        private double? _lastStat = null;
        protected override void DynamicExecutor_OnExecute()
        {
            if (!Collector.GetNumericalStatus(StatusKey, out float stat, out Statistics_Collector.ErrorCodes error))
            {
                if (error == Statistics_Collector.ErrorCodes.StatusFoundWithDifferentType)
                {
                    Debug.LogWarning("Status is not numeric! " + StatusKey, gameObject);
                }

                return;
            }

            if (stat == _lastStat) return;

            _lastStat = stat;

            Material mat;
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                mat = Renderer.material;
            }
            else
            {
                mat = Renderer.sharedMaterial;
            }
#else
            mat = Renderer.material;
#endif

            float range = Mathf.Abs(FillRange.x - FillRange.y);

            if (range == 0) return;

            if (FillRange.x > 0)
            {
                stat -= - FillRange.x;
            }
            else
            {
                stat += FillRange.x;
            }

            float fillAmount = (float)(stat / range);

            fillAmount = Mathf.Clamp(fillAmount, 0, 1);
            SetFill(Renderer.bounds, mat, fillAmount);
        }

        private void SetFill(Bounds bounds, Material mat, float amount)
        {
            StopAllCoroutines();

            switch (RotationType)
            {
                case Rotation.Up:
                    mat.SetFloat("_BaseRotation", 0);
                    StartCoroutine(SetFillCoroutine(mat, -0.5f - bounds.extents.y + (bounds.size.y * amount)));
                    break;
                case Rotation.Down:
                    mat.SetFloat("_BaseRotation", 180);
                    StartCoroutine(SetFillCoroutine(mat, -0.5f - bounds.extents.y + (bounds.size.y * amount)));
                    break;
                case Rotation.Right:
                    mat.SetFloat("_BaseRotation", 90);
                    StartCoroutine(SetFillCoroutine(mat, -0.5f - bounds.extents.x + (bounds.size.x * amount)));
                    break;
                case Rotation.Left:
                    mat.SetFloat("_BaseRotation", 270);
                    StartCoroutine(SetFillCoroutine(mat, -0.5f - bounds.extents.x + (bounds.size.x * amount)));
                    break;
            }
        }

        private IEnumerator SetFillCoroutine(Material mat, float fill)
        {
            Vector4 lastFill = mat.GetVector("_FillAmount");

            float rnd = UnityEngine.Random.value;
            float t = 0;
            while (t < 1)
            {
                if (FillSpeed <= 0) yield break;

                mat.SetVector("_FillAmount", new Vector4(0, Mathf.LerpUnclamped(lastFill.y, fill, FillLerpCurve.Evaluate(t, rnd))));

                t += Time.deltaTime * FillSpeed;

                yield return null;
            }

            mat.SetVector("_FillAmount", new Vector4(0, fill));
        }
    }
}
