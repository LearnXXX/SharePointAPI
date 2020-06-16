using CommandLine;
using log4net;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools.PEQA
{
    class AddMultipleItems
    {

        private static ILog logger = LogManager.GetLogger(typeof(AddMultipleItems));
        private static TokenHelper tokenHelper = new TokenHelper();
        public static void Run(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<AddMultipleItemsOptions>(args).WithParsed<AddMultipleItemsOptions>(o =>
                {
                    o.CheckArguement();
                    Run(o);
                });
            }
            catch (Exception e)
            {
                logger.ErrorFormat("An error occurred while create site collections, error: {0}", e);
            }
            finally
            {
                LogManager.Flush(10 * 1000);
            }
        }

        private static List<string> FilterSiteCollections(AddMultipleItemsOptions option)
        {
            logger.Info($"Start searh site collections with {option.KeyWord}");
            var siteUrls = new List<string>();
            var tenantContext = tokenHelper.GetClientContextForServiceAccount(option.AdminUrl, option.UserName, option.Password);
            tenantContext.RequestTimeout = 6000 * 1000;
            Tenant tenant = new Tenant(tenantContext);
            SPOSitePropertiesEnumerableFilter filter = new SPOSitePropertiesEnumerableFilter
            {
                //IncludeDetail = true,
                IncludePersonalSite = PersonalSiteFilter.Exclude,
                Template = "STS",
                Filter = $"Url -like '{option.KeyWord}'",
            };

            string tempIndex = null;
            do
            {
                filter.StartIndex = tempIndex;
                var sitePropertyEnum = tenant.GetSitePropertiesFromSharePointByFilters(filter);
                tenantContext.Load(sitePropertyEnum);
                tenantContext.ExecuteQuery();
                foreach (SiteProperties siteProperty in sitePropertyEnum)
                {
                    siteUrls.Add(siteProperty.Url.TrimEnd('/'));
                }
                tempIndex = sitePropertyEnum.NextStartIndexFromSharePoint;
            }
            while (tempIndex != null);
            logger.Info($"Finish searh site collections with {option.KeyWord}, site collection count: {siteUrls.Count}");
            return siteUrls;
        }

        public static void Run(AddMultipleItemsOptions option)
        {

            var siteUrls = FilterSiteCollections(option);
            int index = 0;
            foreach (var siteUrl in siteUrls)
            {
                try
                {
                    logger.Info($"Start add files to {siteUrl}");
                    var context = tokenHelper.GetClientContextForServiceAccount(siteUrl, option.UserName, option.Password);
                    context.RequestTimeout = 6000 * 1000;
                    var list = context.Web.Lists.GetByTitle("Documents");
                    context.Load(list, l => l.BaseType, l => l.ItemCount);
                    context.ExecuteQuery();
                    if (list.BaseType == BaseType.DocumentLibrary)
                    {
                        if (list.ItemCount < option.Count)
                        {
                            AddFilesToLibrary(list, context, option.Count - list.ItemCount);
                        }
                    }
                    else
                    {
                        if (list.ItemCount < option.Count)
                        {
                            AddListItemsToList(list, context, option.Count - list.ItemCount);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error($"An error while add item to {siteUrl}, error: {e.ToString()}");
                }
                index++;
                logger.Info($"Add items to {siteUrl} finished, finished site count: {index}, total site collection count: {siteUrls.Count}");

            }
        }
        public static void AddListItemsToList(List list, ClientContext context, int count)
        {
            try
            {
                for (int index = 1; index <= count; index++)
                {
                    if (index % 100 == 0)
                    {
                        Console.WriteLine("Add {0} listitems finished", index);
                        context.ExecuteQuery();
                    }

                    var item = list.AddItem(new ListItemCreationInformation { LeafName = index.ToString(), });
                    item["Title"] = index;
                    item.Update();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("An error occurred while add list items to library, error: {0}", e.ToString());

            }
            if (context.HasPendingRequest)
            {
                context.ExecuteQuery();
            }
        }

        public static void AddFilesToLibrary(List library, ClientContext context, int count)
        {
            var folder = library.RootFolder;

            DateTime date = DateTime.Now;
            try
            {
                for (int index = 0; index < count; index++)
                {
                    date = date.AddHours(1);
                    if (index % 100 == 0)
                    {
                        Console.WriteLine("Add {0} files finished", index);
                        context.ExecuteQuery();
                    }

                    var info = new FileCreationInformation { Content = System.Text.Encoding.Default.GetBytes("1"), Url = date.ToString("yyyyMMddHHmmss") + ".txt", };
                    var file = folder.Files.Add(info);
                    file.ListItemAllFields.Update();

                }
                context.ExecuteQuery();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("An error occurred while add files to library, error: {0}", e.ToString());
            }
            if (context.HasPendingRequest)
            {
                context.ExecuteQuery();
            }
        }
    }
}
