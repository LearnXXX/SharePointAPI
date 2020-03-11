using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    public partial class GraphAPITester
    {
        private GraphServiceClient graphServiceClient;

        public GraphAPITester(string clientId, string tenantId, X509Certificate2 cert)
        {
            var application = ConfidentialClientApplicationBuilder.Create(clientId)
                        .WithCertificate(cert)
                        .WithAuthority(AzureCloudInstance.AzurePublic, tenantId, false)
                        .Build();
            var token = application.AcquireTokenForClient(new List<string> { "https://graph.microsoft.com/.default" }).ExecuteAsync().Result.AccessToken;

            graphServiceClient = new GraphServiceClient("https://graph.microsoft.com/v1.0", new DelegateAuthenticationProvider(a =>
           {
               a.Headers.Add("Authorization", "Bearer " + token);
               return Task.FromResult(0);
           }), new CustomHTTPProvider(new Serializer()));

        }


        public void GraphBatchTest()
        {
            string webGraphId = "m365x157144-my.sharepoint.com,47b37a14-09dd-407f-9509-6fa9b4ad20d4,08d6db6f-5165-4fd4-ad22-7a61201f766a";
            string listId = "a44cb590-eb05-45d4-bf28-30f73385cd3e";
            string parentFolderUrl = "/Folder1";
            //var sfdfdsd = graphServiceClient.Sites[webGraphId].Lists[listId].Items["06227e34-688e-4805-b37a-b73d8739e6a4_NotFound"].Versions.Request().WithMaxRetry(3).GetAsync().Result; ;

            var driveItems = GetDriveFiles(webGraphId, listId, parentFolderUrl);
            string expandQueryString = "fields($select=File_x0020_Size,EditorLookupId,AuthorLookupId,FileRef,Created,Modified,_UIVersionString,_UIVersion,_Level,ContentTypeId,FileLeafRef,FileDirRef,ID,UniqueId,FSObjType,HTML_x0020_File_x0020_Type,_ModerationStatus,_ModerationComments,Title,IsMyDocuments,XLuoTestField,SingleLineTest,MultipleLinesTest,ChoiceTest,NumberTest,CurrencyTest,DateTimeTest,LookupTestLookupId,YesOrNoTest,PersonOrGroupTestLookupId,HyperlinkTest,TaskOutcomTest,ManagedMetadataTest,MetaDataMultiTest,)";

            foreach (var driveItem in driveItems)
            {
                ConcurrentBag<int> sdfs;
                var ve = graphServiceClient.Sites[webGraphId].Lists[listId].Items[driveItem.SharepointIds.ListItemUniqueId].Versions.Request().GetAsync().Result; ;
                var items = graphServiceClient.Sites[webGraphId].Lists[listId].Items[driveItem.SharepointIds.ListItemUniqueId].Request().Expand(expandQueryString).GetAsync().Result;
                var itemRequestUrl = graphServiceClient.Sites[webGraphId].Lists[listId].Items[driveItem.SharepointIds.ListItemUniqueId].Request().Expand(expandQueryString).RequestUrl;
                var itemRequest = graphServiceClient.Sites[webGraphId].Lists[listId].Items[driveItem.SharepointIds.ListItemUniqueId].Request().Expand(expandQueryString);

                var itemVersionsRequestUrl = graphServiceClient.Sites[webGraphId].Lists[listId].Items[driveItem.SharepointIds.ListItemUniqueId].Versions.Request().RequestUrl;
                var itemPermisionRequetUrl = graphServiceClient.Sites[webGraphId].Lists[listId].Items[driveItem.SharepointIds.ListItemId].DriveItem.Permissions.Request().Select("inheritedFrom").RequestUrl;
                var request = new BatchRequestContent();
                var id1 = request.AddBatchRequestStep(new HttpRequestMessage(HttpMethod.Get, itemRequestUrl));
                var id2 = request.AddBatchRequestStep(new HttpRequestMessage(HttpMethod.Get, itemVersionsRequestUrl));
                var id3 = request.AddBatchRequestStep(new HttpRequestMessage(HttpMethod.Get, itemPermisionRequetUrl));
                var batchResult = graphServiceClient.Batch.Request().WithMaxRetry(3).PostAsync(request).Result;
                var item = batchResult.GetResponseByIdAsync<ListItem>(id1).Result;
                //var response = batchResult.GetResponseByIdAsync(id2).Result;

                //var content = response.Content.ReadAsStreamAsync().Result;
                //var serializer = new Serializer();
                //serializer.DeserializeObject<IListItemVersionsCollectionPage>(content);
                //var builder=  AsyncTaskMethodBuilder<ListItemVersionsCollectionPage>.Create();
                //var sdfsf = JsonConvert.DeserializeObject<ListItemVersionsCollectionPage>(content);
                var version = batchResult.GetResponseByIdAsync<ListItemVersionsCollectionResponse>(id2).Result;
                //var handler =  new ResponseHandler(graphServiceClient.HttpProvider.Serializer);
                //var result = handler.HandleResponse<IListItemVersionsCollectionPage>(response).Result;
                var permission = batchResult.GetResponseByIdAsync<DriveItemPermissionsCollectionResponse>(id3).Result;
            }
        }

    }
}
