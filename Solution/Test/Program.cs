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
            SpeedTest();

            //var val = new Values();
            
            //while (true) 
            //{
            //    Console.Write("Enter: ");
            //    var ch = Console.ReadKey().KeyChar;
            //    switch (ch)
            //    { 
            //        case 'p':
            //            Console.WriteLine();
            //            Console.WriteLine(val.ValuesLog());
            //            Console.ReadLine();
            //            break;
            //        case 'm':
            //            Console.WriteLine();
            //            Console.WriteLine(val.MemoryLog());
            //            Console.ReadLine();
            //            break;
            //        case 'L':
            //            Console.WriteLine();
            //            Console.WriteLine(val.ValuesLog());
            //            Console.WriteLine(val.MemoryLog());
            //            Console.ReadLine();
            //            break;
            //        case 'c':
            //            val.CharVal = char.Parse(Console.ReadLine());
            //            break;
            //        case 'd':
            //            val.DoubleVal = double.Parse(Console.ReadLine());
            //            break;
            //        case 'i':
            //            val.Int32Val = int.Parse(Console.ReadLine());
            //            break;
            //        case 'u':
            //            val.UInt16Val = ushort.Parse(Console.ReadLine());
            //            break;
            //        case 'q':
            //            return;
            //    }
               
            //    Console.Clear();
            //}
        }

        private static void SpeedTest()
        {
            double read = 0, readP = 0, write = 0, writeP = 0;
            int temp;
            var rand = new Random();
            var val = new Values();
            val.Value = rand.Next();
            val.ProtectedValue = val.Value;
            var startTime = DateTime.Now;
            int count = 0;

            var secure = SecureFieldBuilder.Factory();
            for (int i = 0; i < 100; i++)
            {
                secure.AddHash();
            }

            while ((DateTime.Now - startTime).TotalMinutes < 3)
            {
                var time = DateTime.Now;
                count++;
                for (int i = 0; i < 1000000; i++)
                {
                    temp = val.Value;
                }
                read += (DateTime.Now - time).TotalMilliseconds;
                time = DateTime.Now;
                for (int i = 0; i < 1000000; i++)
                {
                    temp = val.ProtectedValue;
                }
                readP += (DateTime.Now - time).TotalMilliseconds;
                time = DateTime.Now;
                for (int i = 0; i < 1000000; i++)
                {
                    val.Value = rand.Next();
                }
                write += (DateTime.Now - time).TotalMilliseconds;
                time = DateTime.Now;
                for (int i = 0; i < 1000000; i++)
                {
                    val.ProtectedValue = rand.Next();
                }
                writeP += (DateTime.Now - time).TotalMilliseconds;
            }
            Console.WriteLine("Read:  {0} | {1}", read/count, readP/count);
            Console.WriteLine("Write: {0} | {1}", write / count, writeP / count);
            Console.WriteLine(count);
            Console.ReadKey();
        }
    }
}
