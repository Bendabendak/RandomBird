using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Player Config", menuName = "Game/Player/Config")]
    public class Player_Config : GameConfig, IGameConfig_HorizontalDirectional, IGameConfig_Spawnable, IGameConfig_Toggleable
    {
        [Serializable]
        public class PlayerData : SpawnableExtensions.SpawnableWithProbability
        {

        }

        [BlendableField("Enabled"), SerializeField]
        private bool _enabled = true;
        [BlendableProperty("Enabled")]
        public bool Enabled { get; set; }

        [BlendableField("Prefabs"), Space, SerializeField]
        private List<PlayerData> _prefabs;
        [BlendableProperty("Prefabs"),
        BlendablePropertyCorrespondingInterface(nameof(IGameConfig_Spawnable), nameof(IGameConfig_Spawnable.Prefabs))]
        public List<PlayerData> Prefabs { get; set; }
        IEnumerable<GameObject> IGameConfig_Spawnable.Prefabs
        {
            get
            {
                foreach (PlayerData rD in Prefabs)
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

        public GameObject Spawn(GameObject caller) 
        {
            return SpawnableExtensions.Spawn(Prefabs, _rndId)?.Prefab; ;
        }

        [BlendableField("PrefabDead"), SerializeField]
        private GameObject _prefabDead;
        [BlendableProperty("PrefabDead")]
        public GameObject PlayerDead { get; set; }

        [BlendableField("JumpThrust"), Space, SerializeField]
        private float _jumpThrust = 7f;
        [BlendableProperty("JumpThrust")]
        public float JumpThrust { get; set; }
        [BlendableField("GravityScale"), SerializeField]
        private float _gravityScale = 2f;
        [BlendableProperty("GravityScale")]
        public float GravityScale { get; set; }

        [BlendableField("GravityDirection"), Space, Range(-1, 1), SerializeField]
        private int _gravityDirection = 1;
        [BlendableProperty("GravityDirection")]
        public int GravityDirection { get; set; }

        [BlendableField("HorizontalDirection"), Range(-1, 1), SerializeField]
        private int _horizontalDirection = 1;
        [BlendableProperty("HorizontalDirection")]
        public int HorizontalDirection { get; set; }

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

            if (_gravityDirection < 0)
            {
                _gravityDirection = -1;
            }
            else
            {
                _gravityDirection = 1;
            }
        }
    }
}
