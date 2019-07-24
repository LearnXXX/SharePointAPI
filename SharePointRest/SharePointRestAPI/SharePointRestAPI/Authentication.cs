using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestAPI
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
        public static CookieContainer GetCookiesByUserInfo(string userName, string password)
        {
            var token = new SharePointOnlineCredentials(userName, GetPassword(password)).GetAuthenticationCookie(new Uri("https://longgod.sharepoint.com/sites/XluoTest1"));

            string cookieName = token.Substring(0, token.IndexOf('='));
            string cookieValue = token.Substring(token.IndexOf('=') + 1);
            CookieContainer cookies = new CookieContainer();
            cookies.Add(new Cookie(cookieName, cookieValue, "/", "longgod.sharepoint.com"));
            return cookies;
        }

        public static string GetAccessTokenByCertificateV1(string scope,string tenantId, string clientId, X509Certificate2 certificate)
        {
            var app = ConfidentialClientApplicationBuilder.Create(clientId).WithTenantId(tenantId).WithCertificate(certificate).Build();
            string[] scopes = new string[] { scope };
            return app.AcquireTokenForClient(scopes).ExecuteAsync().Result.AccessToken;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource">https://longgod.sharepoint.com</param>
        /// <param name="tenantId"></param>
        /// <param name="clientId"></param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static string GetAccessTokenByCertificateV2(string resource,string tenantId, string clientId, X509Certificate2 certificate)
        {
            string authority = string.Format("https://login.windows.net/{0}", tenantId);
            AuthenticationContext context = new AuthenticationContext(authority, Microsoft.IdentityModel.Clients.ActiveDirectory.TokenCache.DefaultShared);
            var cac = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientAssertionCertificate(clientId.ToString(), certificate);
            return  context.AcquireTokenAsync(resource, cac).Result.AccessToken;

        }
    }
}
