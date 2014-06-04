using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecureField
{
    enum SerializationType { Char, Int16, UInt16, Int32, UInt32, Int64, UInt64, Double, Single, None };

    public struct BitValue
    {
        public byte[] Bytes;

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

        public void Xor(byte[] bytes)
        {
            Bytes = Bytes.Xor(bytes);
        }
    }
}
