using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Collections;
using System.Reflection;

namespace SecureField
{
    public class SecureFieldBuilder
    {
        private static readonly SecureFieldBuilder _instance = new SecureFieldBuilder();
        private static Hashtable _hash = new Hashtable();
        private static Random _rand = new Random();

        protected SecureFieldBuilder()
        {
        }

        static public SecureFieldBuilder Factory()
        {
            return _instance;
        }

        //public int GetSecureField(MethodBase method, object obj)
        //{
        //    var numField = method.ReflectedType.GetField(method.Name.Substring(3).ToLower(), BindingFlags.NonPublic | BindingFlags.Instance);
        //    var methodName = method.GetPropertyName();
        //    var mask = _hash[methodName];
        //    if (mask == null)
        //        return 0;
        //    var type = numField.FieldType;
        //    MethodInfo castMethod = this.GetType().GetMethod("Cast", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type);
        //    var castedObject = castMethod.Invoke(null, new object[] { mask });
            
        //    var val = (int)numField.GetValue(obj);
        //    int maskInt = (int)mask;
        //    return val - maskInt;
        //}
        public dynamic GetSecureField(MethodBase method, object obj)
        {
            var numField = method.ReflectedType.GetField(method.Name.Substring(3).ToLower(), BindingFlags.NonPublic | BindingFlags.Instance);
            var methodName = method.GetPropertyName();
            var mask = _hash[methodName];
            var val = numField.GetValue(obj);
            if (mask == null)
                return val;

            var byteVal = new BitValue(val);
            byteVal.Xor((byte[])mask);
            return byteVal.ToValue();
        }

        //public void SetSecureField(MethodBase method, object obj, int value)
        //{
        //    var numField = method.ReflectedType.GetField(method.Name.Substring(3).ToLower(), BindingFlags.NonPublic | BindingFlags.Instance);

        //    var methodName = method.GetPropertyName();
        //    int mask = 10;// _rand.Next(1000);
        //    if (_hash.ContainsKey(methodName))
        //        _hash[methodName] = mask;
        //    else
        //    { 
        //        _hash.Add(methodName, mask);
        //    }

        //    //var mask = _rand.Next(1000);
        //    numField.SetValue(obj, value + mask);
        //}
        public void SetSecureField(MethodBase method, object obj, object value)
        {
            var numField = method.ReflectedType.GetField(method.Name.Substring(3).ToLower(), BindingFlags.NonPublic | BindingFlags.Instance);
            var methodName = method.GetPropertyName();
            var mask = BitConverter.GetBytes(666666666);// _rand.Next(1000);
            if (_hash.ContainsKey(methodName))
                _hash[methodName] = mask;
            else
            {
                _hash.Add(methodName, mask);
            }

            var bytes = new BitValue(value);
            bytes.Xor(mask);
            numField.SetValue(obj, bytes.ToValue());
        }
    }

    static class MethodBaseExt 
    { 
        static public string GetPropertyName(this MethodBase m)
        {
            return String.Format("{0}.{1}", m.ReflectedType.Name, m.Name.Substring(4));
        }
    }
    public static class ByteArrayExt 
    {
        public static byte[] Xor(this byte[] buffer1, byte[] buffer2)
        {
            for (int i = 0; i < buffer2.Length; i++)
                buffer1[i] ^= buffer2[i];
            return buffer1;
        }
    }

    public struct BitValue
    {
        public byte[] Bytes;
        enum SerializationType { Char, Int32, UInt32, Double, Single, None };
        private SerializationType _serializationType;

        public BitValue(object obj)
        {
            _serializationType = SerializationType.None;
            var type = obj.GetType();
            if (type.IsValueType)
            {
                dynamic d = obj;
                Bytes = BitConverter.GetBytes(d);
                switch (type.Name)
                {
                    case "Char":
                        _serializationType = SerializationType.Char; break;
                    case "Int32":
                        _serializationType = SerializationType.Int32; break;
                    case "Double":
                        _serializationType = SerializationType.Double; break;
                    case "Single":
                        _serializationType = SerializationType.Single; break;
                }
            }
            else {
                throw new ArgumentException("Ожидается значимый тип");
            }
        }

        public dynamic ToValue()
        {
            switch (_serializationType)
            {
                case SerializationType.Char:
                    return BitConverter.ToChar(Bytes, 0);
                case SerializationType.Int32:
                    return BitConverter.ToInt32(Bytes, 0);
                case SerializationType.Double:
                    return BitConverter.ToDouble(Bytes, 0);
                case SerializationType.Single:
                    return BitConverter.ToSingle(Bytes, 0);
            }
            return null;
        }

        public void Xor (byte[] bytes)
        {
            Bytes = Bytes.Xor(bytes);
        }
    }
}
