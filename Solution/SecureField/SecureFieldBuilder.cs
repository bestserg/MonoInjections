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
        private static readonly SecureFieldBuilder Instance = new SecureFieldBuilder();
        private static readonly Hashtable Hash = new Hashtable();
        private static readonly Random Rand = new Random();

        protected SecureFieldBuilder()
        {
        }

        public void AddHash() 
        { 
            var rand = Rand.Next();
            Hash.Add(rand.ToString(), rand);
        }
        static public SecureFieldBuilder Factory()
        {
            return Instance;
        }

        public object GetSecureField(MethodBase method, object obj)
        {
            var numField = method.DeclaringType.GetField(method.GetFieldName(), BindingFlags.NonPublic | BindingFlags.Instance);
            var fullPropName = method.GetFullPropertyName();

            var val = numField.GetValue(obj);
            var mask = (byte[])Hash[fullPropName];
            if (mask == null)
                return val;

            var byteVal = new BitValue(val);
            byteVal.Xor(mask);
            return byteVal.ToValue();
        }

        public void SetSecureField(MethodBase method, object obj, object val)
        {
            var numField = method.DeclaringType.GetField(method.GetFieldName(), BindingFlags.NonPublic | BindingFlags.Instance);
            var fullPropName = method.GetFullPropertyName();

            var byteVal = new BitValue(val);
            var mask = new byte[byteVal.Bytes.Length];
            Rand.NextBytes(mask);

            if (Hash.ContainsKey(fullPropName))
            {
                Hash[fullPropName] = mask;
            }
            else
            {
                Hash.Add(fullPropName, mask);
            }

            byteVal.Xor(mask);
            numField.SetValue(obj, byteVal.ToValue());
        }
    }
}
