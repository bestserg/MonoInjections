using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using SecureField;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var val = new Values();
            //var metdos = typeof(Values).GetProperty("Number").GetSetMethod();
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
