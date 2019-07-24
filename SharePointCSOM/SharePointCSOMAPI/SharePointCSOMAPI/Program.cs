using Microsoft.SharePoint.Client;
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
        static void Main(string[] args)
        {
        }

        private static SecureString GetPassword(string password)
        {
            SecureString pwd = new SecureString();
            foreach (char c in password)
            {
                pwd.AppendChar(c);
            }
            return pwd;
        }

        private static ClientContext Context
        {
            get
            {
                if (context == null)
                {
                    string siteUrl = "https://longgod.sharepoint.com/sites/XluoTest1";
                    string userName = "aosiptest@longgod.onmicrosoft.com";
                    string password = "demo12!@";
                    context = new ClientContext(siteUrl);
                    context.Credentials = new SharePointOnlineCredentials(userName, GetPassword(password));
                    context.ExecuteQuery();
                }

                //if (context == null)
                //{
                //    string siteUrl = "https://tingtest123.sharepoint.com/sites/Shen01";
                //    string userName = "TingTing@TingTest123.onmicrosoft.com";
                //    string password = "1qaz2wsx!@";
                //    context = new ClientContext(siteUrl);
                //    context.Credentials = new SharePointOnlineCredentials(userName, GetPassword(password));
                //    context.ExecuteQuery();
                //}
                return context;
            }
        }
    }
}
