using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class Configuration
    {

        private static Configuration config = null;
        private string sharePointAdminUrl;

        private static void InlizeConfiguration()
        {
            config = new Configuration();
            System.Configuration.Configuration processConfig = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);

            #region Authentication

            config.Authority = processConfig.AppSettings.Settings["Authority"].Value;
            config.ClientId = processConfig.AppSettings.Settings["ClientId"].Value;
            config.sharePointAdminUrl = processConfig.AppSettings.Settings["SharePointAdminUrl"].Value;
            config.RedirectURL = processConfig.AppSettings.Settings["RedirectURL"].Value;
            config.ExtraQuery = processConfig.AppSettings.Settings["ExtraQuery"].Value;
            config.GraphUri = processConfig.AppSettings.Settings["GraphUri"].Value;
            #endregion

            #region ProxyInfo
            config.EnableProxy = Parse(processConfig.AppSettings.Settings["EnableProxy"].Value, false);

            if (config.EnableProxy)
            {
                var proxyAddress = processConfig.AppSettings.Settings["ProxyAddress"].Value;
                var proxyPort = Parse(processConfig.AppSettings.Settings["ProxyPort"].Value, 0);
                var proxyUserName = processConfig.AppSettings.Settings["ProxyUserName"].Value;
                var proxyPassword = processConfig.AppSettings.Settings["ProxyPassword"].Value;
                config.Proxy = new ProxyInfo(proxyUserName, proxyPassword, string.Format("{0}:{1}", proxyAddress, proxyPort));
            }

            #endregion
        }

        private static int Parse(string value, int defaultValue)
        {
            int tempValue;

            if (!int.TryParse(value, out tempValue))
            {
                tempValue = defaultValue;
            }
            return tempValue;
        }

        private static bool Parse(string value, bool defaultValue)
        {
            bool tempValue;

            if (!bool.TryParse(value, out tempValue))
            {
                tempValue = defaultValue;
            }
            return tempValue;
        }

        public static Configuration Config
        {
            get
            {
                if (config == null)
                {
                    InlizeConfiguration();
                }
                return config;
            }
        }

        public string GraphUri
        {
            get;
            private set;
        }

        public string ExtraQuery
        {
            get;
            private set;
        }

        public bool EnableProxy
        {
            get;
            private set;
        }

        public ProxyInfo Proxy
        {
            get;
            private set;
        }

        public string RedirectURL
        {
            get;
            private set;
        }

        public string ClientId
        {
            get;
            private set;
        }

        public string Authority
        {
            get;
            private set;
        }
    }
}
