using SadJam;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Progress Config", menuName = "Game/Progress/Config")]
    public class Progress_Config : GameConfig
    {
        [BlendableField("DefaultTime"), SerializeField]
        private int _defaultTime;
        [BlendableProperty("DefaultTime")]
        public int DefaultTime { get; set; }

        public int CurrentTime { get; set; }

        public void IncreaseProgress(int time)
        {
            CurrentTime += time;
        }

        public void ResetProgress()
        {
            CurrentTime = _defaultTime;
        }

        protected override void OnConfigResetedToDefault()
        {
            base.OnConfigResetedToDefault();

            ResetProgress();
        }
    }
}
