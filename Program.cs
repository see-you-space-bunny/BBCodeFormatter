using System;

namespace BBCodeFormatter 
{
    class Program
    {
        public static void Main()
        {
            CodeConverter cc = new();
            Console.WriteLine(cc.ReadAndConvert("examplecode.xml").Result);
        }
    }
}