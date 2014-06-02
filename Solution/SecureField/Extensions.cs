using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SecureField
{
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
}
