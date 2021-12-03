using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Authentication
{
    class MSALTest
    {

        public static void Test()
        {
            X509Certificate2 apponlyCertificate = new X509Certificate2(@"C:\Users\xluo\Desktop\Xluov Cert\XluovCer.pfx", "demo12!@");
            ConfidentialClientApplicationBuilder applicationBuilder = ConfidentialClientApplicationBuilder.Create("ed438ecd-b165-4dd2-a681-ed55b25e7069")
              .WithAuthority(AzureCloudInstance.AzurePublic, "1a58e338-5637-4e10-88ce-591844ee5470", false)
              //.WithHttpClientFactory(new MsalHttpClientProxyFactory(base.authParameters.WebProxy))
              .WithCertificate(apponlyCertificate);
            //if (!string.IsNullOrEmpty(base.authParameters.RedirectUri))
            //{
            //    applicationBuilder.WithRedirectUri(base.authParameters.RedirectUri);
            //}
            var application = applicationBuilder.Build();


            Uri uri;
            string scopeString = "https://xluov.sharepoint.com/sites/Test3";
            if (Uri.TryCreate(scopeString, UriKind.RelativeOrAbsolute, out uri))
            {
                scopeString = string.Format("{0}/.default", uri.GetLeftPart(UriPartial.Authority));
            }
            var scopes = new List<string> { scopeString };

            var token = application.AcquireTokenForClient(scopes)
                    .WithForceRefresh(true)
                    .ExecuteAsync().Result;

        }
    }
}
