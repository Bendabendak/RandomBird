using UnityEngine;

namespace SadJam
{
    public class GameConfig_SelectBySelection : GameConfig_Selector, ISerializationCallbackReceiver
    {
        [GameConfigSerializeProperty]
        private GameConfig config { get; set; }

        public virtual void OnAfterDeserialize()
        {
            Config = config;
        }

        public virtual void OnBeforeSerialize()
        {

        }

        [OnNewGameConfigSet(nameof(config))]
        private void OnNewConfigSet() 
        {
            Config = config;
        }
    }
}
