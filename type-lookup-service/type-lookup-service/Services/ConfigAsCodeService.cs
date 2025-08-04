using System;
using System.IO;

namespace type_lookup_service.Services
{
    public class ConfigAsCodeService
    {
        public string LoadConfig()
        {
            var filePath = Environment.GetEnvironmentVariable("CAC_CONFIG_PATH");
            if (string.IsNullOrEmpty(filePath))
            {
                var msg = "{ \"message\" : \"ERROR: CAC_CONFIG_PATH not set\" }";
                return msg;
            }

            var configText = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(configText))
            {
                var msg = "{ \"message\" : \"ERROR:  Unable to load config file.\" }";
                return msg;
            }


            return configText;
        }
    }
}
