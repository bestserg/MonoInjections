using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;
using SecureField;

namespace MonoInjections
{
    class Program
    {
        private const string RepPath = @"C:\GitRepository\MonoInjections\Solution\{0}\bin\Debug\{0}.exe";
        private static TypeReference _secureFieldBuilderRef;
        private static MethodReference _secureFieldFactoryRef;
        private static MethodReference _getSecureFieldRef;
        private static MethodReference _setSecureFieldRef;
        private static TypeReference _methodBaseRef;
        private static MethodReference _getCurrentMethodRef;

        static void Main(string[] args)
        {
            //if (args.Length == 0)
            //    return;
            //string assemblyPath = args[0];
            var assemblyPath = string.Format(RepPath, "Test");

            var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
            InitReferences(assembly);
            assembly = InjectToAssembly(assembly);
            assembly.Write(assemblyPath);
        }

        private static void InitReferences(AssemblyDefinition assembly)
        {
            _methodBaseRef = assembly.MainModule.Import(typeof(MethodBase));
            _getCurrentMethodRef = assembly.MainModule.Import(typeof(MethodBase).GetMethod("GetCurrentMethod"));
            _secureFieldBuilderRef = assembly.MainModule.Import(typeof(SecureFieldBuilder));
            _secureFieldFactoryRef = assembly.MainModule.Import(typeof(SecureFieldBuilder).GetMethod("Factory"));
            _getSecureFieldRef = assembly.MainModule.Import(typeof(SecureFieldBuilder).GetMethod("GetSecureField"));
            _setSecureFieldRef = assembly.MainModule.Import(typeof(SecureFieldBuilder).GetMethod("SetSecureField"));
        }

        private static AssemblyDefinition InjectToAssembly(AssemblyDefinition assembly)
        {
            foreach (var typeDef in assembly.MainModule.Types)
            {
                var properties = typeDef.Properties 
                                        .Where(p => p.CustomAttributes.Any(attr => attr.AttributeType.Name == "SecureFieldAttribute")
                                               && !p.Name.StartsWith("_"));
                foreach (var prop in properties)
                {
                    var fieldName = String.Format("_{0}{1}", char.ToLower(prop.Name[0]), prop.Name.Substring(1));
                    if(typeDef.Fields.All(f => f.Name != fieldName))
                    {
                        var field = new FieldDefinition(fieldName, Mono.Cecil.FieldAttributes.Private, prop.PropertyType);
                        typeDef.Fields.Add(field);
                    }
                    ReplaceGetMethod(prop);
                    ReplaceSetMethod(prop);
                }
            }
            return assembly;
        }

