using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class Navigation
    {
        public static void NavigationTest(ClientContext context)
        {
            context.Load(context.Web.Navigation);
            context.Load(context.Web.Navigation, w => w.TopNavigationBar);
            context.Load(context.Web.Navigation, w => w.QuickLaunch);
            context.ExecuteQuery();
        }
    }
}
