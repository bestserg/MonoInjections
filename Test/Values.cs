﻿using System;
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
        private int _number;

        [SecureField]
        public int Number { 
            get { return _number; } 
            set { _number = value; } 
        }


        //{
        //    get
        //    {
        //        var currentMethod = MethodBase.GetCurrentMethod();
        //        var sf = SecureFieldBuilder.Factory();
        //        return sf.GetSecureField(currentMethod, this);
        //    }
        //    set
        //    {
        //        var currentMethod = MethodBase.GetCurrentMethod();
        //        var sf = SecureFieldBuilder.Factory();
        //        sf.SetSecureField(currentMethod, this, value);
        //    }
        //}

        //var currentMethod = MethodBase.GetCurrentMethod();
        //var propName = currentMethod.Name.Substring(4);
        //var prop = currentMethod.ReflectedType.GetProperty(propName);
        //var attribute = (SecureFieldAttribute)Attribute.GetCustomAttribute(prop, typeof(SecureFieldAttribute));

    }
}