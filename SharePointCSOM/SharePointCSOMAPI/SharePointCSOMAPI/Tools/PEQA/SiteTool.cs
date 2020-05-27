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
    class SiteTool
    {
        private static ILog logger = LogManager.GetLogger(typeof(ScanSubSiteDocumentLibrary));
        private static TokenHelper tokenHelper = new TokenHelper();

        public static void Run(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<SiteToolOptions>(args).WithParsed<SiteToolOptions>(o =>
                {
                    o.CheckArguement();
                    CreateMultipleSiteCollections(o);

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

        private static bool IsSiteExist(ClientContext context, Tenant tenant, string siteUrl)
        {
            var newSiteProps = tenant.GetSitePropertiesFromSharePointByFilter(string.Format("Url -eq '{0}'", System.Web.HttpUtility.UrlPathEncode(siteUrl)), "0", false);
            context.Load(newSiteProps);
            context.ExecuteQuery();
            bool siteExist = newSiteProps.Count > 0;
            return siteExist;
        }
        public static void WaitSiteCreateFinished(ClientContext context, Tenant tenant, List<string> siteUrls)
        {
            foreach (var siteUrl in siteUrls)
            {
                SiteProperties siteProperties = null;
                bool errorOccurred = false;
                logger.InfoFormat("Satrt check {0} site status", siteUrl);
                do
                {
                    errorOccurred = false;
                    try
                    {
                        System.Threading.Thread.Sleep(10000);
                        var newSiteProps = tenant.GetSitePropertiesFromSharePointByFilter(string.Format("Url -eq '{0}'", System.Web.HttpUtility.UrlPathEncode(siteUrl)), "0", false);
                        context.Load(newSiteProps);
                        context.ExecuteQuery();
                        siteProperties = newSiteProps.FirstOrDefault();
                        if (siteProperties != null)
                        {
                            logger.InfoFormat("Site Collection Url: {0}, Site Collection Status:{1}", siteUrl, siteProperties.Status);
                        }
                    }
                    catch (Exception e)
                    {
                        string message = e.Message;
                        logger.Warn("An error occurred while getting site properties. Error:{0}", e);
                        errorOccurred = true;
                    }
                }
                while (errorOccurred || siteProperties == null || string.Equals("Creating", siteProperties.Status, StringComparison.OrdinalIgnoreCase));
                logger.InfoFormat("Create site collction {0} successfully", siteUrl);
            }
        }

        public static void CreateMultipleSiteCollections(SiteToolOptions option)
        {
            var context = tokenHelper.GetClientContextForServiceAccount(option.AdminUrl, option.UserName, option.Password);
            Tenant tenant = new Tenant(context);
            var creatingSites = new List<string>();
            for (int index = 1; index <= option.Count; index++)
            {
                var siteUrl = option.Url + index.ToString();
                if (IsSiteExist(context, tenant, siteUrl))
                {
                    logger.Info($"{0} aleady exist in tenant");
                    continue;
                }
                SpoOperation ope = tenant.CreateSite(
                new SiteCreationProperties()
                {
                    CompatibilityLevel = 15,
                    Lcid = 1033,
                    Owner = option.UserName,
                    Template = option.Template,
                    TimeZoneId = 45,
                    Title = "",
                    Url = siteUrl,
                    StorageMaximumLevel = 0,
                    UserCodeMaximumLevel = 0,
                    UserCodeWarningLevel = Math.Floor(0 * 0.85),
                    StorageWarningLevel = (long)Math.Floor(0 * 0.85)
                });
                context.Load(ope);
                context.ExecuteQuery();
                creatingSites.Add(siteUrl);
                if (creatingSites.Count >= option.Section)
                {
                    WaitSiteCreateFinished(context, tenant, creatingSites);
                    creatingSites.Clear();
                }
            }
            WaitSiteCreateFinished(context, tenant, creatingSites);
        }
    }
}
