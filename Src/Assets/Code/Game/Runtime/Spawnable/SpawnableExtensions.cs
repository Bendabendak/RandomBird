using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class SpawnableExtensions
    {
        [Serializable]
        public class Spawnable
        {
            public GameObject Prefab;

            public static explicit operator GameObject(Spawnable s)
            {
                return s.Prefab;
            }
        }

        [Serializable]
        public class SpawnableWithProbability : Spawnable, ISerializationCallbackReceiver
        {
            [Space, Range(0, 1)]
            public float SpawnProbability = 1;
            [Range(0, 1)]
            public float SpawnProbabilityAdditionAfterSpawned;
            [Range(0, 1)]
            public float SpawnProbabilityAdditionAfterNotSpawned;

            [Space]
            public bool ResetSpawnProbabilityAfterSpawned = false;
            public bool ResetSpawnProbabilityAfterNotSpawned = false;

            public float ModifiedSpawnProbability { get; set; }

            public void OnBeforeSerialize()
            {

            }

            public void OnAfterDeserialize()
            {
                ModifiedSpawnProbability = SpawnProbability;
            }
        }

        private static IEnumerable<Game.Random.ElementWithProbability<SpawnableWithProbability>> GetElementsWithProbabilities(IEnumerable<SpawnableWithProbability> elements)
        {
            foreach(SpawnableWithProbability e in elements)
            {
                yield return new(e, e.ModifiedSpawnProbability);
            }
        }

        public static SpawnableWithProbability Spawn(IEnumerable<SpawnableWithProbability> elements, Game.Random.Identifier randomIdentifier)
        {
            Game.Random.ElementWithProbability<SpawnableWithProbability>? target = randomIdentifier.GetElementByProbability(GetElementsWithProbabilities(elements));

            if (target == null)
            {
                foreach (SpawnableWithProbability rD in elements)
                {
                    if (rD.ResetSpawnProbabilityAfterNotSpawned)
                    {
                        rD.ModifiedSpawnProbability = rD.SpawnProbability;
                    }

                    rD.ModifiedSpawnProbability += rD.SpawnProbabilityAdditionAfterNotSpawned;
                }

                return null;
            }

            foreach (SpawnableWithProbability rD in elements)
            {
                if (rD == target.Value.CustomData)
                {
                    if (rD.ResetSpawnProbabilityAfterSpawned)
                    {
                        rD.ModifiedSpawnProbability = rD.SpawnProbability;
                    }

                    rD.ModifiedSpawnProbability += rD.SpawnProbabilityAdditionAfterSpawned;
                    continue;
                }

                if (rD.ResetSpawnProbabilityAfterNotSpawned)
                {
                    rD.ModifiedSpawnProbability = rD.SpawnProbability;
                }

                rD.ModifiedSpawnProbability += rD.SpawnProbabilityAdditionAfterNotSpawned;
            }

            return target.Value.CustomData;
        }
    }
}
