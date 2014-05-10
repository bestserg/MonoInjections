using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Collections;
using SecureField;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var bitv = new BitValue(new object());
            //var mask = new byte[] {255};
            //bitv.Bytes = bitv.Bytes.Xor(mask);
            //var newVal = bitv.ToValue();
            //bitv = new BitValue(newVal);
            //bitv.Bytes = bitv.Bytes.Xor(mask);
            //var oldVal = bitv.ToValue();


            var val = new Values();
            while (true) 
            {
                Console.Write("Enter: ");
                var ch = Console.ReadKey().KeyChar;
                switch (ch)
                { 
                    case 'p':
                        Console.WriteLine();
                        Console.WriteLine(val.ValuesLog());
                        Console.ReadLine();
                        break;
                    case 'm':
                        Console.WriteLine();
                        Console.WriteLine(val.MemoryLog());
                        Console.ReadLine();
                        break;
                    case 'L':
                        Console.WriteLine();
                        Console.WriteLine(val.ValuesLog());
                        Console.WriteLine(val.MemoryLog());
                        Console.ReadLine();
                        break;
                    case 'c':
                        val.CharVal = char.Parse(Console.ReadLine());
                        break;
                    case 'd':
                        val.DoubleVal = double.Parse(Console.ReadLine());
                        break;
                    case 'i':
                        val.Int32Val = int.Parse(Console.ReadLine());
                        break;
                    case 'u':
                        val.UInt16Val = ushort.Parse(Console.ReadLine());
                        break;
                    case 'q':
                        return;
                }
               
                Console.Clear();
            }
        }
    }
}
