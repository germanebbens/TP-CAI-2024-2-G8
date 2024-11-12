using System;
using System.Configuration;

namespace ElectroHogar.Config
{
    public static class ConfigHelper
    {
        public static string GetValue(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new ConfigurationErrorsException($"La clave '{key}' no está configurada en App.config");
            }
            return value;
        }

        public static string GetValueOrDefault(string key, string defaultValue)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        public static int GetIntValue(string key)
        {
            string value = GetValue(key);
            if (!int.TryParse(value, out int result))
            {
                throw new ConfigurationErrorsException($"El valor para '{key}' debe ser un número entero");
            }
            return result;
        }

        public static int GetIntValueOrDefault(string key, int defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            return !string.IsNullOrEmpty(value) && int.TryParse(value, out int result) ? result : defaultValue;
        }
    }
}