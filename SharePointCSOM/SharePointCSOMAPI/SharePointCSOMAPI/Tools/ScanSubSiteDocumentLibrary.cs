using log4net;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools
{
    /// <summary>
    /// 该 Tool用于scan 和root web 时区不同的subsite下的Document Library
    /// </summary>
    class ScanSubSiteDocumentLibrary
    {
        private static ILog logger = LogManager.GetLogger(typeof(ScanSubSiteDocumentLibrary));
        public static void Scan(ClientContext context)
        {
            StringBuilder report = new StringBuilder();
            context.Load(context.Site.RootWeb.RegionalSettings, rs => rs.TimeZone);
            context.ExecuteQuery();
            ScanSubWeb(context, context.Site.RootWeb, context.Site.RootWeb.RegionalSettings.TimeZone, report);
            System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),"Report.txt"), report.ToString());
            logger.InfoFormat("Scan Finish!!");
            Console.ReadKey();
        }

        private static void ScanSubWeb(ClientContext context, Web parentWeb, Microsoft.SharePoint.Client.TimeZone rootTimeZone, StringBuilder report)
        {
            context.Load(parentWeb.Webs, web => web.Include(w => w.Url, w => w.RegionalSettings.TimeZone));
            context.ExecuteQuery();
            foreach (var subWeb in parentWeb.Webs)
            {
                logger.InfoFormat("Start scan {0}", subWeb);
                if (subWeb.RegionalSettings.TimeZone.Id != rootTimeZone.Id)
                {
                    context.Load(subWeb.Lists, lists => lists.Include(l => l.BaseType, l => l.RootFolder.ServerRelativeUrl, l => l.Title, l => l.Hidden,l => l.ItemCount));
                    context.ExecuteQuery();
                    foreach (var list in subWeb.Lists)
                    {
                        if (list.BaseType == BaseType.DocumentLibrary && list.Hidden == false)
                        {
                            report.AppendLine(string.Format("{0}, {1}", string.Format("https://{0}/{1}", new Uri(subWeb.Url).Host, list.RootFolder.ServerRelativeUrl.TrimStart('/')), list.ItemCount));
                        }
                    }
                }
                ScanSubWeb(context, subWeb, rootTimeZone, report);
            }
        }
    }
}
