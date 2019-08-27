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
        public static void GetSiteSize(ClientContext context)
        {
            context.Load(context.Site,s=>s.Usage);
            context.ExecuteQuery();
            double sizeGB = Math.Round((double)context.Site.Usage.Storage/ (1024 * 1024 * 1024),2);
        }
    }
}
