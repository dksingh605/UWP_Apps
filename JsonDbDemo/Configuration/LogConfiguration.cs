using Serilog;
using System;
using System.Net;
using System.Net.Mail;
using Windows.Storage;

namespace JsonDbDemo
{
    public class LogConfiguration
    {
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        LogConfiguration()
        {

        }
       
        public static Serilog.Core.Logger GetFileLogger()
        {
            
            return
              new LoggerConfiguration()
                    .WriteTo.RollingFile(ApplicationData.Current.LocalFolder.Path + @"\JsonDbDemo-{Date}.txt")
                    .CreateLogger();
        }
        public static Serilog.Core.Logger GetFileLogger(string logFileName)
        {
           
            return
              new LoggerConfiguration()
                    .WriteTo.RollingFile(ApplicationData.Current.LocalFolder.Path + @"\"+logFileName.ToString()+"-{Date}.txt")
                    .CreateLogger();
        }
       
    }
}
