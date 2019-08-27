using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class TokenHelper
    {
        private TokenProvider tokenProvider;

        public TokenHelper()
        {
            tokenProvider = new TokenProvider();
        }

        public ClientContext GetClientContextForServiceAccount(string siteUrl, string userName, string password)
        {
            var context = new ClientContext(siteUrl);
            context.Credentials = new SharePointOnlineCredentials(userName, GetPassword(password));
            return context;
        }

        public ClientContext GetClientContextForAppToken(string targetUrl)
        {
            ClientContext clientContext = new ClientContext(targetUrl);
            clientContext.AuthenticationMode = ClientAuthenticationMode.Anonymous;
            clientContext.FormDigestHandlingEnabled = false;
            clientContext.ExecutingWebRequest +=
                delegate (object oSender, WebRequestEventArgs webRequestEventArgs)
                {
                    var tokenResult = GetToken(targetUrl);
                    webRequestEventArgs.WebRequestExecutor.RequestHeaders["Authorization"] =
                        "Bearer " + tokenResult.AccessToken;
                };

            return clientContext;
        }

        public SharePointOnlineCredentials GetCredentials(string userName, string password)
        {
            return new SharePointOnlineCredentials(userName, GetPassword(password));
        }

        private AuthenticationResult GetToken(string targetUrl)
        {
            var url = new Uri(targetUrl);
            string scope = string.Format("{0}://{1}/", url.Scheme, url.Host);
            return tokenProvider.GetAccessToken(scope);
        }

        private SecureString GetPassword(string password)
        {
            SecureString pwd = new SecureString();
            foreach (char c in password)
            {
                pwd.AppendChar(c);
            }
            return pwd;
        }

    }
}
