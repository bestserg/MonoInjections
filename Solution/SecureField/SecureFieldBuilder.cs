﻿using System;
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

        public object GetSecureField(MethodBase method, object obj)
        {
            var numField = method.ReflectedType.GetField(method.GetFieldName(), BindingFlags.NonPublic | BindingFlags.Instance);
            var fullPropName = method.GetFullPropertyName();
            var mask = (byte[])_hash[fullPropName];
            var val = numField.GetValue(obj);
            if (mask == null)
                return val;

            var byteVal = new BitValue(val);
            byteVal.Xor(mask);
            return byteVal.ToValue();
        }

        public void SetSecureField(MethodBase method, object obj, object value)
        {
            var numField = method.DeclaringType.GetField(method.GetFieldName(), BindingFlags.NonPublic | BindingFlags.Instance);
            var fullPropName = method.GetFullPropertyName();

            var bytes = new BitValue(value);
            var mask = new byte[bytes.Bytes.Length];
            _rand.NextBytes(mask);
            bytes.Xor(mask);

            if (_hash.ContainsKey(fullPropName))
                _hash[fullPropName] = mask;
            else
            {
                _hash.Add(fullPropName, mask);
            }
            numField.SetValue(obj, bytes.ToValue());
        }
    }

    static class MethodBaseExt 
    {
        static public string GetFieldName(this MethodBase m)
        {
            return String.Format("_{0}{1}", char.ToLower(m.Name[4]), m.Name.Substring(5)); 
        }
        static public string GetFullPropertyName(this MethodBase m)
        {
            return String.Format("{0}.{1}", m.DeclaringType.Name, m.Name.Substring(4));
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
        enum SerializationType { Char, Int16, UInt16, Int32, UInt32, Int64, UInt64, Double, Single, None };
        private readonly SerializationType _serializationType;

        public BitValue(object obj)
        {
            _serializationType = SerializationType.None;
            Bytes = null;

            var type = obj.GetType();
            if (!type.IsValueType)
                throw new ArgumentException("Ожидается значимый тип");
            
            switch (type.Name)
            {
                case "Char":
                    _serializationType = SerializationType.Char; break;
                case "Int16":
                    _serializationType = SerializationType.Int16; break;
                case "UInt16":
                    _serializationType = SerializationType.UInt16; break;
                case "Int32":
                    _serializationType = SerializationType.Int32; break;
                case "UInt32":
                    _serializationType = SerializationType.UInt32; break;
                case "Int64":
                    _serializationType = SerializationType.Int64; break;
                case "UInt64":
                    _serializationType = SerializationType.UInt64; break;
                case "Double":
                    _serializationType = SerializationType.Double; break;
                case "Single":
                    _serializationType = SerializationType.Single; break;
            }

            GetBytes(obj);
        }
        private void GetBytes(object obj)
        {
            switch (_serializationType)
            {
                case SerializationType.Char:
                    Bytes = BitConverter.GetBytes((Char)obj); return;
                case SerializationType.Int16:
                    Bytes = BitConverter.GetBytes((Int16)obj); return;
                case SerializationType.UInt16:
                    Bytes = BitConverter.GetBytes((UInt16)obj); return;
                case SerializationType.Int32:
                    Bytes = BitConverter.GetBytes((Int32)obj); return;
                case SerializationType.UInt32:
                    Bytes = BitConverter.GetBytes((UInt32)obj); return;
                case SerializationType.Int64:
                    Bytes = BitConverter.GetBytes((Int64)obj); return;
                case SerializationType.UInt64:
                    Bytes = BitConverter.GetBytes((UInt64)obj); return;
                case SerializationType.Double:
                    Bytes = BitConverter.GetBytes((Double)obj); return;
                case SerializationType.Single:
                    Bytes = BitConverter.GetBytes((Single)obj); return;
            }
            Bytes = null;
        }
        public object ToValue()
        {
            switch (_serializationType)
            {
                case SerializationType.Char:
                    return BitConverter.ToChar(Bytes, 0);
                case SerializationType.Int16:
                    return BitConverter.ToInt16(Bytes, 0);
                case SerializationType.UInt16:
                    return BitConverter.ToUInt16(Bytes, 0);
                case SerializationType.Int32:
                    return BitConverter.ToInt32(Bytes, 0);
                case SerializationType.UInt32:
                    return BitConverter.ToUInt32(Bytes, 0);
                case SerializationType.Int64:
                    return BitConverter.ToInt64(Bytes, 0);
                case SerializationType.UInt64:
                    return BitConverter.ToUInt64(Bytes, 0);
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
