﻿using System;
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
            var assemblyPath = @"C:\Users\SerG\Documents\Visual Studio 2010\Projects\MonoInjections\Test\bin\Debug\Test.exe";
            //Inject(assemblyPath);
            InjectToAssembly(assemblyPath);
        }

        static void InjectToAssembly(string path)
        {
            var assembly = AssemblyDefinition.ReadAssembly(path);

            _methodBaseRef          = assembly.MainModule.Import(typeof(MethodBase));
            _getCurrentMethodRef    = assembly.MainModule.Import(typeof(MethodBase).GetMethod("GetCurrentMethod"));
            _secureFieldBuilderRef  = assembly.MainModule.Import(typeof(SecureFieldBuilder));
            _secureFieldFactoryRef  = assembly.MainModule.Import(typeof(SecureFieldBuilder).GetMethod("Factory"));
            _getSecureFieldRef      = assembly.MainModule.Import(typeof(SecureFieldBuilder).GetMethod("GetSecureField"));
            _setSecureFieldRef      = assembly.MainModule.Import(typeof(SecureFieldBuilder).GetMethod("SetSecureField"));

            foreach (var typeDef in assembly.MainModule.Types)
            {
                foreach (var prop in typeDef.Properties.Where(m => m.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Name == "SecureFieldAttribute") != null))
                {
                    ReplaceGetMethod(prop);
                    ReplaceSetMethod(prop);
                }
            }
            assembly.Write(path);
        }

        private static void ReplaceSetMethod(PropertyDefinition prop)
        {
            // создаем две локальных переменных для attribute, currentMethod и parameters
            var currentMethodVar = new VariableDefinition(_methodBaseRef);
            var builderVar = new VariableDefinition(_secureFieldBuilderRef);

            var ilProc = prop.GetMethod.Body.GetILProcessor();
            // необходимо установить InitLocals в true, так как если он находился в false (в методе изначально не было локальных переменных)
            // а теперь локальные переменные появятся - верификатор IL кода выдаст ошибку.
            prop.SetMethod.Body.InitLocals = true;
            // создаем три локальных переменных для attribute, currentMethod и parameters
            ilProc.Body.Variables.Add(currentMethodVar);
            ilProc.Body.Variables.Add(builderVar);
            ilProc.Body.Instructions.Clear();

            ilProc.Append(Instruction.Create(OpCodes.Ret));
            var firstInstruction = ilProc.Body.Instructions[0];
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
            // получаем текущий метод
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, _getCurrentMethodRef));
            // помещаем результат со стека в переменную currentMethodVar
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, currentMethodVar));
            // Вызываем SecureFieldFactory
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, _secureFieldFactoryRef));
            // помещаем результат со стека в переменную builderVar
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, builderVar));

            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, builderVar));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, currentMethodVar));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldarg_0));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldarg_1));

            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Callvirt, _setSecureFieldRef));
        }

        private static void ReplaceGetMethod(PropertyDefinition prop)
        {
            // создаем две локальных переменных для attribute, currentMethod и parameters
            var currentMethodVar = new VariableDefinition(_methodBaseRef);
            var builderVar = new VariableDefinition(_secureFieldBuilderRef);

            var ilProc = prop.GetMethod.Body.GetILProcessor();
            // необходимо установить InitLocals в true, так как если он находился в false (в методе изначально не было локальных переменных)
            // а теперь локальные переменные появятся - верификатор IL кода выдаст ошибку.
            prop.GetMethod.Body.InitLocals = true;

            ilProc.Body.Variables.Add(currentMethodVar);
            ilProc.Body.Variables.Add(builderVar);
            ilProc.Body.Instructions.Clear();

            ilProc.Append(Instruction.Create(OpCodes.Ret));
            var firstInstruction = ilProc.Body.Instructions[0];
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
            // получаем текущий метод
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, _getCurrentMethodRef));
            // помещаем результат со стека в переменную currentMethodVar
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, currentMethodVar));
            // Вызываем SecureFieldFactory
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, _secureFieldFactoryRef));
            // помещаем результат со стека в переменную builderVar
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, builderVar));

            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, builderVar));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, currentMethodVar));
            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldarg_0));

            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Callvirt, _getSecureFieldRef));
        }

        static void ProtectMethod(string path, string methodName)
        {
            var assembly = AssemblyDefinition.ReadAssembly(path);
            foreach (var typeDef in assembly.MainModule.Types)
            {
                foreach (var method in typeDef.Methods)
                {
                    if (method.Name == methodName)
                    {
                        var ilProc = method.Body.GetILProcessor();
                        // здесь получаем internal конструктор для класса OpCode
                        var constructor = typeof(OpCode).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(int), typeof(int) }, null);
                        // в Mono.Cecil инструкции создаются оригинальным способом - в конструктор передается два 4х-битных(int) числа, из которых операциями побитового сдвига получается 8 байт, а каждый байт отвечает за определенный параметр MSIL иструкции. Соответственно, такими же операциями побитового сдвига мы превращаем 8 байт в 2 числа. Каждый байт отвечает за определенную характеристику OpCode, но нам важны только первые два. Остальные тоже имеют некоторое значение для нашей задачи, так как если задать их абы как, Mono.Cecil может не допустить такую инструкцию и выкинет Exception, но я не буду останавливаться на подробностях.
                        int x =
                          0xff << 0 | //это первый байт IL инстуркции
                          0x24 << 8 | //это второй байт IL инструкции
                          0x00 << 16 |
                          (byte)FlowControl.Next << 24;
                        // дальнейшее не имеет отношения к нашей цели, однако необходимо для того, чтобы Mono.Cecil корректно обработал нашу инстукцию
                        int y = (byte)OpCodeType.Primitive << 0 |
                                (byte)OperandType.InlineNone << 8 |
                                (byte)StackBehaviour.Pop0 << 16 |
                                (byte)StackBehaviour.Push0 << 24;

                        var badOpCode = (OpCode)constructor.Invoke(new object[] { x, y });
                        // создаем плохую инструкцию
                        Instruction badInstruction = Instruction.Create(badOpCode);
                        Instruction firstInstruction = ilProc.Body.Instructions[0];
                        // вставляем инструкцию безусловного перехода на реальный код метода
                        ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Br_S, firstInstruction));
                        // вставляем плохую инструкцию перед началом кода метода
                        ilProc.InsertBefore(firstInstruction, badInstruction);
                    }
                }
            }
            assembly.Write(path);
        }
    }
}
