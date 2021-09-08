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

        private static void AddToRow(string colName, string value, ref List<string> row, ref List<string> collumns)
        {
            colName.Replace("=", "");
            if (!collumns.Contains(colName))
            {
                collumns.Add(colName);
            }

            EnsureLength(ref row, collumns.Count);

            int idx = collumns.FindIndex(0, x => x == colName);
            try
            {
                row[idx] = value.ToString().Replace("\r", "<").Replace("\n", ">").Replace(",", "");
            }
            catch
            {
                Console.WriteLine($"Cant Find Row {colName}");
            }
        }

        private static void EnsureLength(ref List<string> row, int makeLength)
        {
            for (int i = row.Count; i < makeLength; i++) row.Add("");
        }

        private static List<string> GetRow(JObject obj, ref List<string> collumns, string col_name_prefix = "")
        {
            List<string> row = new List<string>();

            foreach (JProperty token in obj.Children())
            {
                string colName = col_name_prefix + token.Name;

                Type token_type = token.Value.GetType();

                if (token_type == typeof(JObject))
                {
                    List<string> innerRow = GetRow((JObject)token.Value, ref collumns, col_name_prefix + token.Name + "-");

                    EnsureLength(ref row, collumns.Count);
                    MergeRow(ref row, innerRow);
                }
                else if (token_type == typeof(JArray))
                {
                    ProcessJArray((JArray)token.Value, ref collumns, ref row, col_name_prefix + token.Name + "-");
                }
                else if (token_type == typeof(JValue))
                {
                    AddToRow(colName, token.Value.ToString(), ref row, ref collumns);
                }
            }
            return row;
        }

        private static void MergeRow(ref List<string> row, List<string> subRow)
        {
            for (int i = 0; i < row.Count; i++)
            {
                if (subRow[i] != "") row[i] = subRow[i];
            }
        }

        private static void ProcessJArray(JArray jArray, ref List<string> collumns, ref List<string> row, string col_name_prefix = "")
        {
            int item_idx = 0;
            foreach (JToken item in jArray)
            {
                if (item.Type == JTokenType.Array) ProcessJArray((JArray)item, ref collumns, ref row, col_name_prefix + item_idx + "-");
                else if (item.Type == JTokenType.Object)
                {
                    List<string> innerRow = GetRow((JObject)item, ref collumns, col_name_prefix + item_idx + "-");

                    EnsureLength(ref row, collumns.Count);
                    MergeRow(ref row, innerRow);
                }
                else
                {
                    string colName = col_name_prefix + item_idx;
                    AddToRow(colName, item.ToString(), ref row, ref collumns);
                }

                item_idx++;
            }
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