using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Game
{
    [CreateAssetMenu(fileName = "Surrounding Config", menuName = "Game/Surrounding/Config")]
    public class Surrounding_Config : GameConfig, IGameConfig_Toggleable, IGameConfig_HorizontalDirectional, IGameConfig_HorizontalLineupable, IGameConfig_HorizontallyMoveable, IGameConfig_Spawnable
    {
        [Serializable]
        public class SurroundingData : SpawnableExtensions.SpawnableWithProbability
        {
            [Space]
            public Surrounding Surrounding;
        }

        [BlendableField("Enabled"), SerializeField]
        private bool _enabled = true;
        [BlendableProperty("Enabled")]
        public bool Enabled { get; set; }

        [BlendableField("Surroundings"), Space, SerializeField]
        private List<SurroundingData> _surroundings;
        [BlendableProperty("Surroundings"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<SurroundingData> Surroundings { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (SurroundingData rD in Surroundings)
                {
                    yield return rD.Prefab;
                }
            }
        }

        [NonSerialized]
        private Game.Random.Identifier _rndId;
        public GameObject Spawn(GameObject caller) => SpawnableExtensions.Spawn(Surroundings, _rndId)?.Prefab;

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

        [BlendableField("HorizontalDirection"), SerializeField, Range(-1,1)]
        private int _horizontalDirection = 1;
        [BlendableProperty("HorizontalDirection")]
        public int HorizontalDirection { get; set; }

        [BlendableField("SpaceBetween"), SerializeField, Header("Horizontal Lineup")]
        private float _spaceBetween = 0;
        [BlendableProperty("SpaceBetween")]
        public float SpaceBetween { get; set; }

        [BlendableField("WithBounds"), SerializeField]
        private bool _withBounds = true;
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

            HorizontalSpeedCurveLerpFactor = UnityEngine.Random.value;
        }
    }
}
