using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JSON_To_CSV
{
    public static class Converter
    {
        #region Methods

        private static List<string> GetRow(JObject obj, ref List<string> collumns, string col_name_prefix="")
        {
            List<string> row = new List<string>();

            foreach (JProperty token in obj.Children())
            {
                string colName = col_name_prefix + token.Name;

                Type token_type = token.Value.GetType();

                if (token_type == typeof(JObject))
                {
                    List<string> innerRow = GetRow((JObject)token.Value, ref collumns, token.Name+"-");

                    for (int i = row.Count; i < collumns.Count; i++) row.Add("");
                    for (int i = 0; i < row.Count; i++)
                    {
                        if (innerRow[i] != "") row[i] = innerRow[i];
                    }
                }
                else if (token_type == typeof(JArray))
                {
                    Console.WriteLine("Ignored JAarray");
                }
                else if (token_type == typeof(JValue))
                {
                    if (!collumns.Contains(colName))
                    {
                        collumns.Add(colName);
                    }

                    for (int i = row.Count; i < collumns.Count; i++) row.Add("");

                    int idx = collumns.FindIndex(0, x => x == colName);
                    try
                    {
                        row[idx] = token.Value.ToString().Replace("\r","<").Replace("\n", ">").Replace(",","");
                    }
                    catch
                    {
                        Console.WriteLine($"Cant Find Row {colName}");
                    }
                }
            }
            return row;
        }

        public static void WriteConversion(string file, JArray json)
        {
            List<string> collumns = new List<string>();
            List<List<string>> rows = new List<List<string>>();

            foreach (JObject obj in json)
            {
                rows.Add(GetRow(obj, ref collumns));
            }

            string col_names = String.Join(",", collumns);
            string csv = String.Join("\n", rows.Select(x => String.Join(",", x)));

            File.WriteAllText(file, $"{col_names}\n{csv}");
        }

        #endregion Methods
    }
}