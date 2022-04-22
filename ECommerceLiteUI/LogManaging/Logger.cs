using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.LogManaging
{
    public static class Logger
    {

        public static void LogMessage(string message, string page="-", string user="-")
        {
            try
            {

              string fileName = "ECommerceLite303Logs_" + DateTime.Now.ToString("dd/MM/yyyy") + ".txt";
              string directoryPath =Path.Combine(HttpContext.Current.Server.MapPath("~/Logs/"));
                string filePath = Path.Combine(directoryPath, fileName);

                if(!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                StreamWriter writer = new StreamWriter(filePath, append: true);

                writer.Flush();

                writer.WriteLine($"" + $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}" +
                    $"\t User:{user}\tPage: {page}\t Message: {message}");

                writer.Close();
      
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        








    }
}