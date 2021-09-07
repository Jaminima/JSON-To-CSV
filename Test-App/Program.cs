using System;
using Newtonsoft.Json.Linq;

namespace Test_App
{
    class Program
    {
        class SomeClass
        {
            public string name = "Example", key = "value";
        }

        static void Main(string[] args)
        {
            SomeClass[] someClasses = new SomeClass[5];
            for (int i = 0; i < someClasses.Length; i++) someClasses[i] = new SomeClass();

            JArray jArray = JArray.FromObject(someClasses);
            JSON_To_CSV.Converter.WriteConversion("./out.csv", jArray);
        }
    }
}
