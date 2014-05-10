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
                var str = Console.ReadLine();
                if (str == "p")
                {
                    Console.WriteLine("value: {0}", val.Number);
                    Console.ReadLine();
                }
                else
                {
                    var n = int.Parse(str);
                    val.Number = n;
                }
                
                Console.Clear();
            }
        }


    }
}
