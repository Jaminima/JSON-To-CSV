using Newtonsoft.Json.Linq;

namespace Test_App
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            SomeClass[] someClasses = new SomeClass[5];
            for (int i = 0; i < someClasses.Length; i++) someClasses[i] = new SomeClass();

            JArray jArray = JArray.FromObject(someClasses);
            JSON_To_CSV.Converter.WriteConversion("./out.csv", jArray);
        }

        #endregion Methods

        #region Classes

        private class SomeClass
        {
            #region Fields

            public string name = "Example", key = "value";

            #endregion Fields
        }

        #endregion Classes
    }
}