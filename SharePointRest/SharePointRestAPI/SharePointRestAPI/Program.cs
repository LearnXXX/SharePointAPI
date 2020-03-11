using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestAPI
{
    class Program
    {
        private const string ClientId = "45d6d2e3-f4c2-4af3-9d50-79129c7e3645";
        private const string TenantId = "65001581-c366-4764-80ab-aef9bc86ecca";

        private static byte[] GetCertificateBytes()
        {
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SharePointRestAPI.Certificate.RestAPICertificate.pfx"))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        static void Main(string[] args)
        {
            RenderListDataMethodTest.Test();
            TestBatchRequest.Test();

            var accessToken = Authentication.GetAccessTokenByCertificateV1("https://longgod.sharepoint.com/.default", TenantId, ClientId, new X509Certificate2(GetCertificateBytes(), "demo12!@"));
            accessToken = Authentication.GetAccessTokenByCertificateV2("https://longgod.sharepoint.com/", TenantId, ClientId, new X509Certificate2(GetCertificateBytes(), "demo12!@"));
            var cookies = Authentication.GetCookiesByUserInfo(new Uri(""), "admin@M365x157144.onmicrosoft.com", "X60LyQ995R");
        }

        private static void Test(string accessToken)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://longgod.sharepoint.com/sites/XluoTest1/_api/web");
            request.Method = "GET";
            request.Accept = "application/json;odata=verbose";
            request.Headers.Add("Authorization", "bearer " + accessToken);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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

        private static void UseRestAPIByCookies(CookieContainer cookies)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://longgod.sharepoint.com/sites/XluoTest1/_api/web");
            request.Method = "GET";
            request.Accept = "application/json;odata=verbose";
            request.CookieContainer = cookies;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }
    }
}
