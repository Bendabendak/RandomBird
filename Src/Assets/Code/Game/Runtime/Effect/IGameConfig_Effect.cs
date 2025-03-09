using UnityEngine;

namespace Game
{
    public interface IGameConfig_Effect : IGameConfig_Toggleable
    {
        public Sprite Icon { get; }
        public float Duration { get; }
        public bool DurationUnscaled { get; }

        public bool ActivateEffect(GameObject effectHolder);
        public void RemoveEffect(GameObject effectHolder);
    }
}
