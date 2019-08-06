using Microsoft.SharePoint.Client;
using SharePointCSOMAPI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class Program
    {
        private static ClientContext context = null;
        private static string siteUrl = "https://longgod.sharepoint.com/sites/XluoTest1";
        private static string userName = "aosiptest@longgod.onmicrosoft.com";
        private static string password = "demo12!@";
        static void Main(string[] args)
        {
            MetadataService.Test1(Authentication.GetClientContext(siteUrl, userName, password));
            WebLevel.GetAllListsInWeb(Authentication.GetClientContext(siteUrl, userName,password));
        }

    }
}
