using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
namespace BI_Project.Helpers
{
    public enum LogType { Access, Error, Action };
    public static class Logging
    {
        static Logging()
        {

        }
        private static readonly object locker = new object();

        public static void WriteToLog(string message, LogType t)
        {
            lock (locker)
            {
                StreamWriter SW;

                string path = "";
                switch (t)
                {
                    case LogType.Access:
                        path = HostingEnvironment.MapPath("~/resources/logs/access.log"); break;
                    case LogType.Error:
                        path = HostingEnvironment.MapPath("~/resources/logs/error.log"); break;
                    case LogType.Action:
                        path = HostingEnvironment.MapPath("~/resources/logs/action.log"); break;
                }

                SW = File.AppendText(path);
                SW.WriteLine("{0:G}: {1}", DateTime.Now, message);
                SW.Close();
            }
        }
    }
}