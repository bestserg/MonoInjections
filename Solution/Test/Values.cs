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
        [SecureField]
        public char CharVal { get; set; }

        [SecureField]
        public int Int32Val { get; set; }

        [SecureField]
        public ushort UInt16Val { get; set; }

        [SecureField]
        public double DoubleVal { get; set; }

        //[SecureField]
        //public int Number { get; set; }
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

        public string ValuesLog()
        { 
            var type = MethodBase.GetCurrentMethod().DeclaringType;
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var log = "Values log:\n";
            foreach (var prop in props)
            {
                log += string.Format("{0}\t: {1}\n",prop.PropertyType.Name, prop.GetValue(this, null));
            }
            return log;
        }
        public string MemoryLog()
        {
            var type = MethodBase.GetCurrentMethod().DeclaringType;
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.Name.StartsWith("_"));
            var log = "Memory log:\n";
            foreach (var field in fields)
            {
                log += string.Format("{0}\t: {1}\n", field.FieldType.Name, field.GetValue(this));
            }
            return log;
        }
    }
}