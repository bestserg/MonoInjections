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
    }
}
