using System;

namespace SadJam
{
    public abstract class GameConfig_Selector : SadJam.Component
    {
        [NonSerialized]
        private GameConfig _config;
        public GameConfig Config
        {
            get { return _config; }
            set
            {
                if (_config != value)
                {
                    _config = value;

                    _onNewConfigSet?.Invoke(value);
                }
                else
                {
                    _config = value;
                }
            }
        }

        [NonSerialized]
        private Action<GameConfig> _onNewConfigSet;
        public void AddOnNewConfigSetReceiver(Action<GameConfig> receiver)
        {
            if (receiver == null) return;

            _onNewConfigSet -= receiver;
            _onNewConfigSet += receiver;
        }

        public void RemoveOnNewConfigSetReceiver(Action<GameConfig> receiver)
        {
            if (receiver == null) return;

            _onNewConfigSet -= receiver;
        }
    }
}
