using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MBUretim.Mvc.Models.Logger
{
    public class Logger
    {
        public Logger()
        {
        }

        public static void Log(string stringText)
        {
            StreamWriter streamWriter;
            string directoryName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            directoryName = string.Concat(directoryName, "\\UretimLogs");
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            DateTime now = DateTime.Now;
            string str = string.Concat(directoryName, "\\Uretim_Log_", now.ToString("dd-MM-yyyy"), ".txt");
            streamWriter = (File.Exists(str) ? File.AppendText(str) : new StreamWriter(str));
            streamWriter.WriteLine(string.Format("{0} {1}", DateTime.Now, stringText));
            streamWriter.Close();
            streamWriter.Dispose();
        }

        public static void LogError(string stringText)
        {
            StreamWriter streamWriter;
            string directoryName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            directoryName = string.Concat(directoryName, "\\UretimLogs");
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            DateTime now = DateTime.Now;
            string str = string.Concat(directoryName, "\\Uretim_ErrorLog_", now.ToString("dd-MM-yyyy"), ".txt");
            streamWriter = (File.Exists(str) ? File.AppendText(str) : new StreamWriter(str));
            streamWriter.WriteLine(string.Format("{0} {1}", DateTime.Now, stringText));
            streamWriter.Close();
            streamWriter.Dispose();
        }
    }
}