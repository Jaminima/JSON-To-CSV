using System.Linq;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using System.IO;

namespace JSON_To_CSV
{
    public static class Converter
    {
        public static void WriteConversion(string file, JArray json)
        {
            List<string> collumns = new List<string>();
            List<List<string>> rows = new List<List<string>>();

            foreach (JObject obj in json)
            {
                List<string> row = new List<string>();
                for (int i = 0; i < collumns.Count; i++) row.Add("");

                foreach (JProperty token in obj.Children())
                {
                    if (!collumns.Contains(token.Name))
                    {
                        collumns.Add(token.Name);
                        row.Add("");
                    }

                    int idx = collumns.FindIndex(0,x=>x == token.Name);
                    row[idx] = token.Value.ToString();
                }
                rows.Add(row);
            }

            string col_names = String.Join(",", collumns);
            string csv = String.Join("\n", rows.Select(x => String.Join(",", x)));

            File.WriteAllText(file, $"{col_names}\n{csv}");
        }
    }
}
