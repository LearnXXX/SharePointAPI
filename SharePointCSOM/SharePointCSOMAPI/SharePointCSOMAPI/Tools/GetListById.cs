using log4net;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools
{
    class GetListById
    {
        private static ILog logger = LogManager.GetLogger(typeof(GetListById));

        public static void GetListByIdR(ClientContext context)
        {
            var webServerRelativeUrl = "/sites/XluoTest3/Sub1";
            var listId = new Guid("c65d9f85-76a7-443d-a8a1-f09bb672bfa6");
            Web web = context.Site.OpenWeb(webServerRelativeUrl);
            List list = web.Lists.GetById(listId);
            LoadList(context, list);

            logger.InfoFormat("List ValidationFormula: {0}", list.ValidationFormula);
            logger.InfoFormat("List ValidationMessage: {0}", list.ValidationMessage);
            logger.InfoFormat("List OnQuickLaunch: {0}", list.OnQuickLaunch);
            logger.InfoFormat("List RootFolder: {0}", list.RootFolder);
            logger.InfoFormat("List IsSiteAssetsLibrary: {0}", list.IsSiteAssetsLibrary);
            logger.InfoFormat("List HasUniqueRoleAssignments: {0}", list.HasUniqueRoleAssignments);
            logger.InfoFormat("List Id: {0}", list.Id);
            logger.InfoFormat("List ItemCount: {0}", list.ItemCount);
            logger.InfoFormat("List DefaultDisplayFormUrl: {0}", list.DefaultDisplayFormUrl);
            logger.InfoFormat("List DefaultViewUrl: {0}", list.DefaultViewUrl);
        }
        private static string LoadList(ClientContext context, List list)
        {
            ExceptionHandlingScope scope = new ExceptionHandlingScope(context);
            using (scope.StartScope())
            {
                using (scope.StartTry())
                {
                    context.Load(list);
                    context.Load(list, l => l.ValidationFormula,
                                              l => l.ValidationMessage,
                                              l => l.OnQuickLaunch,
                                              //l => l.SchemaXml,
                                              l => l.RootFolder,
                                              l => l.IsSiteAssetsLibrary,
                                              l => l.HasUniqueRoleAssignments,
                                              l => l.DataSource,
                                              l => l.Id,
                                              l => l.ItemCount,
                                              l => l.DefaultDisplayFormUrl,
                                              l => l.DefaultViewUrl);
                }
                using (scope.StartCatch())
                {
                    context.Load(list);
                    context.Load(list, l => l.ValidationFormula,
                                              l => l.ValidationMessage,
                                              l => l.OnQuickLaunch,
                                              //l => l.SchemaXml,
                                              //l => l.RootFolder,
                                              //l => l.IsSiteAssetsLibrary,
                                              l => l.HasUniqueRoleAssignments,
                                              l => l.DataSource,
                                              l => l.Id,
                                              l => l.ItemCount);
                }
            }
            context.ExecuteQuery();
            if (scope.HasException)
            {
                logger.WarnFormat("Has Exception: {0}", scope.ErrorMessage);
                return scope.ErrorMessage;
            }
            logger.Info("get list info successfully");
            return null;
        }
    }
}

