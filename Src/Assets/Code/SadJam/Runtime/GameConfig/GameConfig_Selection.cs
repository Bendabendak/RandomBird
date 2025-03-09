using System;
using UnityEngine;

namespace SadJam
{
    [Serializable]
    public class GameConfig_Selection<T> : ISerializationCallbackReceiver where T : class
    {
        [SerializeField]
        private GameConfig _config;
        [SerializeField]
        private GameConfig_Selector _configSelector;

        public T Config => GetSelection() as T;

        public GameConfig GetSelection()
        {
            if (_configSelector != null)
            {
                return _configSelector.Config;
            }

            return _config;
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

        [NonSerialized]
        private GameConfig _lastConfig = null;
        [NonSerialized]
        private GameConfig_Selector _lastConfigSelector = null;
        public void OnAfterDeserialize() 
        {
            if (_configSelector != null) 
            {
                if (_configSelector.Config != _lastConfig)
                {
                    _lastConfig = _configSelector.Config;

                    _onNewConfigSet?.Invoke(_configSelector.Config);
                }

                if (_lastConfigSelector != _configSelector)
                {
                    if (_lastConfigSelector != null)
                    {
                        _lastConfigSelector.RemoveOnNewConfigSetReceiver(OnConfigSelectorSetNewConfig);
                    }

                    _configSelector.AddOnNewConfigSetReceiver(OnConfigSelectorSetNewConfig);

                    _lastConfigSelector = _configSelector;
                }
            }
            else
            {
                if (_config != _lastConfig)
                {
                    _lastConfig = _config;

                    _onNewConfigSet?.Invoke(_config);
                }

                if (_lastConfigSelector != null)
                {
                    _lastConfigSelector.RemoveOnNewConfigSetReceiver(OnConfigSelectorSetNewConfig);

                    _lastConfigSelector = null;
                }
            }
        }
        public void OnBeforeSerialize()
        {
            GameConfig g = GetSelection();

            if (g != null && g is not T)
            {
                Debug.LogError($"Selected config {g.name} is not type of {typeof(T).Name}", g);
            }
        }

        private void OnConfigSelectorSetNewConfig(GameConfig config)
        {
            if (config != _lastConfig)
            {
                _lastConfig = config;

                _onNewConfigSet?.Invoke(config);
            }
        }
    }
}
