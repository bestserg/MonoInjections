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
        private int _number;

        [SecureField]
        public int Number
        {
            //get { return _number; }
            //set { _number = value; }
            get
            {
                var currentMethod = MethodBase.GetCurrentMethod();
                var builder = SecureFieldBuilder.Factory();
                return builder.GetSecureField(currentMethod, this);
            }
            set
            {
                var currentMethod = MethodBase.GetCurrentMethod();
                var builder = SecureFieldBuilder.Factory();
                builder.SetSecureField(currentMethod, this, value);
            }
        }
    }
}

/* SetMethod original
set
{
    MethodBase currentMethod = MethodBase.GetCurrentMethod();
    SecureFieldBuilder.Factory().SetSecureField(currentMethod, this, value);
}
 * IL:
.method public hidebysig specialname instance void set_Number(int32 'value') cil managed
{
    .maxstack 4
    .locals init (
        [0] class [mscorlib]System.Reflection.MethodBase currentMethod)
    L_0000: nop 
    L_0001: call class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetCurrentMethod()
    L_0006: stloc.0 
    L_0007: call class [SecureField]SecureField.SecureFieldBuilder [SecureField]SecureField.SecureFieldBuilder::Factory()
    L_000c: ldloc.0 
    L_000d: ldarg.0 
    L_000e: ldarg.1 
    L_000f: box int32
    L_0014: callvirt instance void [SecureField]SecureField.SecureFieldBuilder::SetSecureField(class [mscorlib]System.Reflection.MethodBase, object, object)
    L_0019: nop 
    L_001a: ret 
}

 

 * GetMethod original
get
{
    MethodBase currentMethod = MethodBase.GetCurrentMethod();
    return SecureFieldBuilder.Factory().GetSecureField(currentMethod, this);
}
 * IL:
.method public hidebysig specialname instance int32 get_Number() cil managed
{
    .maxstack 3
    .locals init (
        [0] int32 num,
        [1] class [mscorlib]System.Reflection.MethodBase base2,
        [2] class [SecureField]SecureField.SecureFieldBuilder builder)
    L_0000: nop 
    L_0001: call class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetCurrentMethod()
    L_0006: stloc num
    L_000a: call class [SecureField]SecureField.SecureFieldBuilder [SecureField]SecureField.SecureFieldBuilder::Factory()
    L_000f: stloc base2
    L_0013: ldloc base2
    L_0017: ldloc num
    L_001b: ldarg.0 
    L_001c: callvirt instance int32 [SecureField]SecureField.SecureFieldBuilder::GetSecureField(class [mscorlib]System.Reflection.MethodBase, object)
    L_0021: ret 
}
 */