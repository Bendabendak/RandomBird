using SadJam;
using System;
using System.Collections.Generic;

namespace Game
{
    public static class Random
    {
        public struct ElementWithProbability<T>
        {
            public T CustomData { get; set; }
            public float Probability { get; set; }

            public ElementWithProbability(T customData, float probability)
            {
                CustomData = customData;
                Probability = probability;
            }
        }

        public struct Identifier
        {
            private readonly string Id;

            public Identifier(string id)
            {
                Id = id;
            }

            public float Value()
            {
                if (Config == null) throw new ConfigNotSetException();

                UnityEngine.Random.State stateBefore = UnityEngine.Random.state;
                SetRandom(Id);

                float result = UnityEngine.Random.value;
                _rndStates[Id] = UnityEngine.Random.state;
                UnityEngine.Random.state = stateBefore;

                return result;
            }

            public float Range(float minInclusive, float maxInclusive)
            {
                if (Config == null) throw new ConfigNotSetException();

                UnityEngine.Random.State stateBefore = UnityEngine.Random.state;
                SetRandom(Id);

                float result = UnityEngine.Random.Range(minInclusive, maxInclusive);
                _rndStates[Id] = UnityEngine.Random.state;
                UnityEngine.Random.state = stateBefore;

                return result;
            }

            public int Range(int minInclusive, int maxInclusive)
            {
                if (Config == null) throw new ConfigNotSetException();

                UnityEngine.Random.State stateBefore = UnityEngine.Random.state;
                SetRandom(Id);

                int result = UnityEngine.Random.Range(minInclusive, maxInclusive);
                _rndStates[Id] = UnityEngine.Random.state;
                UnityEngine.Random.state = stateBefore;

                return result;
            }

            public void Reset()
            {
                _rndStates.Remove(Id);
            }

            private static List<string> _rndStatestoRemove = new();
            private static void SetRandom(string id)
            {
                _rndStatestoRemove.Clear();
                foreach (string key in _rndStates.Keys)
                {
                    if (key == null)
                    {
                        _rndStatestoRemove.Add(key);
                    }
                }

                foreach (string key in _rndStatestoRemove)
                {
                    _rndStates.Remove(key);
                }

                if (!_rndStates.TryGetValue(id, out UnityEngine.Random.State state))
                {
                    UnityEngine.Random.state = _rndSeedState;
                }
                else
                {
                    UnityEngine.Random.state = state;
                }
            }

            public ElementWithProbability<T>? GetElementByProbability<T>(IEnumerable<ElementWithProbability<T>> elements)
            {
                float random = Value();
                float add = 0;

                foreach (ElementWithProbability<T> e in elements)
                {
                    add += e.Probability;
                    if (add >= random)
                    {
                        return e;
                    }
                }

                return null;
            }
        }

        public static IGameConfig_GameManager Config { get; private set; }

        private static Dictionary<string, UnityEngine.Random.State> _rndStates = new();

        static Random()
        {
            OnGameManagerConfigChanged();
            OnGameConfigAboutToReset();

            GameConfig.OnAllAboutToReset += OnGameConfigAboutToReset;
            GameManager.OnConfigChanged += OnGameManagerConfigChanged;
        }

        private static void OnGameConfigAboutToReset()
        {
            _rndStates.Clear();
        }

        private static UnityEngine.Random.State _rndSeedState;
        private static void OnGameManagerConfigChanged()
        {
            Config = GameManager.Config;

            if (Config != null)
            {
                UnityEngine.Random.InitState(Config.Seed);
                _rndSeedState = UnityEngine.Random.state;
            }
        }

        public static Identifier GetIdentifier()
        {
            return new(Guid.NewGuid().ToString());
        }

        public class ConfigNotSetException : Exception
        {
            public ConfigNotSetException() : base("GameManager config not set!")
            {

            }
        }
    }
}
