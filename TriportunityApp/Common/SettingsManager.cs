using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SettingsManager : ISettingsManager
    {
        public string ReadSettings(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? String.Empty;
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine($"Error reading app settings {e.Message} ");
                return String.Empty;
            }
        }
    }
}