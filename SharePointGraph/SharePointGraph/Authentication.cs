using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    class Authentication
    {
        private const string ClientId = "45d6d2e3-f4c2-4af3-9d50-79129c7e3645";
        private const string TenantId = "65001581-c366-4764-80ab-aef9bc86ecca";
        private const string ClientSecret = "64f=:]DsKCoZP9kfXJlw1EpTRwDn?N6M";

        public static string GetAccessTokenBySecret(string tenantId,string clientId, string clientSecret)
        {
            var app = ConfidentialClientApplicationBuilder.Create(clientId).WithTenantId(tenantId).WithClientSecret(clientSecret).Build();
            string[] scopes = new string[] { "https://graph.microsoft.com/.default", };
            return  app.AcquireTokenForClient(scopes).ExecuteAsync().Result.AccessToken;
        }


        public static string GetAccessTokenByCertificate(string tenantId, string clientId, X509Certificate2 certificate)
        {
            var app = ConfidentialClientApplicationBuilder.Create(clientId).WithTenantId(tenantId).WithCertificate(certificate).Build();
            string[] scopes = new string[] { "https://graph.microsoft.com/.default", };
            return app.AcquireTokenForClient(scopes).ExecuteAsync().Result.AccessToken;
        }

    }
}
