using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.CecilX.Rocks;
using SadJam;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SadJamEditor.Weaver
{
    public static class OnNewGameConfigSetProcessor
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
                MethodDefinition onNewConfigSetMethod = td.Methods[mDIndex];

                if (!onNewConfigSetMethod.HasCustomAttributes || onNewConfigSetMethod.IsVirtual || onNewConfigSetMethod.IsSetter || onNewConfigSetMethod.IsGetter) continue;

                for (int cAIndex = 0; cAIndex < onNewConfigSetMethod.CustomAttributes.Count; cAIndex++)
                {
                    CustomAttribute ca = onNewConfigSetMethod.CustomAttributes[cAIndex];

                    if (ca.AttributeType.Is<OnNewGameConfigSetAttribute>())
                    {
                        if (!ca.HasConstructorArguments || ca.ConstructorArguments.Count <= 0)
                        {
                            logger.Error($"{nameof(OnNewGameConfigSetAttribute)} doesn't contain property name argument!", onNewConfigSetMethod);
                            continue;
                        }

                        string configPropName = (string)ca.ConstructorArguments[0].Value;

                        if (string.IsNullOrWhiteSpace(configPropName))
                        {
                            continue;
                        }

                        if (td.GetParentWithInterface(typeof(SadJam.IILComponent).FullName) == null)
                        {
                            logger.Warning($"Using {nameof(OnNewGameConfigSetAttribute)} is allowed only in {typeof(SadJam.IILComponent).FullName} classes!", td);
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
                                        modified |= InjectFieldOnNewConfigSetReceiver(td, onNewConfigSetMethod, pd, weaverTypes, logger, ref weavingFailed);
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

        private static bool InjectFieldOnNewConfigSetReceiver(TypeDefinition td, MethodDefinition onNewConfigSetMethod, GameConfigPropertyDefinition pd, WeaverTypes weaverTypes, Logger logger, ref bool weavingFailed)
        {
            MethodDefinition ctor = td.GetMethod(".ctor");
            if (ctor == null || !ctor.RemoveFinalRetInstruction())
            {
                logger.Error($"{td.Name} has invalid constructor", td);

                weavingFailed = true;
                return false;
            }

            string onNewConfigSetReceiverMethodName = $"OnNewConfigSetReceiver__{Guid.NewGuid():N}";
            MethodDefinition onNewConfigSetReceiverMethod = new(onNewConfigSetReceiverMethodName, MethodAttributes.Private, weaverTypes.Assembly.MainModule.TypeSystem.Void);
            onNewConfigSetReceiverMethod.Parameters.Add(new("gameconfig", ParameterAttributes.None, weaverTypes.GameConfigReference));
            onNewConfigSetReceiverMethod.Body = new(onNewConfigSetReceiverMethod);

            string onAwakeOnceReceiverMethodName = $"OnAwakeOnceReceiver__{Guid.NewGuid():N}";
            MethodDefinition onAwakeOnceReceiverMethod = new(onAwakeOnceReceiverMethodName, MethodAttributes.Private, weaverTypes.Assembly.MainModule.TypeSystem.Void);
            onAwakeOnceReceiverMethod.Body = new(onAwakeOnceReceiverMethod);

            ILProcessor onAwakeOnceReceiverWorker = onAwakeOnceReceiverMethod.Body.GetILProcessor();
            Instruction onAwakeOnceReceiverJumpToEnd = onAwakeOnceReceiverWorker.Create(OpCodes.Nop);

            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldarg_0);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldfld, pd.Field);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldarg_0);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldftn, onNewConfigSetReceiverMethod);
            GenericInstanceType actionGameConfigGenericInstance = weaverTypes.ActionTReference.MakeGenericInstanceType(weaverTypes.GameConfigReference);
            MethodReference actionGameConfigCtorReference = weaverTypes.ActionTCtorReference.MakeHostInstanceGeneric(weaverTypes.Assembly.MainModule, actionGameConfigGenericInstance);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Newobj, actionGameConfigCtorReference);

            GenericInstanceType gameConfigSelectionGenericInstance = weaverTypes.GameConfigSelectionReference.MakeGenericInstanceType(weaverTypes.GameConfigReference);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Callvirt, weaverTypes.GameConfigSelectionAddOnNewConfigSetReceiverReference.MakeHostInstanceGeneric(weaverTypes.Assembly.MainModule, gameConfigSelectionGenericInstance));

            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldarg_0);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Call, pd.Property.GetMethod);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Brfalse_S, onAwakeOnceReceiverJumpToEnd);

            onAwakeOnceReceiverWorker.Emit(OpCodes.Ldarg_0);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Call, onNewConfigSetMethod);

            onAwakeOnceReceiverWorker.Append(onAwakeOnceReceiverJumpToEnd);
            onAwakeOnceReceiverWorker.Emit(OpCodes.Ret);

            td.Methods.Add(onAwakeOnceReceiverMethod);

            ILProcessor onNewConfigSetWorker = onNewConfigSetReceiverMethod.Body.GetILProcessor();
            onNewConfigSetWorker.Emit(OpCodes.Ldarg_0);
            onNewConfigSetWorker.Emit(OpCodes.Call, onNewConfigSetMethod);

            onNewConfigSetWorker.Emit(OpCodes.Ret);

            td.Methods.Add(onNewConfigSetReceiverMethod);

            ILProcessor ctorWorker = ctor.Body.GetILProcessor();

            ctorWorker.Emit(OpCodes.Ldarg_0);
            ctorWorker.Emit(OpCodes.Ldarg_0);
            ctorWorker.Emit(OpCodes.Ldftn, onAwakeOnceReceiverMethod);
            ctorWorker.Emit(OpCodes.Newobj, weaverTypes.ActionCtorReference);
            ctorWorker.Emit(OpCodes.Callvirt, weaverTypes.ComponentAddOnAwakeOnceReceiverReference);
            ctorWorker.Emit(OpCodes.Ret);

            return true;
        }
    }
}

