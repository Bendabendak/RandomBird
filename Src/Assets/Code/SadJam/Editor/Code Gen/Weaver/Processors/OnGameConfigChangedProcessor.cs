using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.CecilX.Rocks;
using SadJam;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SadJamEditor.Weaver
{
    public static class OnGameConfigChangedProcessor
    {
        public static bool Process(TypeDefinition td, Dictionary<TypeDefinition, List<GameConfigPropertyDefinition>> gameConfigProperties, WeaverTypes weaverTypes, Logger logger, ref bool weavingFailed)
        {
            bool modified = false;
            if (!td.HasMethods)
            {
                return modified;
            }

            List<TypeDefinition> parentsAndThis = td.GetAllParents().ToList();
            parentsAndThis.Capacity += 1;
            parentsAndThis.Add(td);

            for (int mDIndex = 0; mDIndex < td.Methods.Count; mDIndex++)
            {
                MethodDefinition onConfigChangedMethod = td.Methods[mDIndex];

                if (!onConfigChangedMethod.HasCustomAttributes || onConfigChangedMethod.IsVirtual || onConfigChangedMethod.IsSetter || onConfigChangedMethod.IsGetter) continue;

                for (int cAIndex = 0; cAIndex < onConfigChangedMethod.CustomAttributes.Count; cAIndex++)
                {
                    CustomAttribute ca = onConfigChangedMethod.CustomAttributes[cAIndex];

                    if (ca.AttributeType.Is<OnGameConfigChangedAttribute>())
                    {
                        if (!ca.HasConstructorArguments || ca.ConstructorArguments.Count <= 0)
                        {
                            logger.Error($"{nameof(OnGameConfigChangedAttribute)} doesn't contain property name argument!", onConfigChangedMethod);
                            continue;
                        }

                        string configPropName = (string)ca.ConstructorArguments[0].Value;

                        if (string.IsNullOrWhiteSpace(configPropName))
                        {
                            continue;
                        }

                        if (td.GetParentWithInterface(typeof(SadJam.IILComponent).FullName) == null)
                        {
                            logger.Warning($"Using {nameof(OnGameConfigChangedAttribute)} is allowed only in {typeof(SadJam.IILComponent).FullName} classes!", td);
                            break;
                        }

                        bool propFound = false;
                        foreach (TypeDefinition parent in parentsAndThis)
                        {
                            if (gameConfigProperties.TryGetValue(parent, out List<GameConfigPropertyDefinition> props))
                            {
                                for (int pDIndex = 0; pDIndex < props.Count; pDIndex++)
                                {
                                    GameConfigPropertyDefinition pd = props[pDIndex];

                                    if (pd.Property.Name == configPropName)
                                    {
                                        modified |= InjectFieldOnNewConfigSetReceiver(td, onConfigChangedMethod, pd, weaverTypes, logger, ref weavingFailed);
                                        propFound = true;

                                        break;
                                    }
                                }
                            }

                            if (propFound) break;
                        }
                    }
                }
            }

            return modified;
        }

        private static bool InjectFieldOnNewConfigSetReceiver(TypeDefinition td, MethodDefinition onConfigChangedMethod, GameConfigPropertyDefinition pd, WeaverTypes weaverTypes, Logger logger, ref bool weavingFailed)
        {
            MethodDefinition ctor = td.GetMethod(".ctor");
            if (ctor == null || !ctor.RemoveFinalRetInstruction())
            {
                logger.Error($"{td.Name} has invalid constructor", td);

                weavingFailed = true;
                return false;
            }

            string lastConfigFieldName = $"_lastConfig__{Guid.NewGuid():N}";
            FieldDefinition lastConfigField = new(lastConfigFieldName, FieldAttributes.Private, weaverTypes.GameConfigReference);

            CustomAttribute nonSerializedAt = new(weaverTypes.NonSerializedAttributeCtorReference);
            lastConfigField.CustomAttributes.Add(nonSerializedAt);

            td.Fields.Add(lastConfigField);

            string onAwakeOnceReceiverMethodName = $"OnAwakeOnceReceiver__{Guid.NewGuid():N}";
            MethodDefinition onAwakeOnceReceiverMethod = new(onAwakeOnceReceiverMethodName, MethodAttributes.Private, weaverTypes.Assembly.MainModule.TypeSystem.Void);
            onAwakeOnceReceiverMethod.Body = new(onAwakeOnceReceiverMethod);

            string onNewConfigSetMethodName = $"OnNewConfigSetReceiver__{Guid.NewGuid():N}";
            MethodDefinition onNewConfigSetMethod = new(onNewConfigSetMethodName, MethodAttributes.Private, weaverTypes.Assembly.MainModule.TypeSystem.Void);
            onNewConfigSetMethod.Parameters.Add(new("gameconfig", ParameterAttributes.None, weaverTypes.GameConfigReference));
            onNewConfigSetMethod.Body = new(onNewConfigSetMethod);

            ILProcessor onAwakeOnceReceiverWorker = onAwakeOnceReceiverMethod.Body.GetILProcessor();
            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldarg_0);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldfld, pd.Field);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldarg_0);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldftn, onNewConfigSetMethod);
            GenericInstanceType actionGameConfigGenericInstance = weaverTypes.ActionTReference.MakeGenericInstanceType(weaverTypes.GameConfigReference);
            MethodReference actionGameConfigCtorReference = weaverTypes.ActionTCtorReference.MakeHostInstanceGeneric(weaverTypes.Assembly.MainModule, actionGameConfigGenericInstance);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Newobj, actionGameConfigCtorReference);

            GenericInstanceType gameConfigSelectionGenericInstance = weaverTypes.GameConfigSelectionReference.MakeGenericInstanceType(weaverTypes.GameConfigReference);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Callvirt, weaverTypes.GameConfigSelectionAddOnNewConfigSetReceiverReference.MakeHostInstanceGeneric(weaverTypes.Assembly.MainModule, gameConfigSelectionGenericInstance));

            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldarg_0);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldarg_0);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Call, pd.Property.GetMethod);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Call, onNewConfigSetMethod);

            onAwakeOnceReceiverWorker.Emit(OpCodes.Ret);

            td.Methods.Add(onAwakeOnceReceiverMethod);

            string onDestroyReceiverMethodName = $"OnDestroyReceiver__{Guid.NewGuid():N}";
            MethodDefinition onDestroyReceiverMethod = new(onDestroyReceiverMethodName, MethodAttributes.Private, weaverTypes.Assembly.MainModule.TypeSystem.Void);
            onDestroyReceiverMethod.Body = new(onDestroyReceiverMethod);

            ILProcessor onDestroyReceiverWorker = onDestroyReceiverMethod.Body.GetILProcessor();
            onDestroyReceiverWorker.Emit(OpCodes.Ldarg_0);
            onDestroyReceiverWorker.Emit(OpCodes.Ldfld, lastConfigField);
            onDestroyReceiverWorker.Emit(OpCodes.Ldarg_0);
            onDestroyReceiverWorker.Emit(OpCodes.Ldftn, onConfigChangedMethod);
            GenericInstanceType actionStringGenericInstance = weaverTypes.ActionTReference.MakeGenericInstanceType(weaverTypes.Assembly.MainModule.TypeSystem.String);
            MethodReference actionStringCtorReference = weaverTypes.ActionTCtorReference.MakeHostInstanceGeneric(weaverTypes.Assembly.MainModule, actionStringGenericInstance);
            onDestroyReceiverWorker.Emit(OpCodes.Newobj, actionStringCtorReference);
            onDestroyReceiverWorker.Emit(OpCodes.Call, weaverTypes.GameConfigRemoveOnBlendReceiverReference);

            onDestroyReceiverWorker.Emit(OpCodes.Ret);

            td.Methods.Add(onDestroyReceiverMethod);

            ILProcessor onNewConfigSetWorker = onNewConfigSetMethod.Body.GetILProcessor();
            onNewConfigSetWorker.Emit(OpCodes.Ldarg_0);
            onNewConfigSetWorker.Emit(OpCodes.Ldfld, lastConfigField);
            onNewConfigSetWorker.Emit(OpCodes.Ldarg_1);
            onNewConfigSetWorker.Emit(OpCodes.Ldarg_0);
            onNewConfigSetWorker.Emit(OpCodes.Ldftn, onConfigChangedMethod);
            onNewConfigSetWorker.Emit(OpCodes.Newobj, actionStringCtorReference);
            onNewConfigSetWorker.Emit(OpCodes.Call, weaverTypes.GameConfigAddOnBlendReceiverReference);

            onNewConfigSetWorker.Emit(OpCodes.Ldarg_0);
            onNewConfigSetWorker.Emit(OpCodes.Ldarg_1);
            onNewConfigSetWorker.Emit(OpCodes.Stfld, lastConfigField);

            onNewConfigSetWorker.Emit(OpCodes.Ret);

            td.Methods.Add(onNewConfigSetMethod);

            ILProcessor ctorWorker = ctor.Body.GetILProcessor();

            ctorWorker.Emit(OpCodes.Ldarg_0);
            ctorWorker.Emit(OpCodes.Ldarg_0);
            ctorWorker.Emit(OpCodes.Ldftn, onAwakeOnceReceiverMethod);
            ctorWorker.Emit(OpCodes.Newobj, weaverTypes.ActionCtorReference);
            ctorWorker.Emit(OpCodes.Callvirt, weaverTypes.ComponentAddOnAwakeOnceReceiverReference);

            ctorWorker.Emit(OpCodes.Ldarg_0);
            ctorWorker.Emit(OpCodes.Ldarg_0);
            ctorWorker.Emit(OpCodes.Ldftn, onDestroyReceiverMethod);
            ctorWorker.Emit(OpCodes.Newobj, weaverTypes.ActionCtorReference);
            ctorWorker.Emit(OpCodes.Callvirt, weaverTypes.ComponentAddOnDestroyReceiverReference);

            ctorWorker.Emit(OpCodes.Ret);

            return true;
        }
    }
}
