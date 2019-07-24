using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    class Program
    {
        private const string ClientId = "45d6d2e3-f4c2-4af3-9d50-79129c7e3645";
        private const string TenantId = "65001581-c366-4764-80ab-aef9bc86ecca";
        private const string ClientSecret = "64f=:]DsKCoZP9kfXJlw1EpTRwDn?N6M";

        private static byte[] GetCertificateBytes()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SharePointGraph.ReferenceFiles.RestAPICertificate.pfx"))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        static void Main(string[] args)
        {


            var accessToken = Authentication.GetAccessTokenBySecret(TenantId, ClientId, ClientSecret);


            //var accessToken = Authentication.GetAccessTokenByCertificate(TenantId, ClientId, new X509Certificate2(GetCertificateBytes(), "demo12!@"));

           
            RestAPITest(accessToken);
            dynamic siteInfo = GetSiteCollectionByUrl(accessToken, "https://longgod.sharepoint.com/sites/XluoTest1/");
            GetListsWithSystem(accessToken, (string)siteInfo.id);
        }

        private static void RestAPITest(string accessToken)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://longgod.sharepoint.com/sites/XluoTest1/_api/web");
            request.Method = "GET";
            request.Accept = "application/json;odata=verbose";
            request.Headers.Add("Authorization",
  "bearer " + accessToken);
            HttpWebResponse response =
  (HttpWebResponse)request.GetResponse();

        }

        private static JObject GetSiteCollectionByUrl(string token, string siteUrl)
        {
            var siteUri = new Uri(siteUrl);
            string webApiUrl = string.Format("{0}/sites/{1}:{2}", GraphAPIVersion.V1, siteUri.Host, siteUri.AbsolutePath);
            return GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }
        private static void GetSubSites(string token, string siteId)
        {
            string webApiUrl = string.Format("{0}/sites/{1}/sites", GraphAPIVersion.V1, siteId);
            dynamic siteInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
            var subSiteCount = siteInfo.value.Count;
        }

        private static void GetListsWithoutSystem(string token, string siteId)
        {
            string webApiUrl = string.Format("{0}/sites/{1}/lists", GraphAPIVersion.V1, siteId);
            dynamic allListsInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
            var subSiteCount = allListsInfo.value.Count;
        }

        private static void GetListsWithSystem(string token, string siteId)
        {
            string webApiUrl = string.Format("{0}/sites/{1}/lists/daf86790-5321-4e47-bcaf-5b89fc441ef8/items", GraphAPIVersion.V1, siteId);
            dynamic allListsInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
            var subSiteCount = allListsInfo.value.Count;
        }
    }
}
