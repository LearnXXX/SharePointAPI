using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class ListItemLevel
    {
        public static void LoadItemProperties(ClientContext context)
        {
            var list = context.Site.RootWeb.Lists.GetById(new Guid("997805ea-c20c-493e-b3ee-8627100604d6"));
            var user = list.GetItemById(5);
            var group = list.GetItemById(7);
            //var item = list.GetItemById(2);
            context.Load(user);
            context.Load(group);
            context.ExecuteQuery();
            context.Load(user.RoleAssignments, r => r.Include(a => a.PrincipalId, async => async.RoleDefinitionBindings, a => a.Member));
            context.Load(context.Site.RootWeb.SiteUsers);
            context.ExecuteQuery();
        }
    }
}
