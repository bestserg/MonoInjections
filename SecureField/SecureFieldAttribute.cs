using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SecureField
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SecureFieldAttribute : Attribute
    {
        public SecureFieldAttribute()
        {

        }

        //public int MethodGet(MethodBase currentMethod, int val)
        //{
        //    var methodName = String.Format("{0}.{1}", currentMethod.ReflectedType.Name, currentMethod.Name.Substring(4));
        //    var sf = SecureFieldBuilder.Factory();
        //    return sf.GetSecureField(methodName, val);
        //}
        //public int MethodSet(MethodBase currentMethod, int val)
        //{
        //    var methodName = String.Format("{0}.{1}", currentMethod.ReflectedType.Name, currentMethod.Name.Substring(4));
        //    var sf = SecureFieldBuilder.Factory();
        //    return sf.SetSecureField(methodName, val);
        //}

    }
}
