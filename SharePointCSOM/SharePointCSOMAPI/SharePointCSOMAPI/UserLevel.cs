using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class UserLevel
    {

        public static void GetUserByLoginName(ClientContext context)
        {
            
            //var user = context.Site.RootWeb.SiteUsers.GetByLoginName((context.Credentials as SharePointOnlineCredentials).UserName);
            var user = context.Site.RootWeb.SiteUsers.GetByLoginName("i:0#.f|membership|aosiptest@longgod.onmicrosoft.com");
            context.Load(user);
            context.ExecuteQuery();
        }
    }
}
