using SadJam.StateMachine;
using UnityEngine;

namespace SadJam.Components
{
    [CreateAssetMenu(fileName = "Spawn Config", menuName = "SadJam/Spawn/Config")]
    public class Spawn_Config : ScriptableObject
    {
        [field: SerializeField]
        public StateCategory SpawnStateCategory { get; private set; }


        [field: Space, SerializeField]
        public State SpawnedState { get; private set; }
        [field: SerializeField]
        public State DespawnedState { get; private set; }
        [field: SerializeField]
        public State DeactivedState { get; private set; }

        [field: Space, SerializeField]
        public TriggerState AwakeTrigger { get; private set; }
        [field: SerializeField]
        public TriggerState StartTrigger { get; private set; }
    }
}
