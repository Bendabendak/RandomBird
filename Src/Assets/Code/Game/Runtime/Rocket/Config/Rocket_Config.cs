using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Rocket Config", menuName = "Game/Rocket/Config")]
    public class Rocket_Config : GameConfig, IGameConfig_Toggleable, IGameConfig_HorizontallyMoveable, IGameConfig_HorizontalDirectional, IGameConfig_VerticalLineupable, IGameConfig_Spawnable, IGameConfig_ScoreDealer
    {
        [Serializable]
        public class RocketData : SpawnableExtensions.SpawnableWithProbability
        {

        }

        [BlendableField("Enabled"), SerializeField]
        private bool _enabled;
        [BlendableProperty("Enabled")]
        public bool Enabled { get; set; }

        [BlendableField("Prefabs"), Space, SerializeField]
        private List<RocketData> _prefabs;
        [BlendableProperty("Prefabs"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<RocketData> Prefabs { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (RocketData rD in Prefabs)
                {
                    yield return rD.Prefab;
                }
            }
        }

        [NonSerialized]
        private Game.Random.Identifier _rndId;
        protected override void OnConfigResetedToDefault()
        {
            _rndId = Game.Random.GetIdentifier();
        }

        public GameObject Spawn(GameObject caller) => SpawnableExtensions.Spawn(Prefabs, _rndId)?.Prefab;

        [BlendableField("ScoreToDeal"), Space, SerializeField]
        private int _scoreToDeal;
        [BlendableProperty("ScoreToDeal")]
        public int ScoreToDeal { get; set; }

        [BlendableField("HorizontalSpeed"), Space, SerializeField]
        private float _horizontalSpeed;
        [BlendableProperty("HorizontalSpeed")]
        public float HorizontalSpeed { get; set; }

        [BlendableField("HorizontalDirection"), Space, Range(-1f, 1f), SerializeField]
        private int _horizontalDirection;
        [BlendableProperty("HorizontalDirection")]
        public int HorizontalDirection { get; set; }

        [BlendableField("SpaceBetween"), Header("Vertical lineup"), Space, SerializeField]
        private float _spaceBetween;
        [BlendableProperty("SpaceBetween")]
        public float SpaceBetween { get; set; }

        [BlendableField("WithBounds"), SerializeField]
        private bool _withBounds;
        [BlendableProperty("WithBounds")]
        public bool WithBounds { get; set; }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_horizontalDirection < 0)
            {
                _horizontalDirection = -1;
            }
            else
            {
                _horizontalDirection = 1;
            }
        }
    }
}
