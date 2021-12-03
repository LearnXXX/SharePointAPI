using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class WebLevel
    {
        public static void FeatureTest(ClientContext context)
        {
            context.Load(context.Web.Features);
            context.ExecuteQuery();
            foreach (var feature in context.Web.Features)
            {
                var result = context.Web.IsFeatureActive(feature.DefinitionId);
                if (!result)
                {

                }
            }
        }
        public static bool CheckListExist(ClientContext context, string listUrl)
        {
            var file = context.Web.GetFileByServerRelativePath(ResourcePath.FromDecodedUrl("/sites/Test5/Shared Documents/20200611142343.txt"));
            context.Load(file);
            context.ExecuteQuery();
            bool siteAssetsExist = false;
            List list = null;
            ExceptionHandlingScope scope = new ExceptionHandlingScope(context);
            using (scope.StartScope())
            {
                using (scope.StartTry())
                {
                    list = context.Web.GetList(listUrl);
                    context.Load(list, l => l.Title, l => l.IsSiteAssetsLibrary);
                }
                using (scope.StartCatch())
                { }
            }
            context.ExecuteQuery();
            if (scope.HasException)
            {
            }
            else if (list.ServerObjectIsNull.HasValue && !list.ServerObjectIsNull.Value)
            {
                siteAssetsExist = true;//list.IsSiteAssetsLibrary;
            }
            return siteAssetsExist;
        }
        public static void GetAllListsInWeb(ClientContext context)
        {
            var web = context.Site.OpenWeb("/sites/XluoTest1/NintexWorkflowforOffice365");
            var list = web.Lists.GetById(new Guid("730c9a52-15dd-4a37-91b0-16a4a1d2a3b2"));
            context.Load(list);
            context.ExecuteQuery();
        }

        public static void GetLitByTitle(ClientContext context)
        {
            context.Load(context.Web.Lists, lists => lists.Include(l => l.Title));
            context.ExecuteQuery();
            foreach (var temp in context.Web.Lists)
            {

            }
            //var list = context.Web.Lists.GetByTitle("Documents");
            var list = context.Web.Lists.GetByTitle("Dokumente");
            context.Load(list);
            context.ExecuteQuery();
        }

    }
}
