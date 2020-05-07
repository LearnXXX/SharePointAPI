using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class SiteLevel
    {
        public static void GetSiteUserAndGroups(ClientContext context)
        {
            ////var user = context.Web.EnsureUser("i:0#.f|membership|admin@m365x157144.onmicrosoft.com");
            //context.Site.Owner = null;
            //context.ExecuteQuery();
            context.Load(context.Site.Owner);
            context.Load(context.Web.SiteUsers);
            context.ExecuteQuery();
        }
        public static void GetSiteOwner(ClientContext context)
        {
            context.Load(context.Site.Owner);
            context.Load(context.Site.RootWeb.CurrentUser);
            context.ExecuteQuery();
            context.Site.Owner = context.Site.RootWeb.CurrentUser;
            context.ExecuteQuery();
        }
        public static void RecycleBinTest(ClientContext context)
        {
            context.Load(context.Site.RecycleBin);
            context.ExecuteQuery();
            var items = CovertToRecycleBinItemList(context.Site.RecycleBin);
            foreach (var item in items)
            {
                if (item.LeafName == "F11")
                {
                    item.Restore();
                }
            }
            context.ExecuteQuery();
        }

        /// <summary>
        /// Convert to List and sort
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static List<RecycleBinItem> CovertToRecycleBinItemList(RecycleBinItemCollection items)
        {
            var recycleBinItems = items.ToList();
            recycleBinItems.Sort((x, y) =>
            {
                if (x.DeletedDate > y.DeletedDate)
                {
                    return 1;
                }
                if (x.DeletedDate < y.DeletedDate)
                {
                    return -1;
                }
                return -0;
            });
            return recycleBinItems;
        }

        public static void GetSiteSize(ClientContext context)
        {
            context.Load(context.Site, s => s.Usage);
            context.ExecuteQuery();
            double sizeGB = Math.Round((double)context.Site.Usage.Storage / (1024 * 1024 * 1024), 2);
        }
    }
}
