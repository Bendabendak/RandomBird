using SadJam;
using SadJam.Components;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Day/Transformer")]
    [DefaultExecutionOrder(-99999)]
    public class Day_Transformer : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutor,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        [GameConfigSerializeProperty]
        public Day_Config Config { get; }
        public Day_TransformStatus TransformStatus { get; private set; }

        [OnGameConfigChanged(nameof(Config))]
        private void OnConfigChanged(string affected)
        {
            if (GameConfig.IsFieldAffected(affected, nameof(Day_Config.TransformStatus)))
            {
                ChangeTransformStatus(Config.TransformStatus);
            }
        }

        private static Day_Transformer _transformerCache = null;

        protected override void Awake()
        {
            base.Awake();

            if (_transformerCache != null)
            {
                SpawnPool.DestroyImmediate(gameObject);
            }
            else
            {
                _transformerCache = this;
            }

            ChangeTransformStatus(Config.TransformStatus);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _transformerCache = null;
        }

        public void ChangeTransformStatus(Day_TransformStatus status)
        {
            TransformStatus = status;

            Execute(Time.deltaTime, new KeyValuePair<string, object>("day_transformStatus", TransformStatus));
        }

        public static Day_Transformer GetTransformer()
        {
            return _transformerCache;
        }
    }
}