        /* 
         * MethodBase currentMethod = MethodBase.GetCurrentMethod();
         * SecureFieldBuilder.Factory().SetSecureField(currentMethod, this, value);
         */
        private static void ReplaceSetMethod(PropertyDefinition prop)
        {
            var method = prop.SetMethod;
            var ilProc = method.Body.GetILProcessor();
            // необходимо установить InitLocals в true, так как если он находился в false (в методе изначально не было локальных переменных)
            // а теперь локальные переменные появятся - верификатор IL кода выдаст ошибку.
            ilProc.Body.Instructions.Clear();
            ilProc.Body.InitLocals = true;

            // создаем две локальных переменных для MethodBase и SecureFieldBuilder
            var currentMethodVar = new VariableDefinition(_methodBaseRef);
            var builderVar = new VariableDefinition(_secureFieldBuilderRef);
            ilProc.Body.Variables.Add(currentMethodVar);
            ilProc.Body.Variables.Add(builderVar);

            //Добавляем инструкцию Return
            ilProc.Append(Instruction.Create(OpCodes.Ret));
            var firstInstruction = ilProc.Body.Instructions[0];

            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
            // получаем текущий метод MethodBase.GetCurrentMethod();
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, _getCurrentMethodRef));
            // помещаем результат со стека в переменную currentMethod
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, currentMethodVar));
            // Вызываем SecureFieldBuilder.Factory()
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, _secureFieldFactoryRef));
            // помещаем результат со стека в переменную builder
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, builderVar));

            // Вызываем builder.SetSecureField(currentMethod, this, value);
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, builderVar));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, currentMethodVar));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldarg_0));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldarg_1));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Box, prop.PropertyType));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Callvirt, _setSecureFieldRef));
        }

        private static void ReplaceGetMethod(PropertyDefinition prop)
        {
            var method = prop.GetMethod;
            var ilProc = method.Body.GetILProcessor();
            // необходимо установить InitLocals в true, так как если он находился в false (в методе изначально не было локальных переменных)
            // а теперь локальные переменные появятся - верификатор IL кода выдаст ошибку.
            //prop.GetMethod.Body.InitLocals = true;
            ilProc.Body.Instructions.Clear();
            ilProc.Body.InitLocals = true;

            // создаем две локальных переменных для MethodBase и SecureFieldBuilder
            var currentMethodVar = new VariableDefinition(_methodBaseRef);
            var builderVar = new VariableDefinition(_secureFieldBuilderRef);
            ilProc.Body.Variables.Add(currentMethodVar);
            ilProc.Body.Variables.Add(builderVar);

            //Добавляем инструкцию Return
            
            ilProc.Append(Instruction.Create(OpCodes.Ret));
            var firstInstruction = ilProc.Body.Instructions[0];

            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
            // получаем текущий метод MethodBase.GetCurrentMethod();
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, _getCurrentMethodRef));
            // помещаем результат со стека в переменную currentMethod
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, currentMethodVar));
            // Вызываем SecureFieldBuilder.Factory()
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, _secureFieldFactoryRef));
            // помещаем результат со стека в переменную builder
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, builderVar));

            // Вызываем builder.GetSecureField(currentMethod, this);
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, builderVar));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, currentMethodVar));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldarg_0));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Callvirt, _getSecureFieldRef));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Unbox_Any, prop.PropertyType));
        }

        //static void ProtectMethod(string path, string methodName)
        //{
        //    var assembly = AssemblyDefinition.ReadAssembly(path);
        //    foreach (var typeDef in assembly.MainModule.Types)
        //    {
        //        foreach (var method in typeDef.Methods)
        //        {
        //            if (method.Name == methodName)
        //            {
        //                var ilProc = method.Body.GetILProcessor();
        //                // здесь получаем internal конструктор для класса OpCode
        //                var constructor = typeof(OpCode).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(int), typeof(int) }, null);
        //                // в Mono.Cecil инструкции создаются оригинальным способом - в конструктор передается два 4х-битных(int) числа, из которых операциями побитового сдвига получается 8 байт, а каждый байт отвечает за определенный параметр MSIL иструкции. Соответственно, такими же операциями побитового сдвига мы превращаем 8 байт в 2 числа. Каждый байт отвечает за определенную характеристику OpCode, но нам важны только первые два. Остальные тоже имеют некоторое значение для нашей задачи, так как если задать их абы как, Mono.Cecil может не допустить такую инструкцию и выкинет Exception, но я не буду останавливаться на подробностях.
        //                int x =
        //                  0xff << 0 | //это первый байт IL инстуркции
        //                  0x24 << 8 | //это второй байт IL инструкции
        //                  0x00 << 16 |
        //                  (byte)FlowControl.Next << 24;
        //                // дальнейшее не имеет отношения к нашей цели, однако необходимо для того, чтобы Mono.Cecil корректно обработал нашу инстукцию
        //                int y = (byte)OpCodeType.Primitive << 0 |
        //                        (byte)OperandType.InlineNone << 8 |
        //                        (byte)StackBehaviour.Pop0 << 16 |
        //                        (byte)StackBehaviour.Push0 << 24;

        //                var badOpCode = (OpCode)constructor.Invoke(new object[] { x, y });
        //                // создаем плохую инструкцию
        //                Instruction badInstruction = Instruction.Create(badOpCode);
        //                Instruction firstInstruction = ilProc.Body.Instructions[0];
        //                // вставляем инструкцию безусловного перехода на реальный код метода
        //                ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Br_S, firstInstruction));
        //                // вставляем плохую инструкцию перед началом кода метода
        //                ilProc.InsertBefore(firstInstruction, badInstruction);
        //            }
        //        }
        //    }
        //    assembly.Write(path);
        //}
    }
}
