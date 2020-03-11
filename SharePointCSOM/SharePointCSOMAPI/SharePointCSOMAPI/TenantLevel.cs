using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class TenantLevel
    {
        public static void Test(ClientContext context)
        {
            Tenant tenant = new Tenant(context);

            SPOSitePropertiesEnumerableFilter filter = new SPOSitePropertiesEnumerableFilter
            {
                IncludeDetail = true,
                IncludePersonalSite = PersonalSiteFilter.Include,
                Template = "SPSPERS"
            };
            var sitePropertyEnum = tenant.GetSitePropertiesFromSharePointByFilters(filter);
            context.Load(sitePropertyEnum);
            context.ExecuteQuery();
        }

    }
}
