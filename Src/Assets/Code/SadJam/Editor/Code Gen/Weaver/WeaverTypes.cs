using Mono.CecilX;
using SadJam;
using System;

namespace SadJamEditor.Weaver
{
    public class WeaverTypes
    {
        public TypeReference ActionTReference { get; private set; }

        public TypeReference GameConfigReference { get; private set; }
        public TypeReference GameConfigSelectionReference { get; private set; }

        public MethodReference GameConfigSelectionGetSelectionReference { get; private set; }
        public MethodReference GameConfigSelectionAddOnNewConfigSetReceiverReference { get; private set; }
        public MethodReference GameConfigSelectionRemoveOnNewConfigSetReceiverReference { get; private set; }

        public MethodReference ActionCtorReference { get; private set; }
        public MethodReference ActionTCtorReference { get; private set; }

        public MethodReference LogErrorReference { get; private set; }
        public MethodReference LogWarningReference { get; private set; }

        public MethodReference SerializeFieldAttributeCtorReference { get; private set; }
        public MethodReference NonSerializedAttributeCtorReference { get; private set; }
        public MethodReference FormalySerializedAsAttributeReferenceCtorReference { get; private set; }

        public MethodReference GameConfigAddOnBlendReceiverReference { get; private set; }
        public MethodReference GameConfigRemoveOnBlendReceiverReference { get; private set; }

        public MethodReference ComponentAddOnAwakeOnceReceiverReference { get; private set; }
        public MethodReference ComponentRemoveOnAwakeOnceReceiverReference { get; private set; }

        public MethodReference ComponentAddOnDestroyReceiverReference { get; private set; }
        public MethodReference ComponentRemoveOnDestroyReceiverReference { get; private set; }

        public AssemblyDefinition Assembly { get; private set; }

        public TypeReference Import<T>() => Import(typeof(T));
        public TypeReference Import(Type t) => Assembly.MainModule.ImportReference(t);
        public MethodReference Import(MethodReference method) => Assembly.MainModule.ImportReference(method);

        public WeaverTypes(AssemblyDefinition assembly, Logger logger, ref bool weavingFailed)
        {
            Assembly = assembly;

            GameConfigReference = Import(typeof(GameConfig));

            TypeReference actionReference = Import(typeof(Action));
            ActionCtorReference = Resolvers.ResolveMethod(actionReference, assembly, logger, ".ctor", ref weavingFailed);

            ActionTReference = Import(typeof(Action<>));
            ActionTCtorReference = Resolvers.ResolveMethod(ActionTReference, assembly, logger, ".ctor", ref weavingFailed);

            TypeReference nonSerializedAttributeReference = Import(typeof(System.NonSerializedAttribute));
            NonSerializedAttributeCtorReference = Resolvers.ResolveMethod(nonSerializedAttributeReference, assembly, logger, ".ctor", ref weavingFailed);

            TypeReference serializeFieldAttributeReference = Import(typeof(UnityEngine.SerializeField));
            SerializeFieldAttributeCtorReference = Resolvers.ResolveMethod(serializeFieldAttributeReference, assembly, logger, ".ctor", ref weavingFailed);

            TypeReference formalySerializedAsAttributeReferenceReference = Import(typeof(UnityEngine.Serialization.FormerlySerializedAsAttribute));
            FormalySerializedAsAttributeReferenceCtorReference = Resolvers.ResolveMethod(formalySerializedAsAttributeReferenceReference, assembly, logger, ".ctor", ref weavingFailed);

            TypeReference unityDebug = Import(typeof(UnityEngine.Debug));
            LogErrorReference = Resolvers.ResolveMethod(unityDebug, assembly, logger, md => md.Name == nameof(UnityEngine.Debug.LogError) && md.Parameters.Count == 1 && md.Parameters[0].ParameterType.FullName == typeof(object).FullName, ref weavingFailed);
            LogWarningReference = Resolvers.ResolveMethod(unityDebug, assembly, logger, md => md.Name == nameof(UnityEngine.Debug.LogWarning) && md.Parameters.Count == 1 && md.Parameters[0].ParameterType.FullName == typeof(object).FullName, ref weavingFailed);

            TypeReference gameConfigIlExtensionsType = Import(typeof(GameConfig_ILExtensions));
            GameConfigAddOnBlendReceiverReference = Resolvers.ResolveMethod(gameConfigIlExtensionsType, assembly, logger, nameof(GameConfig_ILExtensions.SetGameConfigOnBlendReceiver), ref weavingFailed);
            GameConfigRemoveOnBlendReceiverReference = Resolvers.ResolveMethod(gameConfigIlExtensionsType, assembly, logger, nameof(GameConfig_ILExtensions.RemoveGameConfigOnBlendReceiver), ref weavingFailed);

            TypeReference ilIComponentType = Import(typeof(SadJam.IILComponent));
            ComponentAddOnAwakeOnceReceiverReference = Resolvers.ResolveMethod(ilIComponentType, assembly, logger, nameof(SadJam.IILComponent.AddOnAwakeOnceReceiver), ref weavingFailed);
            ComponentRemoveOnAwakeOnceReceiverReference = Resolvers.ResolveMethod(ilIComponentType, assembly, logger, nameof(SadJam.IILComponent.RemoveOnAwakeOnceReceiver), ref weavingFailed);
            ComponentAddOnDestroyReceiverReference = Resolvers.ResolveMethod(ilIComponentType, assembly, logger, nameof(SadJam.IILComponent.AddOnDestroyReceiver), ref weavingFailed);
            ComponentRemoveOnDestroyReceiverReference = Resolvers.ResolveMethod(ilIComponentType, assembly, logger, nameof(SadJam.IILComponent.RemoveOnDestroyReceiver), ref weavingFailed);

            GameConfigSelectionReference = Import(typeof(GameConfig_Selection<>));
            GameConfigSelectionGetSelectionReference = Resolvers.ResolveMethod(GameConfigSelectionReference, assembly, logger, nameof(GameConfig_Selection<GameConfig>.GetSelection), ref weavingFailed);
            GameConfigSelectionAddOnNewConfigSetReceiverReference = Resolvers.ResolveMethod(GameConfigSelectionReference, assembly, logger, nameof(GameConfig_Selection<GameConfig>.AddOnNewConfigSetReceiver), ref weavingFailed);
            GameConfigSelectionRemoveOnNewConfigSetReceiverReference = Resolvers.ResolveMethod(GameConfigSelectionReference, assembly, logger, nameof(GameConfig_Selection<GameConfig>.RemoveOnNewConfigSetReceiver), ref weavingFailed);
        }
    }
}
