using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class UtilityLevevl
    {

        public static void SendEmail(ClientContext context)
        {
            var list = context.Web.GetListByTitle("Sync Logs");
            var contentType = list.GetContentTypeByName("Item");
            context.Load(list,l=>l.DefaultDisplayFormUrl);
            context.Load(contentType);
            context.ExecuteQuery();
            //System.Web.p
            var url = "https://test.cmom";
            EmailProperties email = new EmailProperties {
                To = new string[] { "admin@M365x522548.onmicrosoft.com","Nicole.kong@nicole123456.partner.onmschina.cn" },
                //Body = $@"<html>
                //            <body>
                //            <p>Dear Administrator,</p>
                //            <p>The sync job 123 is failed, Click <a href='{url}'>hear</a> to find details in the system.</p>
                //            <p> Thanks </p>
                //            </body>
                //            </html> ",
                Body = $"<html><body><p>Dear Administrator,</p><p>The sync job 123 is failed, Click <a href='https://sdfsdf.com'>hear</a> to find details in the system.</p><p> Thanks </p></body></html> ",
                //Body= "<a href=\"https://www.google.com\">Test Visit google!</a>",
                Subject = "SP LMS Bulk Upload failure",
                
            };
            Utility.SendEmail(context, email);
            context.ExecuteQuery();

        }
    }
}
