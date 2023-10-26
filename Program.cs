using System;

namespace BBCodeFormatter 
{
    class Program
    {
        public static void Main()
        {
            CodeConverter cc = new();
            cc.ReadXML("examplecode.xml");
        }
    }
}