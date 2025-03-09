using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.ParticleSystem;

namespace SadJam
{
    public abstract class GameConfig : ScriptableObject, IGameConfig, ISerializationCallbackReceiver
    {
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public class BlendableField : Attribute
        {
            public string Id { get; private set; }
            public BlendableField(string id)
            {
                Id = id;
            }
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class BlendableProperty : Attribute
        {
            public string Id { get; private set; }

            public BlendableProperty(string id)
            {
                Id = id;
            }
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class BlendablePropertyCorrespondingInterface : Attribute
        {
            public string CorrespondingInterfaceName { get; private set; }
            public string CorrespondingInterfacePropertyName { get; private set; }

            public BlendablePropertyCorrespondingInterface(string correspondingInterfaceName, string correspondingInterfacePropertyName)
            {
                CorrespondingInterfaceName = correspondingInterfaceName;
                CorrespondingInterfacePropertyName = correspondingInterfacePropertyName;
            }
        }

        [Serializable]
        public class BlendField
        {
            [FormerlySerializedAs("FieldName")]
            public string PropertyId;
            public char Operation;
            public PropertyNumericType NumericType;

            public BlendField() { }

            public BlendField(string propertyId)
            {
                PropertyId = propertyId;
            }

            public BlendField(string propertyId, char operation)
            {
                PropertyId = propertyId;
                Operation = operation;
            }

            public object InitializedOn { get; private set; }
            public Func<object> Getter { get; private set; }
            public Action<object> Setter { get; private set; }

            private static Dictionary<Type, Action<BlendField, object, PropertyInfo>> _setGetSetMap = new(28)
            {
                {
                    typeof(byte),
                    (bF, on, prop) =>
                    {
                        Func<byte> getter = (Func<byte>)Delegate.CreateDelegate(typeof(Func<byte>), on, prop.GetGetMethod(true));
                        Action<byte> setter = (Action<byte>)Delegate.CreateDelegate(typeof(Action<byte>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((byte)o);
                    }
                },
                {
                    typeof(sbyte),
                    (bF, on, prop) =>
                    {
                        Func<sbyte> getter = (Func<sbyte>)Delegate.CreateDelegate(typeof(Func<sbyte>), on, prop.GetGetMethod(true));
                        Action<sbyte> setter = (Action<sbyte>)Delegate.CreateDelegate(typeof(Action<sbyte>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((sbyte)o);
                    }
                },
                {
                    typeof(double),
                    (bF, on, prop) =>
                    {
                        Func<double> getter = (Func<double>)Delegate.CreateDelegate(typeof(Func<double>), on, prop.GetGetMethod(true));
                        Action<double> setter = (Action<double>)Delegate.CreateDelegate(typeof(Action<double>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((double)o);
                    }
                },
                {
                    typeof(int),
                    (bF, on, prop) =>
                    {
                        Func<int> getter = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), on, prop.GetGetMethod(true));
                        Action<int> setter = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((int)o);
                    }
                },
                {
                    typeof(uint),
                    (bF, on, prop) =>
                    {
                        Func<uint> getter = (Func<uint>)Delegate.CreateDelegate(typeof(Func<uint>), on, prop.GetGetMethod(true));
                        Action<uint> setter = (Action<uint>)Delegate.CreateDelegate(typeof(Action<uint>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((uint)o);
                    }
                },
                {
                    typeof(short),
                    (bF, on, prop) =>
                    {
                        Func<short> getter = (Func<short>)Delegate.CreateDelegate(typeof(Func<short>), on, prop.GetGetMethod(true));
                        Action<short> setter = (Action<short>)Delegate.CreateDelegate(typeof(Action<short>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((short)o);
                    }
                },
                {
                    typeof(ushort),
                    (bF, on, prop) =>
                    {
                        Func<ushort> getter = (Func<ushort>)Delegate.CreateDelegate(typeof(Func<ushort>), on, prop.GetGetMethod(true));
                        Action<ushort> setter = (Action<ushort>)Delegate.CreateDelegate(typeof(Action<ushort>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((ushort)o);
                    }
                },
                {
                    typeof(float),
                    (bF, on, prop) =>
                    {
                        Func<float> getter = (Func<float>)Delegate.CreateDelegate(typeof(Func<float>), on, prop.GetGetMethod(true));
                        Action<float> setter = (Action<float>)Delegate.CreateDelegate(typeof(Action<float>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((float)o);
                    }
                },
                {
                    typeof(ulong),
                    (bF, on, prop) =>
                    {
                        Func<ulong> getter = (Func<ulong>)Delegate.CreateDelegate(typeof(Func<ulong>), on, prop.GetGetMethod(true));
                        Action<ulong> setter = (Action<ulong>)Delegate.CreateDelegate(typeof(Action<ulong>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((ulong)o);
                    }
                },
                {
                    typeof(long),
                    (bF, on, prop) =>
                    {
                        Func<long> getter = (Func<long>)Delegate.CreateDelegate(typeof(Func<long>), on, prop.GetGetMethod(true));
                        Action<long> setter = (Action<long>)Delegate.CreateDelegate(typeof(Action<long>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((long)o);
                    }
                },
                {
                    typeof(Vector2),
                    (bF, on, prop) =>
                    {
                        Func<Vector2> getter = (Func<Vector2>)Delegate.CreateDelegate(typeof(Func<Vector2>), on, prop.GetGetMethod(true));
                        Action<Vector2> setter = (Action<Vector2>)Delegate.CreateDelegate(typeof(Action<Vector2>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Vector2)o);
                    }
                },
                {
                    typeof(Vector3),
                    (bF, on, prop) =>
                    {
                        Func<Vector3> getter = (Func<Vector3>)Delegate.CreateDelegate(typeof(Func<Vector3>), on, prop.GetGetMethod(true));
                        Action<Vector3> setter = (Action<Vector3>)Delegate.CreateDelegate(typeof(Action<Vector3>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Vector3)o);
                    }
                },
                {
                    typeof(Vector4),
                    (bF, on, prop) =>
                    {
                        Func<Vector4> getter = (Func<Vector4>)Delegate.CreateDelegate(typeof(Func<Vector4>), on, prop.GetGetMethod(true));
                        Action<Vector4> setter = (Action<Vector4>)Delegate.CreateDelegate(typeof(Action<Vector4>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Vector4)o);
                    }
                },
                {
                    typeof(Vector2Int),
                    (bF, on, prop) =>
                    {
                        Func<Vector2Int> getter = (Func<Vector2Int>)Delegate.CreateDelegate(typeof(Func<Vector2Int>), on, prop.GetGetMethod(true));
                        Action<Vector2Int> setter = (Action<Vector2Int>)Delegate.CreateDelegate(typeof(Action<Vector2Int>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Vector2Int)o);
                    }
                },
                {
                    typeof(Vector3Int),
                    (bF, on, prop) =>
                    {
                        Func<Vector3Int> getter = (Func<Vector3Int>)Delegate.CreateDelegate(typeof(Func<Vector3Int>), on, prop.GetGetMethod(true));
                        Action<Vector3Int> setter = (Action<Vector3Int>)Delegate.CreateDelegate(typeof(Action<Vector3Int>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Vector3Int)o);
                    }
                },
                {
                    typeof(Quaternion),
                    (bF, on, prop) =>
                    {
                        Func<Quaternion> getter = (Func<Quaternion>)Delegate.CreateDelegate(typeof(Func<Quaternion>), on, prop.GetGetMethod(true));
                        Action<Quaternion> setter = (Action<Quaternion>)Delegate.CreateDelegate(typeof(Action<Quaternion>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Quaternion)o);
                    }
                },
                {
                    typeof(Rect),
                    (bF, on, prop) =>
                    {
                        Func<Rect> getter = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), on, prop.GetGetMethod(true));
                        Action<Rect> setter = (Action<Rect>)Delegate.CreateDelegate(typeof(Action<Rect>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Rect)o);
                    }
                },
                {
                    typeof(GameObject),
                    (bF, on, prop) =>
                    {
                        Func<GameObject> getter = (Func<GameObject>)Delegate.CreateDelegate(typeof(Func<GameObject>), on, prop.GetGetMethod(true));
                        Action<GameObject> setter = (Action<GameObject>)Delegate.CreateDelegate(typeof(Action<GameObject>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((GameObject)o);
                    }
                },
                {
                    typeof(AnimationClip),
                    (bF, on, prop) =>
                    {
                        Func<AnimationClip> getter = (Func<AnimationClip>)Delegate.CreateDelegate(typeof(Func<AnimationClip>), on, prop.GetGetMethod(true));
                        Action<AnimationClip> setter = (Action<AnimationClip>)Delegate.CreateDelegate(typeof(Action<AnimationClip>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((AnimationClip)o);
                    }
                },
                {
                    typeof(AnimationCurve),
                    (bF, on, prop) =>
                    {
                        Func<AnimationCurve> getter = (Func<AnimationCurve>)Delegate.CreateDelegate(typeof(Func<AnimationCurve>), on, prop.GetGetMethod(true));
                        Action<AnimationCurve> setter = (Action<AnimationCurve>)Delegate.CreateDelegate(typeof(Action<AnimationCurve>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((AnimationCurve)o);
                    }
                },
                {
                    typeof(MinMaxCurve),
                    (bF, on, prop) =>
                    {
                        Func<MinMaxCurve> getter = (Func<MinMaxCurve>)Delegate.CreateDelegate(typeof(Func<MinMaxCurve>), on, prop.GetGetMethod(true));
                        Action<MinMaxCurve> setter = (Action<MinMaxCurve>)Delegate.CreateDelegate(typeof(Action<MinMaxCurve>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((MinMaxCurve)o);
                    }
                },
                {
                    typeof(bool),
                    (bF, on, prop) =>
                    {
                        Func<bool> getter = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), on, prop.GetGetMethod(true));
                        Action<bool> setter = (Action<bool>)Delegate.CreateDelegate(typeof(Action<bool>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((bool)o);
                    }
                },
                {
                    typeof(Char),
                    (bF, on, prop) =>
                    {
                        Func<char> getter = (Func<char>)Delegate.CreateDelegate(typeof(Func<char>), on, prop.GetGetMethod(true));
                        Action<char> setter = (Action<char>)Delegate.CreateDelegate(typeof(Action<char>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((char)o);
                    }
                },
                {
                    typeof(string),
                    (bF, on, prop) =>
                    {
                        Func<string> getter = (Func<string>)Delegate.CreateDelegate(typeof(Func<string>), on, prop.GetGetMethod(true));
                        Action<string> setter = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((string)o);
                    }
                },
                {
                    typeof(Color),
                    (bF, on, prop) =>
                    {
                        Func<Color> getter = (Func<Color>)Delegate.CreateDelegate(typeof(Func<Color>), on, prop.GetGetMethod(true));
                        Action<Color> setter = (Action<Color>)Delegate.CreateDelegate(typeof(Action<Color>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Color)o);
                    }
                },
                {
                    typeof(LayerMask),
                    (bF, on, prop) =>
                    {
                        Func<LayerMask> getter = (Func<LayerMask>)Delegate.CreateDelegate(typeof(Func<LayerMask>), on, prop.GetGetMethod(true));
                        Action<LayerMask> setter = (Action<LayerMask>)Delegate.CreateDelegate(typeof(Action<LayerMask>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((LayerMask)o);
                    }
                },
                {
                    typeof(Matrix4x4),
                    (bF, on, prop) =>
                    {
                        Func<Matrix4x4> getter = (Func<Matrix4x4>)Delegate.CreateDelegate(typeof(Func<Matrix4x4>), on, prop.GetGetMethod(true));
                        Action<Matrix4x4> setter = (Action<Matrix4x4>)Delegate.CreateDelegate(typeof(Action<Matrix4x4>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Matrix4x4)o);
                    }
                },
                {
                    typeof(Texture2D),
                    (bF, on, prop) =>
                    {
                        Func<Texture2D> getter = (Func<Texture2D>)Delegate.CreateDelegate(typeof(Func<Texture2D>), on, prop.GetGetMethod(true));
                        Action<Texture2D> setter = (Action<Texture2D>)Delegate.CreateDelegate(typeof(Action<Texture2D>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Texture2D)o);
                    }
                },               
                {
                    typeof(Sprite),
                    (bF, on, prop) =>
                    {
                        Func<Sprite> getter = (Func<Sprite>)Delegate.CreateDelegate(typeof(Func<Sprite>), on, prop.GetGetMethod(true));
                        Action<Sprite> setter = (Action<Sprite>)Delegate.CreateDelegate(typeof(Action<Sprite>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((Sprite)o);
                    }
                }
            };
            private static Dictionary<Type, Action<BlendField, object, PropertyInfo>> _setGetSetListMap = new(28)
            {
                {
                    typeof(byte),
                    (bF, on, prop) =>
                    {
                        Func<List<byte>> getter = (Func<List<byte>>)Delegate.CreateDelegate(typeof(Func<List<byte>>), on, prop.GetGetMethod(true));
                        Action<List<byte>> setter = (Action<List<byte>>)Delegate.CreateDelegate(typeof(Action<List<byte>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<byte>)o);
                    }
                },
                {
                    typeof(sbyte),
                    (bF, on, prop) =>
                    {
                        Func<List<sbyte>> getter = (Func<List<sbyte>>)Delegate.CreateDelegate(typeof(Func<List<sbyte>>), on, prop.GetGetMethod(true));
                        Action<List<sbyte>> setter = (Action<List<sbyte>>)Delegate.CreateDelegate(typeof(Action<List<sbyte>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<sbyte>)o);
                    }
                },
                {
                    typeof(double),
                    (bF, on, prop) =>
                    {
                        Func<List<double>> getter = (Func<List<double>>)Delegate.CreateDelegate(typeof(Func<List<double>>), on, prop.GetGetMethod(true));
                        Action<List<double>> setter = (Action<List<double>>)Delegate.CreateDelegate(typeof(Action<List<double>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<double>)o);
                    }
                },
                {
                    typeof(int),
                    (bF, on, prop) =>
                    {
                        Func<List<int>> getter = (Func<List<int>>)Delegate.CreateDelegate(typeof(Func<List<int>>), on, prop.GetGetMethod(true));
                        Action<List<int>> setter = (Action<List<int>>)Delegate.CreateDelegate(typeof(Action<List<int>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<int>)o);
                    }
                },
                {
                    typeof(uint),
                    (bF, on, prop) =>
                    {
                        Func<List<uint>> getter = (Func<List<uint>>)Delegate.CreateDelegate(typeof(Func<List<uint>>), on, prop.GetGetMethod(true));
                        Action<List<uint>> setter = (Action<List<uint>>)Delegate.CreateDelegate(typeof(Action<List<uint>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<uint>)o);
                    }
                },
                {
                    typeof(short),
                    (bF, on, prop) =>
                    {
                        Func<List<short>> getter = (Func<List<short>>)Delegate.CreateDelegate(typeof(Func<List<short>>), on, prop.GetGetMethod(true));
                        Action<List<short>> setter = (Action<List<short>>)Delegate.CreateDelegate(typeof(Action<List<short>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<short>)o);
                    }
                },
                {
                    typeof(ushort),
                    (bF, on, prop) =>
                    {
                        Func<List<ushort>> getter = (Func<List<ushort>>)Delegate.CreateDelegate(typeof(Func<List<ushort>>), on, prop.GetGetMethod(true));
                        Action<List<ushort>> setter = (Action<List<ushort>>)Delegate.CreateDelegate(typeof(Action<List<ushort>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<ushort>)o);
                    }
                },
                {
                    typeof(float),
                    (bF, on, prop) =>
                    {
                        Func<List<float>> getter = (Func<List<float>>)Delegate.CreateDelegate(typeof(Func<List<float>>), on, prop.GetGetMethod(true));
                        Action<List<float>> setter = (Action<List<float>>)Delegate.CreateDelegate(typeof(Action<List<float>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<float>)o);
                    }
                },
                {
                    typeof(ulong),
                    (bF, on, prop) =>
                    {
                        Func<List<ulong>> getter = (Func<List<ulong>>)Delegate.CreateDelegate(typeof(Func<List<ulong>>), on, prop.GetGetMethod(true));
                        Action<List<ulong>> setter = (Action<List<ulong>>)Delegate.CreateDelegate(typeof(Action<List<ulong>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<ulong>)o);
                    }
                },
                {
                    typeof(long),
                    (bF, on, prop) =>
                    {
                        Func<List<long>> getter = (Func<List<long>>)Delegate.CreateDelegate(typeof(Func<List<long>>), on, prop.GetGetMethod(true));
                        Action<List<long>> setter = (Action<List<long>>)Delegate.CreateDelegate(typeof(Action<List<long>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < long >)o);
                    }
                },
                {
                    typeof(Vector2),
                    (bF, on, prop) =>
                    {
                        Func<List<Vector2>> getter = (Func<List<Vector2>>)Delegate.CreateDelegate(typeof(Func<List<Vector2>>), on, prop.GetGetMethod(true));
                        Action<List<Vector2>> setter = (Action<List<Vector2>>)Delegate.CreateDelegate(typeof(Action<List<Vector2>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<Vector2>)o);
                    }
                },
                {
                    typeof(Vector3),
                    (bF, on, prop) =>
                    {
                        Func<List<Vector3>> getter = (Func<List<Vector3>>)Delegate.CreateDelegate(typeof(Func<List<Vector3>>), on, prop.GetGetMethod(true));
                        Action<List<Vector3>> setter = (Action<List<Vector3>>)Delegate.CreateDelegate(typeof(Action<List<Vector3>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List <Vector3>)o);
                    }
                },
                {
                    typeof(Vector4),
                    (bF, on, prop) =>
                    {
                        Func<List<Vector4>> getter = (Func<List<Vector4>>)Delegate.CreateDelegate(typeof(Func<List<Vector4>>), on, prop.GetGetMethod(true));
                        Action<List<Vector4>> setter = (Action<List<Vector4>>)Delegate.CreateDelegate(typeof(Action<List<Vector4>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < Vector4 >)o);
                    }
                },
                                {
                    typeof(Vector2Int),
                    (bF, on, prop) =>
                    {
                        Func<List<Vector2Int>> getter = (Func<List<Vector2Int>>)Delegate.CreateDelegate(typeof(Func<List<Vector2Int>>), on, prop.GetGetMethod(true));
                        Action<List<Vector2Int>> setter = (Action<List<Vector2Int>>)Delegate.CreateDelegate(typeof(Action<List<Vector2Int>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List<Vector2Int>)o);
                    }
                },
                {
                    typeof(Vector3Int),
                    (bF, on, prop) =>
                    {
                        Func<List<Vector3Int>> getter = (Func<List<Vector3Int>>)Delegate.CreateDelegate(typeof(Func<List<Vector3Int>>), on, prop.GetGetMethod(true));
                        Action<List<Vector3Int>> setter = (Action<List<Vector3Int>>)Delegate.CreateDelegate(typeof(Action<List<Vector3Int>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List <Vector3Int>)o);
                    }
                },
                {
                    typeof(Quaternion),
                    (bF, on, prop) =>
                    {
                        Func<List<Quaternion>> getter = (Func<List<Quaternion>>)Delegate.CreateDelegate(typeof(Func<List<Quaternion>>), on, prop.GetGetMethod(true));
                        Action<List<Quaternion>> setter = (Action<List<Quaternion>>)Delegate.CreateDelegate(typeof(Action<List<Quaternion>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < Quaternion >)o);
                    }
                },
                {
                    typeof(Rect),
                    (bF, on, prop) =>
                    {
                        Func<List<Rect>> getter = (Func<List<Rect>>)Delegate.CreateDelegate(typeof(Func<List<Rect>>), on, prop.GetGetMethod(true));
                        Action<List<Rect>> setter = (Action<List<Rect>>)Delegate.CreateDelegate(typeof(Action<List<Rect>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < Rect >)o);
                    }
                },
                {
                    typeof(GameObject),
                    (bF, on, prop) =>
                    {
                        Func<List<GameObject>> getter = (Func<List<GameObject>>)Delegate.CreateDelegate(typeof(Func<List<GameObject>>), on, prop.GetGetMethod(true));
                        Action<List<GameObject>> setter = (Action<List<GameObject>>)Delegate.CreateDelegate(typeof(Action<List<GameObject>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < GameObject >)o);
                    }
                },
                {
                    typeof(AnimationClip),
                    (bF, on, prop) =>
                    {
                        Func<List<AnimationClip>> getter = (Func<List<AnimationClip>>)Delegate.CreateDelegate(typeof(Func<List<AnimationClip>>), on, prop.GetGetMethod(true));
                        Action<List<AnimationClip>> setter = (Action<List<AnimationClip>>)Delegate.CreateDelegate(typeof(Action<List<AnimationClip>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < AnimationClip >)o);
                    }
                },
                {
                    typeof(AnimationCurve),
                    (bF, on, prop) =>
                    {
                        Func<List<AnimationCurve>> getter = (Func<List<AnimationCurve>>)Delegate.CreateDelegate(typeof(Func<List<AnimationCurve>>), on, prop.GetGetMethod(true));
                        Action<List<AnimationCurve>> setter = (Action<List<AnimationCurve>>)Delegate.CreateDelegate(typeof(Action<List<AnimationCurve>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < AnimationCurve >)o);
                    }
                },
                {
                    typeof(MinMaxCurve),
                    (bF, on, prop) =>
                    {
                        Func<List<MinMaxCurve>> getter = (Func<List<MinMaxCurve>>)Delegate.CreateDelegate(typeof(Func<List<MinMaxCurve>>), on, prop.GetGetMethod(true));
                        Action<List<MinMaxCurve>> setter = (Action<List<MinMaxCurve>>)Delegate.CreateDelegate(typeof(Action<List<MinMaxCurve>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < MinMaxCurve >)o);
                    }
                },
                {
                    typeof(bool),
                    (bF, on, prop) =>
                    {
                        Func<List<bool>> getter = (Func<List<bool>>)Delegate.CreateDelegate(typeof(Func<List<bool>>), on, prop.GetGetMethod(true));
                        Action<List<bool>> setter = (Action<List<bool>>)Delegate.CreateDelegate(typeof(Action<List<bool>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < bool >)o);
                    }
                },
                {
                    typeof(char),
                    (bF, on, prop) =>
                    {
                        Func<List<char>> getter = (Func<List<char>>)Delegate.CreateDelegate(typeof(Func<List<char>>), on, prop.GetGetMethod(true));
                        Action<List<char>> setter = (Action<List<char>>)Delegate.CreateDelegate(typeof(Action<List<char>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < char >)o);
                    }
                },
                {
                    typeof(string),
                    (bF, on, prop) =>
                    {
                        Func<List<string>> getter = (Func<List<string>>)Delegate.CreateDelegate(typeof(Func<List<string>>), on, prop.GetGetMethod(true));
                        Action<List<string>> setter = (Action<List<string>>)Delegate.CreateDelegate(typeof(Action<List<string>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < string >)o);
                    }
                },
                {
                    typeof(Color),
                    (bF, on, prop) =>
                    {
                        Func<List<Color>> getter = (Func<List<Color>>)Delegate.CreateDelegate(typeof(Func<List<Color>>), on, prop.GetGetMethod(true));
                        Action<List<Color>> setter = (Action<List<Color>>)Delegate.CreateDelegate(typeof(Action<List<Color>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < Color >)o);
                    }
                },
                {
                    typeof(LayerMask),
                    (bF, on, prop) =>
                    {
                        Func<List<LayerMask>> getter = (Func<List<LayerMask>>)Delegate.CreateDelegate(typeof(Func<List<LayerMask>>), on, prop.GetGetMethod(true));
                        Action<List<LayerMask>> setter = (Action<List<LayerMask>>)Delegate.CreateDelegate(typeof(Action<List<LayerMask>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < LayerMask >)o);
                    }
                },
                {
                    typeof(Matrix4x4),
                    (bF, on, prop) =>
                    {
                        Func<List<Matrix4x4>> getter = (Func<List<Matrix4x4>>)Delegate.CreateDelegate(typeof(Func<List<Matrix4x4>>), on, prop.GetGetMethod(true));
                        Action<List<Matrix4x4>> setter = (Action<List<Matrix4x4>>)Delegate.CreateDelegate(typeof(Action<List<Matrix4x4>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < Matrix4x4 >)o);
                    }
                },
                {
                    typeof(Texture2D),
                    (bF, on, prop) =>
                    {
                        Func<List<Texture2D>> getter = (Func<List<Texture2D>>)Delegate.CreateDelegate(typeof(Func<List<Texture2D>>), on, prop.GetGetMethod(true));
                        Action<List<Texture2D>> setter = (Action<List<Texture2D>>)Delegate.CreateDelegate(typeof(Action<List<Texture2D>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List < Texture2D >)o);
                    }
                },
                {
                    typeof(Sprite),
                    (bF, on, prop) =>
                    {
                        Func<List<Sprite>> getter = (Func<List<Sprite>>)Delegate.CreateDelegate(typeof(Func<List<Sprite>>), on, prop.GetGetMethod(true));
                        Action<List<Sprite>> setter = (Action<List<Sprite>>)Delegate.CreateDelegate(typeof(Action<List<Sprite>>), on, prop.GetSetMethod(true));

                        bF.Getter = () => getter();
                        bF.Setter = (object o) => setter((List <Sprite>)o);
                    }
                }
            };

            public PropertyInfo PropertyInfo { get; private set; }
            public BlendableProperty PropertyAttribute { get; private set; }
            public FieldInfo LinkedFieldInfo { get; private set; }
            public Type TargetType { get; private set; }
            public IEnumerable<BlendablePropertyCorrespondingInterface> CorrespondingInterfaces { get; private set; }

            private static Dictionary<object, CacheData> _cache = new();
            private struct CacheData
            {
                public List<KeyValuePair<FieldInfo, BlendableField>> Fields;
                public List<KeyValuePair<PropertyInfo, BlendableProperty>> Properties;
            }

            public bool Init(object initializeOn)
            {
                if (InitializedOn == initializeOn) return true;

                Type targetType = initializeOn.GetType();
                CacheData cacheData;
                if (!_cache.TryGetValue(initializeOn, out cacheData))
                {
                    cacheData = new()
                    {
                        Fields = new()
                    };
                    foreach (FieldInfo f in targetType.GetAllFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        BlendableField att = f.GetCustomAttribute<BlendableField>();

                        if (att != null)
                        {
                            cacheData.Fields.Add(new(f, att));
                        }
                    }

                    cacheData.Properties = new();
                    foreach (PropertyInfo p in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        BlendableProperty att = p.GetCustomAttribute<BlendableProperty>();

                        if (att != null)
                        {
                            cacheData.Properties.Add(new(p, att));
                        }
                    }

                    _cache[initializeOn] = cacheData;
                }

                PropertyInfo propertyInfo = null;
                BlendableProperty propertyAttribute = null;
                foreach (KeyValuePair<PropertyInfo, BlendableProperty> p in cacheData.Properties)
                {
                    if (p.Value.Id == PropertyId) 
                    {
                        propertyInfo = p.Key;
                        propertyAttribute = p.Value;
                        break;
                    }
                }

                if (propertyInfo == null) return false;

                FieldInfo linkedField = null;
                foreach (KeyValuePair<FieldInfo, BlendableField> f in cacheData.Fields)
                {
                    if (f.Value.Id == PropertyId)
                    {
                        linkedField = f.Key;
                    }
                }

                if (linkedField == null)
                {
                    Debug.LogError("Linked field not found! Property id " + propertyAttribute.Id);
                    return false;
                }

                Action<BlendField, object, PropertyInfo> setter;
                if (propertyInfo.PropertyType.IsGenericType && !propertyInfo.PropertyType.IsArray && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    if (!_setGetSetListMap.TryGetValue(propertyInfo.PropertyType, out setter))
                    {
                        Getter = () => propertyInfo.GetValue(initializeOn);
                        Setter = (object o) => propertyInfo.SetValue(initializeOn, o);
                    }
                    else
                    {
                        setter(this, initializeOn, propertyInfo);
                    }
                }
                else
                {
                    if (!_setGetSetMap.TryGetValue(propertyInfo.PropertyType, out setter))
                    {
                        Getter = () => propertyInfo.GetValue(initializeOn);
                        Setter = (object o) => propertyInfo.SetValue(initializeOn, o);
                    }
                    else
                    {
                        setter(this, initializeOn, propertyInfo);
                    }
                }

                InitializedOn = initializeOn;
                LinkedFieldInfo = linkedField;
                PropertyInfo = propertyInfo;
                CorrespondingInterfaces = PropertyInfo.GetCustomAttributes<BlendablePropertyCorrespondingInterface>();
                PropertyAttribute = propertyAttribute;
                TargetType = targetType;

                return true;
            }

            public bool ResetToDefault()
            {
                if (InitializedOn == null || LinkedFieldInfo == null) return false;

                Setter(LinkedFieldInfo.GetValue(InitializedOn));

                return true;
            }
        }

        public enum PropertyNumericType
        {
            Unknown = 0,
            Int8 = 1,
            UInt8 = 2,
            Int16 = 3,
            UInt16 = 4,
            Int32 = 5,
            UInt32 = 6,
            Int64 = 7,
            UInt64 = 8,
            Float = 100,
            Double = 101
        }

        public static Action OnAllAboutToReset { get; set; }
        public static Action OnAllReseted { get; set; }
        public Action<IGameConfig> OnReseted { get; set; }
        public Action<IGameConfig> OnAboutToReset { get; set; }
        public Action<string> OnBlend { get; set; }

        [field: DebugOnly, SerializeField, FormerlySerializedAs("FieldsToBlend")]
        public List<BlendField> FieldsToBlend { get; private set; }

        private static List<GameConfig> _instances = new();

        [NonSerialized]
        private List<BlendField> _blendFields = new();
        protected virtual void OnEnable()
        {
            _instances.Add(this);

            OnConfigResetedToDefault();
            OnReseted?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            _instances.Remove(this);
        }

        protected virtual void OnValidate()
        {

        }

        public virtual void OnBeforeSerialize()
        {

        }

        [NonSerialized]
        private bool _initialized = false;
        public virtual void OnAfterDeserialize()
        {
            if (!_initialized)
            {
                _initialized = true;

                InitBlendFields();
            }

            foreach(BlendField f in _blendFields)
            {
                f.ResetToDefault();
            }
        }

        private void InitBlendFields()
        {
            Type t = GetType();
            foreach (PropertyInfo prop in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                BlendableProperty propAt = prop.GetCustomAttribute<BlendableProperty>();
                if (propAt == null) continue;

                BlendableField fieldAt = null;
                foreach (FieldInfo field in t.GetAllFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    BlendableField at = field.GetCustomAttribute<BlendableField>();

                    if (at != null && at.Id == propAt.Id) 
                    {
                        fieldAt = at;
                        break;
                    }
                }

                if (fieldAt == null)
                {
                    Debug.LogError("Field with same id as BlendableProperty attribute id: " + propAt.Id + " not found!", this);
                    continue;
                }

                BlendField f = new()
                {
                    PropertyId = propAt.Id
                };

                if (!f.Init(this))
                {
                    Debug.LogError("Failed to initialize blendable property " + prop.Name + "!", this);
                    continue;
                }

                _blendFields.Add(f);
            }

            foreach (BlendField field in FieldsToBlend)
            {
                if (!field.Init(this))
                {
                    Debug.LogError("Failed to initialize blendable property to blend with id " + field.PropertyId + "!", this);
                }
            }
        }

        public void AddFieldToBlend(BlendField field)
        {
            if (!FieldsToBlend.Contains(field))
            {
                FieldsToBlend.Add(field);
            }
        }

        public void RemoveFieldFromBlend(BlendField field)
        {
            FieldsToBlend.Remove(field);
        }
#region MathOperations
        private static Dictionary<PropertyNumericType, Func<object, object, object>> _divOperationMap = new(10)
        {
            {
                PropertyNumericType.Double,
                (valueBefore, valueAfter)=>
                {
                    return ((double)valueBefore) / (double)valueAfter;
                }
            },
            {
                PropertyNumericType.Float,
                (valueBefore, valueAfter)=>
                {
                    return ((float)valueBefore) / (float)valueAfter;
                }
            },
            {
                PropertyNumericType.Int16,
                (valueBefore, valueAfter)=>
                {
                    return ((Int16)valueBefore) / (Int16)valueAfter;
                }
            },
            {
                PropertyNumericType.Int32,
                (valueBefore, valueAfter)=>
                {
                    return ((Int32)valueBefore) / (Int32)valueAfter;
                }
            },
            {
                PropertyNumericType.Int64,
                (valueBefore, valueAfter)=>
                {
                    return ((Int64)valueBefore) / (Int64)valueAfter;
                }
            },
            {
                PropertyNumericType.Int8,
                (valueBefore, valueAfter)=>
                {
                    return ((sbyte)valueBefore) / (sbyte)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt16,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt16)valueBefore) / (UInt16)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt32,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt32)valueBefore) / (UInt32)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt64,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt64)valueBefore) / (UInt64)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt8,
                (valueBefore, valueAfter)=>
                {
                    return ((byte)valueBefore) / (byte)valueAfter;
                }
            }
        };
        private static Dictionary<PropertyNumericType, Func<object, object, object>> _mulOperationMap = new(10)
        {
            {
                PropertyNumericType.Double,
                (valueBefore, valueAfter)=>
                {
                    return ((double)valueBefore) * (double)valueAfter;
                }
            },
            {
                PropertyNumericType.Float,
                (valueBefore, valueAfter)=>
                {
                    return ((float)valueBefore) * (float)valueAfter;
                }
            },
            {
                PropertyNumericType.Int16,
                (valueBefore, valueAfter)=>
                {
                    return ((Int16)valueBefore) * (Int16)valueAfter;
                }
            },
            {
                PropertyNumericType.Int32,
                (valueBefore, valueAfter)=>
                {
                    return ((Int32)valueBefore) * (Int32)valueAfter;
                }
            },
            {
                PropertyNumericType.Int64,
                (valueBefore, valueAfter)=>
                {
                    return ((Int64)valueBefore) * (Int64)valueAfter;
                }
            },
            {
                PropertyNumericType.Int8,
                (valueBefore, valueAfter)=>
                {
                    return ((sbyte)valueBefore) * (sbyte)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt16,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt16)valueBefore) * (UInt16)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt32,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt32)valueBefore) * (UInt32)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt64,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt64)valueBefore) * (UInt64)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt8,
                (valueBefore, valueAfter)=>
                {
                    return ((byte)valueBefore) * (byte)valueAfter;
                }
            }
        };
        private static Dictionary<PropertyNumericType, Func<object, object, object>> _subOperationMap = new(10)
        {
            {
                PropertyNumericType.Double,
                (valueBefore, valueAfter)=>
                {
                    return ((double)valueBefore) - (double)valueAfter;
                }
            },
            {
                PropertyNumericType.Float,
                (valueBefore, valueAfter)=>
                {
                    return ((float)valueBefore) - (float)valueAfter;
                }
            },
            {
                PropertyNumericType.Int16,
                (valueBefore, valueAfter)=>
                {
                    return ((Int16)valueBefore) - (Int16)valueAfter;
                }
            },
            {
                PropertyNumericType.Int32,
                (valueBefore, valueAfter)=>
                {
                    return ((Int32)valueBefore) - (Int32)valueAfter;
                }
            },
            {
                PropertyNumericType.Int64,
                (valueBefore, valueAfter)=>
                {
                    return ((Int64)valueBefore) - (Int64)valueAfter;
                }
            },
            {
                PropertyNumericType.Int8,
                (valueBefore, valueAfter)=>
                {
                    return ((sbyte)valueBefore) - (sbyte)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt16,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt16)valueBefore) - (UInt16)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt32,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt32)valueBefore) - (UInt32)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt64,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt64)valueBefore) - (UInt64)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt8,
                (valueBefore, valueAfter)=>
                {
                    return ((byte)valueBefore) - (byte)valueAfter;
                }
            }
        };
        private static Dictionary<PropertyNumericType, Func<object, object, object>> _addOperationMap = new(10)
        {
            {
                PropertyNumericType.Double,
                (valueBefore, valueAfter)=>
                {
                    return ((double)valueBefore) + (double)valueAfter;
                }
            },
            {
                PropertyNumericType.Float,
                (valueBefore, valueAfter)=>
                {
                    return ((float)valueBefore) + (float)valueAfter;
                }
            },
            {
                PropertyNumericType.Int16,
                (valueBefore, valueAfter)=>
                {
                    return ((Int16)valueBefore) + (Int16)valueAfter;
                }
            },
            {
                PropertyNumericType.Int32,
                (valueBefore, valueAfter)=>
                {
                    return ((Int32)valueBefore) + (Int32)valueAfter;
                }
            },
            {
                PropertyNumericType.Int64,
                (valueBefore, valueAfter)=>
                {
                    return ((Int64)valueBefore) + (Int64)valueAfter;
                }
            },
            {
                PropertyNumericType.Int8,
                (valueBefore, valueAfter)=>
                {
                    return ((sbyte)valueBefore) + (sbyte)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt16,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt16)valueBefore) + (UInt16)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt32,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt32)valueBefore) + (UInt32)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt64,
                (valueBefore, valueAfter)=>
                {
                    return ((UInt64)valueBefore) + (UInt64)valueAfter;
                }
            },
            {
                PropertyNumericType.UInt8,
                (valueBefore, valueAfter)=>
                {
                    return ((byte)valueBefore) + (byte)valueAfter;
                }
            }
        };

        private static Dictionary<char, Func<object, object, PropertyNumericType, object>> _mathOperationSelectionMap = new(4)
        {
            {
                '+',
                (valBefore, valAfter, numericType) =>
                {
                    return _addOperationMap[numericType](valBefore, valAfter);
                }
            },
            {
                '-',
                (valBefore, valAfter, numericType) =>
                {
                    return _subOperationMap[numericType](valBefore, valAfter);
                }
            },
            {
                '*',
                (valBefore, valAfter, numericType) =>
                {
                    return _mulOperationMap[numericType](valBefore, valAfter);
                }
            },
            {
                '/',
                (valBefore, valAfter, numericType) =>
                {
                    return _divOperationMap[numericType](valBefore, valAfter);
                }
            }
        };
#endregion
        public static void ResetAllToDefault()
        {
            OnAllAboutToReset?.Invoke();

            foreach (GameConfig c in _instances)
            {
                c.ResetToDefault();
            }

            OnAllReseted?.Invoke();
        }

        public void ResetToDefault()
        {
            OnAboutToReset?.Invoke(this);

            foreach (BlendField field in _blendFields)
            {
                field.ResetToDefault();
            }

            OnConfigResetedToDefault();
            OnReseted?.Invoke(this);
        }

        protected virtual void OnConfigResetedToDefault()
        {

        }

        public void Blend(GameConfig blend)
        {
            foreach (BlendField field in blend.FieldsToBlend)
            {
                field.Init(blend);

                BlendField localTarget = null;
                foreach (BlendField f in _blendFields)
                {
                    if (f.PropertyId == field.PropertyId)
                    {
                        localTarget = f;
                        break;
                    }
                }

                if (localTarget == null) break;

                localTarget.Init(this);

                object valueBefore = localTarget.Getter();
                object valueAfter = field.Getter();

                if (field.NumericType != PropertyNumericType.Unknown)
                {
                    if (_mathOperationSelectionMap.TryGetValue(field.Operation, out Func<object, object, PropertyNumericType, object> mathOp))
                    {
                        valueAfter = mathOp(valueBefore, valueAfter, field.NumericType);
                    }
                }

                if (!valueBefore.Equals(valueAfter))
                {
                    localTarget.Setter(valueAfter);

                    InvokeOnBlend(field);
                }
            }
        }

        private void InvokeOnBlend(BlendField field)
        {
            if (field.CorrespondingInterfaces == null || !field.CorrespondingInterfaces.Any())
            {
                OnBlend?.Invoke(field.PropertyInfo.Name);
            }
            else
            {
                foreach (BlendablePropertyCorrespondingInterface b in field.CorrespondingInterfaces)
                {
                    OnBlend?.Invoke($"{b.CorrespondingInterfaceName}.{b.CorrespondingInterfacePropertyName}|{field.PropertyInfo.Name}");
                }
            }
        }

        public static bool IsFieldAffected(string affected, string fieldName)
        {
            int dotIndex = affected.IndexOf('.');

            if (dotIndex < 0)
            {
                return affected == fieldName;
            }

            int vBarIndex = affected.IndexOf('|');

            if (affected.Length - vBarIndex - 1 != fieldName.Length)
            {
                return CheckFirstFieldName();
            }

            bool isAffected = true;

            if (affected.Length - vBarIndex - 1 != fieldName.Length) return false;

            int fieldIndex = 0;
            for (int i = dotIndex + 1; i < vBarIndex; i++)
            {
                if (affected[i] != fieldName[fieldIndex])
                {
                    isAffected = false;
                    break;
                }

                fieldIndex++;
            }

            if (isAffected) return true;

            return CheckFirstFieldName();

            bool CheckFirstFieldName()
            {
                if (vBarIndex - dotIndex - 1 != fieldName.Length) return false;

                int fieldIndexLocal = 0;
                bool isAffectedLocal = true;
                for (int i = dotIndex + 1; i < vBarIndex; i++)
                {
                    if (affected[i] != fieldName[fieldIndexLocal])
                    {
                        isAffectedLocal = false;
                        break;
                    }

                    fieldIndexLocal++;
                }

                return isAffectedLocal;
            }
        }
        public static bool IsFieldAffected(string affected, string fieldName, string interfaceName)
        {
            int dotIndex = affected.IndexOf('.');

            if (dotIndex < 0)
            {
                return affected == fieldName;
            }

            if (dotIndex != interfaceName.Length) return false;

            bool isAffected = true;
            for(int i = 0; i < dotIndex; i++)
            {
                if (affected[i] != interfaceName[i])
                {
                    isAffected = false;
                    break;
                }
            }

            if (isAffected)
            {
                return IsFieldAffected(affected, fieldName);
            }

            return false;
        }
    }
}
