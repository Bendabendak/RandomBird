using System;
using UnityEngine;

namespace SadJam
{
    public static class GameConfig_ILExtensions
    {
        public static void SetGameConfigOnBlendReceiver(GameConfig oldConfig, GameConfig newConfig, Action<string> onBlendReceiver)
        {
            if (oldConfig == newConfig) return;

            if (oldConfig != null)
            {
                oldConfig.OnBlend -= onBlendReceiver;
            }

            if (newConfig != null)
            {
                newConfig.OnBlend -= onBlendReceiver;
                newConfig.OnBlend += onBlendReceiver;
            }
        }

        public static void RemoveGameConfigOnBlendReceiver(GameConfig config, Action<string> onBlendReceiver)
        {
            if (config == null) return;

            config.OnBlend -= onBlendReceiver;
        }
    }
}
