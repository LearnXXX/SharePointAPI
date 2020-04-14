using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class ListLevel
    {
        public static void LoadListProperty(ClientContext context)
        {
            var list = context.Web.Lists.GetByTitle("DeltaTest");
            context.Load(list.ContentTypes);
            context.Load(list);
            context.Load(list.RootFolder);
            context.ExecuteQuery();
            context.Load(list,l=>l.HasUniqueRoleAssignments);
            context.ExecuteQuery();
            context.Load(list.Fields);
            var filed = list.Fields.GetFieldByInternalName("_ModerationStatus");
            context.Load(filed);
            var fields = list.Fields.Where(f => !f.Hidden && "_Hidden" != f.Group && !f.ReadOnlyField);
            foreach (var field in list.Fields.Where(f => !f.Hidden && "_Hidden" != f.Group && !f.ReadOnlyField))
            { }
            context.ExecuteQuery();
        }
    }
}
