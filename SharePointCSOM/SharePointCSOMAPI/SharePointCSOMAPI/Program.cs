using log4net;
using Microsoft.SharePoint.Client;
using SharePointCSOMAPI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class Program
    {
        private static ILog logger = LogManager.GetLogger(typeof(Program));
        //private static string siteUrl = "https://longgod.sharepoint.com/sites/XluoTest1";
        private static string siteUrl = "https://longgod-my.sharepoint.com/personal/long_longgod_onmicrosoft_com";
        private static string userName = "aosiptest@longgod.onmicrosoft.com";
        private static string password = "demo12!@";
        private static TokenHelper tokenHelper = new TokenHelper();

        static void Main(string[] args)
        {
            Initalize();


            SiteLevel.GetSiteSize(tokenHelper.GetClientContextForAppToken(siteUrl));
            SiteLevel.GetSiteSize(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            UserLevel.GetUserByLoginName(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            MetadataService.Test1(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            WebLevel.GetAllListsInWeb(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
        }

        private static void Initalize()
        {
            if (Configuration.Config.EnableProxy)
            {
                logger.InfoFormat("Use proxy with {0}", Configuration.Config.Proxy.Address);
                WebRequest.DefaultWebProxy = new System.Net.WebProxy(Configuration.Config.Proxy.Address) { Credentials = new NetworkCredential(Configuration.Config.Proxy.Username, Configuration.Config.Proxy.Password) };
            }
            else
            {
                logger.InfoFormat("Use system default proxy");
                WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();
            }
        }

    }
}
