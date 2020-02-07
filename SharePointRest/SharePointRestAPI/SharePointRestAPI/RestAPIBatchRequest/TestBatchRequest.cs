using Microsoft.Data.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharePointRestAPI
{
    class TestBatchRequest
    {
        public static void Test()
        {
            WebRequest.DefaultWebProxy = new WebProxy("127.0.0.1", 8888);

            var sharepointUrl = "https://m365x157144-my.sharepoint.com/personal/admin_m365x157144_onmicrosoft_com";
            var listRetrievalCount = 0;
            //经测试 只有Bearer token 才好用，cookies方式使用$branch方式会出错
            var accessToken = Authentication.GetAccessTokenByCertificateV2("https://m365x157144-my.sharepoint.com/", "b4b8748a-a573-4e49-8665-3a632b65a60c", "50927317-52bf-40c0-a4f1-9f19d04049a0", new X509Certificate2(System.IO.File.ReadAllBytes(@"C:\Users\xluo\Desktop\XluoCert.pfx"), "demo12!@"));
            
            // Create the parent request
            var batchRequest = new BatchODataRequest(String.Format("{0}/_api/", sharepointUrl)); // ctor adds "$batch"
            batchRequest.SetHeader("Authorization", "Bearer " + accessToken);
            //batchRequest.Cookie = cookies;
            using (var oDataMessageWriter = new ODataMessageWriter(batchRequest))
            {
                var oDataBatchWriter = oDataMessageWriter.CreateODataBatchWriter();
                oDataBatchWriter.WriteStartBatch();

                // Create the two child query operations.
                oDataBatchWriter.CreateOperationRequestMessage(
                     "GET", new Uri(sharepointUrl.ToString() + "/_api/Web/lists/getbytitle('Composed Looks')/items?$select=Title"));
                listRetrievalCount++;

                oDataBatchWriter.CreateOperationRequestMessage(
                   "GET", new Uri(sharepointUrl.ToString() + "/_api/Web/lists/getbytitle('User Information List')/items?$select=Title"));
                listRetrievalCount++;

                oDataBatchWriter.WriteEndBatch();
                oDataBatchWriter.Flush();
            }

            // Parse the response and bind the data to the UI controls
            var oDataResponse = batchRequest.GetResponse();

            using (var oDataReader = new ODataMessageReader(oDataResponse))
            {
                var oDataBatchReader = oDataReader.CreateODataBatchReader();

                while (oDataBatchReader.Read())
                {
                    switch (oDataBatchReader.State)
                    {
                        case ODataBatchReaderState.Initial:

                            // Optionally, handle the start of a batch payload.
                            break;
                        case ODataBatchReaderState.Operation:

                            // Start of an operation (either top-level or in a changeset)
                            var operationResponse = oDataBatchReader.CreateOperationResponseMessage();

                            // Response's ATOM markup parsing and presentation section
                            using (var stream = operationResponse.GetStream())
                            {
                                List<XElement> entries = ListDataHelper.ExtractListItemsFromATOMResponse(stream);

                                var itemTitles = ListDataHelper.GetItemTitles(entries);

                                // Bind data to the grid on the page.
                                // In a production app, check operationResponse.StatusCode and handle non-200 statuses.
                                // For simplicity, this sample assumes status 200 (the list items are returned).
                                switch (listRetrievalCount)
                                {
                                    case 2:
                                        var title1 = itemTitles;
                                        listRetrievalCount--;
                                        break;

                                    case 1:
                                        var title2 = itemTitles;
                                        listRetrievalCount--;
                                        break;
                                }
                            };
                            break;
                        case ODataBatchReaderState.ChangesetStart:
                            // Optionally, handle the start of a change set.
                            break;

                        case ODataBatchReaderState.ChangesetEnd:
                            // When this sample was created, SharePoint did not support "all or nothing" transactions. 
                            // If that changes in the future this is where you would commit the transaction.
                            break;

                        case ODataBatchReaderState.Exception:
                            // In a producition app handle exeception. Omitted for simplicity in this sample app.
                            break;
                    }
                }
            }
        }
    }
}
