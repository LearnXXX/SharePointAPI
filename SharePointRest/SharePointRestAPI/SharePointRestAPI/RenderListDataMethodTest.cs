using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestAPI
{
    class RenderListDataMethodTest
    {
        public static void Test()
        {
            WebRequest.DefaultWebProxy = new WebProxy("127.0.0.1", 8888);

            string viewXml = "<View><FieldRef Name='ID'/><FieldRef Name='Title'/></View>";//"<Where><Geq><FieldRef Name=\"Modified\" /><Value Type=\"DateTime\" IncludeTimeValue=\"TRUE\" StorageTZ=\"TRUE\">2015-08-05T15:50:08</Value></Geq></Where>";
            string requestUrl = "https://xluov.sharepoint.com/sites/Test1/_api/web/lists/GetBYId('%7Bcb8c8ad9-13fc-44cd-8a00-622f76b5bff3%7D')/RenderListData?$expand=Versions";
            var dataType = "application/json;odata=verbose";
            //var dataType = "application/atom+xml;odata=verbose";
            var accessToken = Authentication.GetAccessTokenByCertificateV2("https://xluov.sharepoint.com/", "1a58e338-5637-4e10-88ce-591844ee5470", "ed438ecd-b165-4dd2-a681-ed55b25e7069", new X509Certificate2(System.IO.File.ReadAllBytes(@"C:\Users\xluo\Desktop\XluoCert.pfx"), "demo12!@"));

            var handler = new WebRequestHandler();
            var client = new HttpClient(handler, true);

            client.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);
            //client.BaseAddress = new Uri(requestUrl);
            var content = new MemoryStream(Encoding.UTF8.GetBytes("{" + string.Format("\"viewXml\":\" {0}\"", viewXml) + "}"));
            //var content = new MemoryStream(System.IO.File.ReadAllBytes(@"C:\Users\xluo\Desktop\cont.txt"));
            var streamContent = new StreamContent(content);

            streamContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(dataType);

            var result = client.PostAsync(requestUrl, streamContent).Result;

            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
            //request.Method = "POST";
            //request.Accept = dataType;
            //request.Headers.Add("Authorization", "bearer " + accessToken);

            //var stream = request.GetRequestStream();
            //stream.
            //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();


        }
    }
}
