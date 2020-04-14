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
            var list = context.Site.RootWeb.Lists.GetById(new Guid("068a0483-c851-4e05-b679-5e3f1e690de0"));
            context.ExecuteQuery();

            var item = list.GetItemById(3);
            //var item = list.GetItemById(2);
            context.Load(item);
            context.ExecuteQuery();
            context.Load(item.RoleAssignments, r => r.Include(a => a.PrincipalId, async => async.RoleDefinitionBindings, a => a.Member));
            context.Load(context.Site.RootWeb.SiteUsers);
            context.ExecuteQuery();
        }
    }
}
