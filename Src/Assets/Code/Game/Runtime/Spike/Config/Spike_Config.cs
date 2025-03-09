using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Game
{
    [CreateAssetMenu(fileName = "Spike Config", menuName = "Game/Spike/Config")]
    public class Spike_Config : GameConfig, IGameConfig_Toggleable, IGameConfig_HorizontalDirectional, IGameConfig_HorizontalLineupable, IGameConfig_HorizontallyMoveable, IGameConfig_Spawnable, IGameConfig_ScoreDealer
    {
        [Serializable]
        public class SpikeData : SpawnableExtensions.SpawnableWithProbability
        {
            
        }

        [BlendableField("Enabled"), SerializeField]
        private bool _enabled = true;
        [BlendableProperty("Enabled")]
        public bool Enabled { get; set; }

        [BlendableField("Prefabs"), Space, SerializeField]
        private List<SpikeData> _prefabs;
        [BlendableProperty("Prefabs"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<SpikeData> Prefabs { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (SpikeData rD in Prefabs)
                {
                    yield return rD.Prefab;
                }
            }
        }

        [NonSerialized]
        private Game.Random.Identifier _rndId;
        public GameObject Spawn(GameObject caller) => SpawnableExtensions.Spawn(Prefabs, _rndId)?.Prefab;

        [BlendableField("ScoreToDeal"), Space, SerializeField]
        private int _scoreToDeal;
        [BlendableProperty("ScoreToDeal")]
        public int ScoreToDeal { get; set; }

        [BlendableField("Gap"), SerializeField, Space]
        private float _gap = 2.7f;
        [BlendableProperty("Gap")]
        public float Gap { get; set; }

        [BlendableField("Progress"), SerializeField, Space]
        private Progress_Config _progress;
        [BlendableProperty("Progress")]
        public Progress_Config Progress { get; set; }

        [BlendableField("HorizontalSpeedCurve"), SerializeField, Header("Horizontal Speed")]
        private MinMaxCurve _horizontalSpeedCurve = 2f;
        [BlendableProperty("HorizontalSpeedCurve")]
        public MinMaxCurve HorizontalSpeedCurve { get; set; }

        public float HorizontalSpeedCurveLerpFactor { get; set; }
        public float HorizontalSpeed => HorizontalSpeedCurve.Evaluate(Progress.CurrentTime, HorizontalSpeedCurveLerpFactor);

        [BlendableField("HorizontalDirection"), SerializeField, Range(-1, 1)]
        private int _horizontalDirection = 1;
        [BlendableProperty("HorizontalDirection")]
        public int HorizontalDirection { get; set; }

        [BlendableField("VerticalSpeedCurve"), SerializeField, Header("Vertical Speed")]
        private MinMaxCurve _verticalSpeedCurve = 0f;
        [BlendableProperty("VerticalSpeedCurve")]
        public MinMaxCurve VerticalSpeedCurve { get; set; }

        public float VerticalSpeedCurveLerpFactor { get; set; }
        public float VerticalSpeed => VerticalSpeedCurve.Evaluate(Progress.CurrentTime, VerticalSpeedCurveLerpFactor);

        [BlendableField("SpaceBetween"), SerializeField, Header("Horizontal Lineup")]
        private float _spaceBetween;
        [BlendableProperty("SpaceBetween")]
        public float SpaceBetween { get; set; }

        [BlendableField("WithBounds"), SerializeField]
        private bool _withBounds;
        [BlendableProperty("WithBounds")]
        public bool WithBounds { get; set; }

        protected override void OnConfigResetedToDefault()
        {
            base.OnConfigResetedToDefault();

            ResetConf();
        }

        private void ResetConf()
        {
            _rndId = Game.Random.GetIdentifier();

            VerticalSpeedCurveLerpFactor = UnityEngine.Random.value;
            HorizontalSpeedCurveLerpFactor = UnityEngine.Random.value;
        }
    }
}
