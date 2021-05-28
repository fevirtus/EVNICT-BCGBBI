using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace BI_Project.Helpers
{
    public enum LogTypeJson { Access, Error, Action };
    public class FileHelper
    {
        public static string FileReadAllText(string filePath)
        {
            string output = String.Empty;

            output = File.ReadAllText(filePath);
            return output;
        }

        public static void SaveFile(object content, string fileName)
        {
            string json = JsonConvert.SerializeObject(content, Formatting.Indented);
            string currentTime = JsonConvert.SerializeObject(new { Datetime = DateTime.Now }, Formatting.Indented);
            File.WriteAllText(fileName, currentTime);
            File.WriteAllText(fileName, json);            
            return;
        }


    }
}