using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonToCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            var allJsonPaths = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.json", SearchOption.AllDirectories);
            List<Dictionary<string,string>> jsons = new List<Dictionary<string, string>>();

            foreach (var path in allJsonPaths)
            {
                if (path.Contains("manifest")) continue;
                var jsonString = File.ReadAllText(path);
                jsons.Add(JObject.Parse(jsonString).Descendants().OfType<JValue>()
                    .ToDictionary(jv => jv.Path, jv => jv.ToString()));
            }

            IEnumerable<string> keys = new List<string>();
            foreach (var dict in jsons)
            {
                keys = keys.Union(dict.Keys);
            }

            var finalData = new List<List<string>>();

            foreach (var item in jsons)
            {
                var itemData = new List<string>();
                foreach (var column in keys)
                {
                    itemData.Add(item.ContainsKey(column) ? item[column] : null);
                }
                finalData.Add((itemData));
            }

            List<string> csvLines = new List<string>();
            csvLines.Add(string.Join("@", keys));
            foreach (var data in finalData)
            {
                csvLines.Add(string.Join("@",data));
            }

            File.WriteAllLines("output.csv", csvLines);
        }

    }
}
