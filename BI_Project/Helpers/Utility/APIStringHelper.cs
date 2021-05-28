using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Helpers.Utility
{
    public class APIStringHelper
    {

        public static string GenerateId1()
        {
            long currentUnixTime = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            return currentUnixTime.ToString();
        }

        public static string GenerateFileId()
        {
            string output = "";
            DateTime now = DateTime.UtcNow;
            output = now.Year.ToString() + "_" + now.Month.ToString() + "_" + now.Day + "_" + now.Hour+"_" + now.Minute+"_" + now.Second +"_"+ now.Millisecond;
            return output;
        }
        public static string UrlEncode(string encodedValue)
        {
            return System.Net.WebUtility.UrlEncode(encodedValue);
        }

        /// <summary>
        /// wait to write continue
        /// </summary>
        /// <param name="unittime"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static string GenKeyDecryptAndEncrypt(string unittime, int duration)
        {
            string output = "";
            int current_unix_time = 0, start = 0;
            DateTime current_day = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);

            switch (unittime)
            {
                case "hour":
                    start = (Int32)(current_day.Subtract(new DateTime(1970, 1, 1))).TotalHours;
                    current_unix_time = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalHours;
                    if (current_unix_time - start > duration)
                    {

                    }
                    break;
                case "day":
                    break;
            }

            return output;
        }

        public static string GenerateId()
        {
            string output = "";
            //DateTime now = DateTime.Now;
            Random rd = new Random();
            //int item = rd.Next(100, 999);
            output = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            //output = output + "." + item;
            output = output.Replace(" ", "_");
            output = output.Replace("-", "_");
            output = output.Replace(":", "_");
            output = output.Replace(".", "_");
            return output;
        }

    }
}
