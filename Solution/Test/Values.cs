using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SecureField;
using System.Reflection;

namespace Test
{
    class Values
    {

        //private float _numberWithField;

        //[SecureField]
        //public float NumberWithField
        //{ get { return _numberWithField; } set { _numberWithField = value; } }

        [SecureField]
        public int Number { get; set; }

        //{
        //    get
        //    {
        //        var currentMethod = MethodBase.GetCurrentMethod();
        //        var builder = SecureFieldBuilder.Factory();
        //        return (int)builder.GetSecureField(currentMethod, this);
        //    }
        //    set
        //    {
        //        var currentMethod = MethodBase.GetCurrentMethod();
        //        var builder = SecureFieldBuilder.Factory();
        //        builder.SetSecureField(currentMethod, this, (object)value);
        //    }
        //}
        

        //var currentMethod = MethodBase.GetCurrentMethod();
        //var propName = currentMethod.Name.Substring(4);
        //var prop = currentMethod.ReflectedType.GetProperty(propName);
        //var attribute = (SecureFieldAttribute)Attribute.GetCustomAttribute(prop, typeof(SecureFieldAttribute));

    }
}
