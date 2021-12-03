using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class Role
    {
        public static void Test(ClientContext context)
        {
            context.Load(context.Web.RoleDefinitions);
            context.ExecuteQuery();
            var role = context.Web.RoleDefinitions[5];
            OutputBasePermissions(role,PermissionKind.AddAndCustomizePages);
            OutputBasePermissions(role,PermissionKind.AddDelPrivateWebParts);
            OutputBasePermissions(role,PermissionKind.AddListItems);
            OutputBasePermissions(role,PermissionKind.AnonymousSearchAccessList);
            OutputBasePermissions(role,PermissionKind.AnonymousSearchAccessWebLists);
            OutputBasePermissions(role,PermissionKind.ApplyStyleSheets);
            OutputBasePermissions(role,PermissionKind.ApplyThemeAndBorder);
            OutputBasePermissions(role,PermissionKind.ApproveItems);
            OutputBasePermissions(role,PermissionKind.BrowseDirectories);
            OutputBasePermissions(role,PermissionKind.BrowseUserInfo);
            OutputBasePermissions(role,PermissionKind.CancelCheckout);
            OutputBasePermissions(role,PermissionKind.CreateAlerts);
            OutputBasePermissions(role,PermissionKind.CreateGroups);
            OutputBasePermissions(role,PermissionKind.CreateSSCSite);
            OutputBasePermissions(role,PermissionKind.DeleteListItems);
            OutputBasePermissions(role,PermissionKind.DeleteVersions);
            OutputBasePermissions(role,PermissionKind.EditListItems);
            OutputBasePermissions(role,PermissionKind.EditMyUserInfo);
            OutputBasePermissions(role,PermissionKind.EmptyMask);
            OutputBasePermissions(role,PermissionKind.EnumeratePermissions);
            OutputBasePermissions(role,PermissionKind.FullMask);
            OutputBasePermissions(role,PermissionKind.ManageAlerts);
            OutputBasePermissions(role,PermissionKind.ManageLists);
            OutputBasePermissions(role,PermissionKind.ManagePermissions);
            OutputBasePermissions(role,PermissionKind.ManagePersonalViews);
            OutputBasePermissions(role,PermissionKind.ManageSubwebs);
            OutputBasePermissions(role,PermissionKind.ManageWeb);
            OutputBasePermissions(role,PermissionKind.Open);
            OutputBasePermissions(role,PermissionKind.OpenItems);
            OutputBasePermissions(role,PermissionKind.UpdatePersonalWebParts);
            OutputBasePermissions(role,PermissionKind.UseClientIntegration);
            OutputBasePermissions(role,PermissionKind.UseRemoteAPIs);
            OutputBasePermissions(role,PermissionKind.ViewFormPages);
            OutputBasePermissions(role,PermissionKind.ViewListItems);
            OutputBasePermissions(role,PermissionKind.ViewPages);
            OutputBasePermissions(role,PermissionKind.ViewUsageData);
            OutputBasePermissions(role,PermissionKind.ViewVersions);
            OutputBasePermissions(role,PermissionKind.ViewFormPages);
        }
        private static void OutputBasePermissions(RoleDefinition role, PermissionKind permissionKind)
        {
            if (role.BasePermissions.Has(permissionKind))
            {
                Console.WriteLine(permissionKind);

            }
        }
    }
}
