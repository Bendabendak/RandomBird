using SadJam;
using System;
using UnityEngine;

namespace Game 
{
    [CreateAssetMenu(fileName = "Statistics Owner", menuName = "Game/Statistics/Owner")]
    public class Statistics_Owner : GameConfig
    {
        [BlendableField("Id"), SerializeField]
        private string _id;
        [BlendableProperty("Id")]
        public string Id { get; set; }

        [field: NonSerialized]
        public bool IsNetworkOwner { get; set; } = false;

        public static implicit operator Statistics.Owner(Statistics_Owner owner)
        {
            return new(owner.Id, owner.IsNetworkOwner);
        }
    }
}
