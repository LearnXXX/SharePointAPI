﻿using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class SiteLevel
    {
        public static void SiteChangeTest(ClientContext context)
        {
            context.ExecutingWebRequest+=delegate (object sender, WebRequestEventArgs e)
            {
                e.WebRequestExecutor.WebRequest.Headers.Add("x-RecycleBinAware", "True");
            };
            context.Load(context.Site);
            context.Load(context.Site.RootWeb.Lists);
            context.ExecuteQuery();
            var siteId = context.Site.Id;
            ChangeQuery query = new ChangeQuery(true, true);
            ChangeToken startToken = new ChangeToken();
            ChangeToken endToken = new ChangeToken();
            startToken.StringValue = "1;1;" + siteId.ToString() + ";" + DateTime.UtcNow.AddDays(-1).Ticks.ToString() + ";-1";
            endToken.StringValue = "1;1;" + siteId.ToString() + ";" + DateTime.UtcNow.Ticks.ToString() + ";-1";
            query.ChangeTokenStart = startToken;
            query.ChangeTokenEnd = endToken;
            ChangeCollection changedCollection = context.Site.GetChanges(query);
            context.Load(changedCollection);
            context.ExecuteQuery();
            foreach (Change changeObject in changedCollection)
            {

            }

        }
        public static void LoadSiteProperties(ClientContext context)
        {
            context.Load(context.Site);
            context.Load(context.Web);
            context.ExecuteQuery();
        }

        public static void GetSiteUserAndGroups(ClientContext context)
        {
            ////var user = context.Web.EnsureUser("i:0#.f|membership|admin@m365x157144.onmicrosoft.com");
            //context.Site.Owner = null;
            //context.ExecuteQuery();
            context.Load(context.Site.Owner);
            context.Load(context.Web.SiteUsers);
            context.ExecuteQuery();
        }
        public static void Test1(ClientContext context)
        {
            var list = context.Web.Lists.GetByTitle("wfsvc");
            context.Load(list);
            context.Load(list.RootFolder);
            context.ExecuteQuery();
        }


        public static void GetSiteOwner(ClientContext context)
        {

            var owner = context.Site.RootWeb.EnsureUser("i:0#.f|membership|shuo.liu@lsazures.onmicrosoft.com");
            context.Site.Owner = owner;
            context.ExecuteQuery();
            context.Site.RootWeb.AllProperties["1231234"] = "23566";
            context.Site.RootWeb.Update();
            context.ExecuteQuery();

            var user = context.Site.RootWeb.EnsureUser("c:0t.c|tenant|60dbc52e-a24d-4201-8da2-f9fd969e462a");
            context.Load(user);
            context.ExecuteQuery();
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
