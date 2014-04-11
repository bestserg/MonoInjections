using System;
using System.Collections.Generic;
using System.Linq;
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

        public int GetSecureField(MethodBase method, object obj)
        {
            var numField = method.ReflectedType.GetField(method.Name.Substring(3).ToLower(), BindingFlags.NonPublic | BindingFlags.Instance);
            var val = (int)numField.GetValue(obj);
            var methodName = method.GetPropertyName();
            if (val == 0 || !_hash.ContainsKey(methodName))
                return 0;
            int mask = (int)_hash[methodName];
            return val - mask;
        }

        public void SetSecureField(MethodBase method, object obj, int value)
        {
            var numField = method.ReflectedType.GetField(method.Name.Substring(3).ToLower(), BindingFlags.NonPublic | BindingFlags.Instance);

            var methodName = method.GetPropertyName();
            int mask = 10;// _rand.Next(1000);
            if (_hash.ContainsKey(methodName))
                _hash[methodName] = mask;
            else
            { 
                _hash.Add(methodName, mask);
            }

            //var mask = _rand.Next(1000);
            numField.SetValue(obj, value + mask);
        }

        

    }

    static class MethodBaseExt { 
    
        static public string GetPropertyName(this MethodBase m)
        {
            return String.Format("{0}.{1}", m.ReflectedType.Name, m.Name.Substring(4));
        }
    }
}
