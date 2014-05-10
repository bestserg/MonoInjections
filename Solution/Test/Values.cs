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
        public float Number 
        //{ 
        //    get { return _number; } 
        //    set { _number = value; } 
        //}
        {
            get
            {
                var currentMethod = MethodBase.GetCurrentMethod();
                var sf = SecureFieldBuilder.Factory();
                return sf.GetSecureField(currentMethod, this);
            }
            set
            {
                var currentMethod = MethodBase.GetCurrentMethod();
                var sf = SecureFieldBuilder.Factory();
                sf.SetSecureField(currentMethod, this, value);
            }
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