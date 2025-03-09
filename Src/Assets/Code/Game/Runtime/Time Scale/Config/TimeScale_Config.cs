using SadJam;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Game
{
    [CreateAssetMenu(fileName = "Time Scale Config", menuName = "Game/TimeScale/Config")]
    public class TimeScale_Config : GameConfig
    {
        [BlendableField("HorizontalSpeedCurve"), SerializeField]
        private MinMaxCurve _scaleCurve = 2f;
        [BlendableProperty("HorizontalSpeedCurve")]
        public MinMaxCurve ScaleCurve { get; set; }

        [BlendableField("Progress"), SerializeField, Space]
        private Progress_Config _progress;
        [BlendableProperty("Progress")]
        public Progress_Config Progress { get; set; }

        public float ScaleCurveLerpFactor { get; set; }
        public float Scale => ScaleCurve.Evaluate(Progress.CurrentTime, ScaleCurveLerpFactor);

        public float? InitTimeScale { get; private set; } = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            InitTimeScale = Time.timeScale;
        }

        public void ResetTimeScale()
        {
            if (InitTimeScale == null) return;

            Time.timeScale = InitTimeScale.Value;
        }

        protected override void OnConfigResetedToDefault()
        {
            base.OnConfigResetedToDefault();

            ResetConf();
        }

        private void ResetConf()
        {
            ScaleCurveLerpFactor = UnityEngine.Random.value;

            ResetTimeScale();
        }
    }
}
