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
        #region https://xluov.sharepoint.com
        private const string ClientId = "";
        private const string TenantId = "1a58e338-5637-4e10-88ce-591844ee5470";
        #endregion

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

            var sfs = System.Xml.XmlConvert.DecodeName("OData__x005f_x0031_x005f_2345");
            RenderListDataMethodTest.Test();
            //TestBatchRequest.Test();
            WebRequest.DefaultWebProxy = new System.Net.WebProxy("127.0.0.1", 8888);

            var accessToken = Authentication.GetAccessTokenByCertificateV1("https://xluov.sharepoint.com/.default", TenantId, ClientId, new X509Certificate2(System.IO.File.ReadAllBytes(@"C:\Users\xluo\Desktop\XluoCert.pfx"), "demo12!@"));
            Test(accessToken);
            accessToken = Authentication.GetAccessTokenByCertificateV2("https://longgod.sharepoint.com/", TenantId, ClientId, new X509Certificate2(GetCertificateBytes(), "demo12!@"));
            var cookies = Authentication.GetCookiesByUserInfo(new Uri(""), "admin@M365x157144.onmicrosoft.com", "X60LyQ995R");
        }
        private static void Test(string accessToken)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://xluov.sharepoint.com/sites/Test1/_api/Web/Lists(guid'7589db69-5a99-4e41-81da-162422a4d4e7')/items?$expand=FieldValuesAsText");
            request.Method = "GET";
            request.Accept = "application/json;odata=verbose";
            request.Headers.Add("Authorization", "bearer " + accessToken);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var content = reader.ReadToEnd();
            }
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
