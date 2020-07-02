using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace SharePointGraph
{
    class Program
    {
        private const string ClientId = "";
        private const string TenantId = "65001581-c366-4764-80ab-aef9bc86ecca";
        private const string ClientSecret = "64f=:]DsKCoZP9kfXJlw1EpTRwDn?N6M";

        private const string XLUOVClientId = "";
        private const string XLUOVTenantId = "1a58e338-5637-4e10-88ce-591844ee5470";
        static X509Certificate2 LoadCertificate()
        {
            //var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SharePointGraph.exportedcert.pfx");

            using (var stream = System.IO.File.OpenRead(@"C:\Users\xluo\Desktop\XluoCert.pfx"))
            {
                using (var binaryReader = new BinaryReader(stream))
                {
                    var rawData = binaryReader.ReadBytes((Int32)stream.Length);
                    return new X509Certificate2(rawData, "demo12!@");
                }
            }
        }


        private static X509Certificate2 GetGraphCertificate()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SharePointGraph.GraphCertificates.XluoCert.pfx"))
            {
                using (var binaryReader = new BinaryReader(stream))
                {
                    var rawData = binaryReader.ReadBytes((Int32)stream.Length);
                    return new X509Certificate2(rawData, "demo12!@");
                }
            }

        }
        private static byte[] GetRestCertificateBytes()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SharePointGraph.ReferenceFiles.RestAPICertificate.pfx"))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        private static List<T> GetRequestAllOfDatas<T>(dynamic currenPage, List<T> datas)
        {
            datas.AddRange(currenPage.CurrentPage as List<T>);
            if (currenPage.NextPageRequest != null)
            {
                var nextPage = currenPage.NextPageRequest.GetAsync().Result;
                GetRequestAllOfDatas(nextPage, datas);
            }
            return datas;
        }


        private static void GraphTest()
        {
            var canceled = new CancellationToken();
            WebRequest.DefaultWebProxy = new System.Net.WebProxy("127.0.0.1", 8888);
            var clientId = "50927317-52bf-40c0-a4f1-9f19d04049a0";
            var tenantId = "b4b8748a-a573-4e49-8665-3a632b65a60c";
            var application = ConfidentialClientApplicationBuilder.Create(clientId)
                        .WithCertificate(LoadCertificate())
                        .WithAuthority(AzureCloudInstance.AzurePublic, tenantId, false)
                        .Build();
            var token = application.AcquireTokenForClient(new List<string> { "https://graph.microsoft.com/.default" }).ExecuteAsync().Result.AccessToken;

            //var graphService = new GraphServiceClient("https://graph.microsoft.com/v1.0", new DelegateAuthenticationProvider(a =>
            //{
            //    a.Headers.Add("Authorization", "Bearer " + token);
            //    return Task.FromResult(0);
            //}));
            var graphService = new GraphServiceClient("https://graph.microsoft.com/v1.0", new DelegateAuthenticationProvider(a =>
            {
                a.Headers.Add("Authorization", "Bearer " + token);
                return Task.FromResult(0);
            }), new CustomHTTPProvider(new Serializer()));
            var siteUrl = "https://m365x157144-my.sharepoint.com/personal/admin_m365x157144_onmicrosoft_com";
            var siteUri = new Uri(siteUrl);
            //var p2 = graphService.Sites["m365x157144-my.sharepoint.com,47b37a14-09dd-407f-9509-6fa9b4ad20d4,08d6db6f-5165-4fd4-ad22-7a61201f766a"].Drive.Items["01Z2O2D6JXDOWJEWGK7JBYDG26AR75AAV6"].Permissions.Request().GetAsync();
            //var perm1 = p2.Result;
            var perm2 = graphService.Sites["m365x157144-my.sharepoint.com,47b37a14-09dd-407f-9509-6fa9b4ad20d4,08d6db6f-5165-4fd4-ad22-7a61201f766a"].Lists["a44cb590-eb05-45d4-bf28-30f73385cd3e"].Items["8828"].DriveItem.Permissions.RequestUrl;
            var perm3 = graphService.Sites["m365x157144-my.sharepoint.com,47b37a14-09dd-407f-9509-6fa9b4ad20d4,08d6db6f-5165-4fd4-ad22-7a61201f766a"].Lists["a44cb590-eb05-45d4-bf28-30f73385cd3e"].Items["8828"].DriveItem.Permissions.RequestUrl;
            var request = new BatchRequestContent();
            var requestID1 = request.AddBatchRequestStep(new HttpRequestMessage(HttpMethod.Get, perm2));
            var requestID2 = request.AddBatchRequestStep(new HttpRequestMessage(HttpMethod.Get, perm3));
            var ssssfs = graphService.Batch.Request().WithMaxRetry(3).PostAsync(request).Result;
            var r1 = ssssfs.GetResponseByIdAsync<Permission>(requestID1).Result;
            var r2 = ssssfs.GetResponseByIdAsync<Permission>(requestID2).Result;
            var fields = graphService.Sites["m365x157144-my.sharepoint.com,47b37a14-09dd-407f-9509-6fa9b4ad20d4,08d6db6f-5165-4fd4-ad22-7a61201f766a"].Lists["a44cb590-eb05-45d4-bf28-30f73385cd3e"].Request().Expand("columns").GetAsync().Result;
            var itemsss = graphService.Sites["m365x157144-my.sharepoint.com,47b37a14-09dd-407f-9509-6fa9b4ad20d4,08d6db6f-5165-4fd4-ad22-7a61201f766a"].Lists["a44cb590-eb05-45d4-bf28-30f73385cd3e"].Drive.Root.ItemWithPath("ffb").Children.Request().GetAsync().Result;




            var driveList = graphService.Sites["m365x157144-my.sharepoint.com,47b37a14-09dd-407f-9509-6fa9b4ad20d4,08d6db6f-5165-4fd4-ad22-7a61201f766a"].Lists["a44cb590-eb05-45d4-bf28-30f73385cd3e"].Drive.Root.Request().GetAsync().Result;
            var items = graphService.Drives["b!FHqzR90Jf0CVCW-ptK0g1G_b1ghlUdRPrSJ6YSAfdmqQtUykBevURb8oMPczhc0-"].Items.Request().GetAsync().Result;
            var list = graphService.Sites["m365x157144-my.sharepoint.com,47b37a14-09dd-407f-9509-6fa9b4ad20d4,08d6db6f-5165-4fd4-ad22-7a61201f766a"].Lists.Request().GetAsync().Result;

            var drive = graphService.Drives["b!FHqzR90Jf0CVCW-ptK0g1G_b1ghlUdRPrSJ6YSAfdmqQtUykBevURb8oMPczhc0-"].Items["01Z2O2D6OOCLSJHS435ZDJEGBMCLVP7YG4"].Versions["1.0"].Content.Request().GetAsync().Result;

            var memberStream = new MemoryStream();
            drive.CopyTo(memberStream);
            System.IO.File.WriteAllBytes(@"C:\Users\xluo\Desktop\File2.docx", memberStream.ToArray());
            var driveItem = graphService.Drive.Items["01Z2O2D6OOCLSJHS435ZDJEGBMCLVP7YG4"].Request().GetAsync().Result;

            var result = graphService.Sites.GetByPath("/personal/admin_m365x157144_onmicrosoft_com", "m365x157144-my.sharepoint.com").Request().GetAsync().Result;


        }
        private static void ADALUsernameTest()
        {
            var clientId = "12128f48-ec9e-42f0-b203-ea49fb6af367";
            var tenantId = "b4b8748a-a573-4e49-8665-3a632b65a60c";
            var application = PublicClientApplicationBuilder.Create(clientId)
             .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
             //.WithRedirectUri("urn:ietf:wg:oauth:2.0:oob")
             .Build();

            var username = "admin@M365x157144.onmicrosoft.com";
            var password = "X60LyQ995R";
            var url = "https://graph.microsoft.com/.default";
            var authenticationResult = application.AcquireTokenByUsernamePassword(new List<string> { url.ToString() }, username, GetPassword(password))
                    .ExecuteAsync().Result;

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
        static void Main(string[] args)
        {
            WebRequest.DefaultWebProxy = new System.Net.WebProxy("127.0.0.1", 8888);
            GraphAPITester tester = new GraphAPITester(XLUOVClientId, XLUOVTenantId, GetGraphCertificate());
            var w = Stopwatch.StartNew();
            //5KFoldersAnd5KFiles : b!j9o9KsE_LU2n_KEOpEqjEDdaShSRC1RHhFnytKAck9TI1HqcROTqR6uJNCgoEbyR  013TTTP5N6Y2GOVW7725BZO354PWSELRRZ
            //10KFilesIn1Folder :   b!j9o9KsE_LU2n_KEOpEqjEDdaShSRC1RHhFnytKAck9RMNmw8-bpLS6L1Ktv-bu9G   013TTTP5N6Y2GOVW7725BZO354PWSELRRZ
            //Documents: b!j9o9KsE_LU2n_KEOpEqjEDdaShSRC1RHhFnytKAck9TnRNgoOXHVT5lHMk7EZqqx     013TTTP5N6Y2GOVW7725BZO354PWSELRRZ     5folders and 2000files under folder
            //var items = tester.GetAllItemsUnderFolder("b!j9o9KsE_LU2n_KEOpEqjEDdaShSRC1RHhFnytKAck9RMNmw8-bpLS6L1Ktv-bu9G", "013TTTP5N6Y2GOVW7725BZO354PWSELRRZ");
            w.Stop();
            Console.WriteLine($"GetAllSubFolders:{w.Elapsed.TotalSeconds}");
            var w2 = Stopwatch.StartNew();
            var deltaItems = tester.DeltaTest("b!j9o9KsE_LU2n_KEOpEqjEDdaShSRC1RHhFnytKAck9RMNmw8-bpLS6L1Ktv-bu9G");
            w2.Stop();
            Console.WriteLine($"DeltaTest:{w2.Elapsed.TotalSeconds}");
            tester.GetDriveFiles("xluov.sharepoint.com,2a3dda8f-3fc1-4d2d-a7fc-a10ea44aa310,144a5a37-0b91-4754-8459-f2b4a01c93d4", "28d844e7-7139-4fd5-9947-324ec466aab1", "2KFiles");

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
