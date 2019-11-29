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

        public static void GetLitByTitle(ClientContext context)
        {
            context.Load(context.Web.Lists,lists=>lists.Include(l=>l.Title));
            context.ExecuteQuery();
            foreach (var temp in context.Web.Lists)
            {

            }
            //var list = context.Web.Lists.GetByTitle("Documents");
            var list = context.Web.Lists.GetByTitle("Dokumente");
            context.Load(list);
            context.ExecuteQuery();
        }

    }
}
