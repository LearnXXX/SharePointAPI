using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class Authentication
    {
        private static SecureString GetPassword(string password)
        {
            SecureString pwd = new SecureString();
            foreach (char c in password)
            {
                pwd.AppendChar(c);
            }
            return pwd;
        }

        public static SharePointOnlineCredentials GetCredentials(string userName, string password)
        {
            return new SharePointOnlineCredentials(userName, GetPassword(password));
        }

        public static ClientContext GetClientContext(string siteUrl, string userName, string password)
        {
            var context = new ClientContext(siteUrl);
            context.Credentials = new SharePointOnlineCredentials(userName, GetPassword(password));
            return context;
        }
    }
}
