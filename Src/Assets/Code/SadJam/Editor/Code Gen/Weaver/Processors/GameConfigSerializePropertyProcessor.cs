using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.CecilX.Rocks;
using SadJam;
using System.Collections.Generic;
using System.Linq;

namespace SadJamEditor.Weaver
{
    public class GameConfigPropertyDefinition
    {
        public FieldDefinition Field { get; set; }
        public PropertyDefinition Property { get; set; }
    }

    public static class GameConfigSerializePropertyProcessor
    {
        public static bool Process(TypeDefinition td, WeaverTypes weaverTypes, Logger logger, ref bool weavingFailed, ref Dictionary<TypeDefinition, List<GameConfigPropertyDefinition>> properties)
        {
            bool modified = false;

            if (!td.HasProperties)
            {
                return modified;
            }

            List<GameConfigPropertyDefinition> props = new();
            properties[td] = props;

            for (int pDIndex = 0; pDIndex < td.Properties.Count; pDIndex++)
            {
                PropertyDefinition pd = td.Properties[pDIndex];

                if (!pd.HasCustomAttributes) continue;

                for (int cAIndex = 0; cAIndex < pd.CustomAttributes.Count; cAIndex++)
                {
                    CustomAttribute ca = pd.CustomAttributes[cAIndex];

                    if (ca.AttributeType.Is<GameConfigSerializePropertyAttribute>())
                    {
                        modified |= CreateField(td, pd, weaverTypes, logger, ref weavingFailed, out FieldDefinition field);
                        if (weavingFailed) break;

                        modified |= InjectPropertyGetter(pd, field, weaverTypes, logger, ref weavingFailed);

                        GameConfigPropertyDefinition gameConfigPropertyDefinition = new()
                        {
                            Field = field,
                            Property = pd,
                        };

                        props.Add(gameConfigPropertyDefinition);

                        break;
                    }
                }
            }

            return modified;
        }

        private static bool CreateField(TypeDefinition td, PropertyDefinition pd, WeaverTypes weaverTypes, Logger logger, ref bool weavingFailed, out FieldDefinition field)
        {
            string fieldName = $"{pd.Name}Selection";
            if (td.HasFields && td.Fields.Any(f => f.Name == fieldName))
            {
                logger.Error($"Type {td.FullName} already contains field with name {fieldName}! Please remove that field.", td);
                weavingFailed = true;
                field = default;

                return false;
            }

            Instruction ldFieldInstruction = pd.GetMethod.Body.Instructions.FirstOrDefault(i => i.OpCode == OpCodes.Ldfld);
            if (ldFieldInstruction == null)
            {
                logger.Error($"Ldfld instruction not found in property {pd.Name} getter!", td);
                weavingFailed = true;
                field = default;

                return false;
            }

            GenericInstanceType genericInstance = weaverTypes.GameConfigSelectionReference.MakeGenericInstanceType(pd.PropertyType);
            field = new(fieldName, FieldAttributes.Public, genericInstance);

            CustomAttribute serializeFieldAt = new(weaverTypes.SerializeFieldAttributeCtorReference);
            field.CustomAttributes.Add(serializeFieldAt);

            string oldFieldName = $"_{char.ToLower(pd.Name[0]) + pd.Name.Substring(1)}";

            CustomAttribute formalySerializedAs = new(weaverTypes.FormalySerializedAsAttributeReferenceCtorReference);
            formalySerializedAs.ConstructorArguments.Add(new(weaverTypes.Assembly.MainModule.TypeSystem.String, oldFieldName));

            field.CustomAttributes.Add(formalySerializedAs);

            FieldReference generatedFieldRef = (FieldReference)ldFieldInstruction.Operand;
            FieldDefinition generatedField = weaverTypes.Assembly.MainModule.ImportReference(generatedFieldRef).Resolve();

            foreach (CustomAttribute at in generatedField.CustomAttributes)
            {
                if (at.AttributeType.FullName == weaverTypes.FormalySerializedAsAttributeReferenceCtorReference.DeclaringType.FullName ||
                    at.AttributeType.FullName == weaverTypes.SerializeFieldAttributeCtorReference.DeclaringType.FullName)
                {
                    continue;
                }

                field.CustomAttributes.Add(at);
            }

            int fdIndex = 0;
            foreach (FieldDefinition fd in td.Fields)
            {
                if (fd.Name == generatedField.Name)
                {
                    td.Fields.Insert(fdIndex, field);

                    break;
                }

                fdIndex++;
            }

            return true;
        }

        private static bool InjectPropertyGetter(PropertyDefinition pd, FieldReference fd, WeaverTypes weaverTypes, Logger logger, ref bool weavingFailed)
        {
            MethodDefinition getterMethod = pd.GetMethod;

            if (getterMethod == null)
            {
                logger.Error($"{pd.Name} has invalid getter", pd);

                weavingFailed = true;
                return false;
            }

            getterMethod.Body = new(getterMethod);

            ILProcessor getterWorker = getterMethod.Body.GetILProcessor();
            getterWorker.Emit(OpCodes.Ldarg_0);
            getterWorker.Emit(OpCodes.Ldfld, fd);
            GenericInstanceType genericInstance = weaverTypes.GameConfigSelectionReference.MakeGenericInstanceType(weaverTypes.GameConfigReference);
            getterWorker.Emit(OpCodes.Callvirt, weaverTypes.GameConfigSelectionGetSelectionReference.MakeHostInstanceGeneric(weaverTypes.Assembly.MainModule, genericInstance));
            getterWorker.Emit(OpCodes.Castclass, pd.PropertyType);
            getterWorker.Emit(OpCodes.Ret);

            return true;
        }
    }
}
