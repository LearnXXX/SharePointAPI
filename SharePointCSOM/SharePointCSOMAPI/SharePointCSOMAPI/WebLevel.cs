using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class WebLevel
    {
        public static void GetAllListsInWeb(ClientContext context)
        {
            var web = context.Site.OpenWeb("/sites/XluoTest1/NintexWorkflowforOffice365");
            var list = web.Lists.GetById(new Guid("730c9a52-15dd-4a37-91b0-16a4a1d2a3b2"));
            context.Load(list);
            context.ExecuteQuery();
        }

    }
}
