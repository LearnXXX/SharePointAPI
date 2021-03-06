﻿using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class UserLevel
    {
        public static void ResolveUser(ClientContext context)
        {

            string input = Guid.NewGuid().ToString();

            var user = context.Web.EnsureUser(input);
            context.Load(user);
            context.ExecuteQuery();


            ClientPeoplePickerQueryParameters searchParams = new ClientPeoplePickerQueryParameters()
            {
                AllowEmailAddresses = true,
                AllowMultipleEntities = true,
                QueryString = input,
                Required = true,
                PrincipalType = PrincipalType.SecurityGroup | PrincipalType.User,
                PrincipalSource = PrincipalSource.All,
                MaximumEntitySuggestions = 30
            };
            ClientResult<string> UserInfos = ClientPeoplePickerWebServiceInterface.ClientPeoplePickerResolveUser(context, searchParams);
            context.ExecuteQuery();



        }
        public static void SiteUsers(ClientContext context)
        {
            context.Load(context.Site.RootWeb.SiteUsers);
            context.ExecuteQuery();
            foreach (var user in context.Site.RootWeb.SiteUsers)
            {
                var tempUserName = user.LoginName;
                if (user.LoginName.IndexOf('|') > 0)
                {
                    tempUserName = user.LoginName.Substring(user.LoginName.IndexOf('|') + 1);
                }
                if (tempUserName.IndexOf("|") == 6)
                {
                    tempUserName = tempUserName.Substring(7, tempUserName.Length - 7);
                }
            }
        }

        public static void GetUserByLoginName(ClientContext context)
        {
            //var user = context.Site.RootWeb.SiteUsers.GetByLoginName((context.Credentials as SharePointOnlineCredentials).UserName);
            var user = context.Site.RootWeb.SiteUsers.GetByLoginName("i:0#.f|membership|aosiptest@longgod.onmicrosoft.com");
            context.Load(user);
            context.ExecuteQuery();
        }
    }
}
