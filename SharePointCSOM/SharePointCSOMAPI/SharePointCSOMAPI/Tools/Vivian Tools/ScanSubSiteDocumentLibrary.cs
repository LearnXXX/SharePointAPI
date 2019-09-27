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
            var listsInfo = new List<ListInfo>();

            context.Load(context.Site.RootWeb.RegionalSettings, rs => rs.TimeZone);
            context.ExecuteQuery();
            using (var reader = new System.IO.StreamReader(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "sites.csv")))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        var url = line.Split(',')[0];
                        var relativeUrl = new Uri(url).AbsolutePath;
                        ScanSubWeb(context, relativeUrl, context.Site.RootWeb.RegionalSettings.TimeZone, listsInfo);
                    }
                }
            }

            System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ScanResult.xml"), SerializerHelper.SerializeObjectToString(listsInfo));
            logger.InfoFormat("Scan Finish!!");
            Console.ReadKey();
        }

        private static void ScanSubWeb(ClientContext context, string webUrl, Microsoft.SharePoint.Client.TimeZone rootTimeZone, List<ListInfo> listsInfo)
        {
            var web = context.Site.OpenWeb(webUrl);
            context.Load(web, w => w.Url, w => w.RegionalSettings.TimeZone);
            context.ExecuteQuery();

            logger.InfoFormat("Start scan {0}", web.Url);
            if (web.RegionalSettings.TimeZone.Id != rootTimeZone.Id)
            {
                context.Load(web.Lists, lists => lists.Include(
                    l => l.BaseType,
                    l => l.Id,
                    l => l.RootFolder.ServerRelativeUrl, 
                    l => l.Title,
                    l => l.Hidden, 
                    l => l.MajorVersionLimit, 
                    l => l.EnableVersioning, 
                    l => l.EnableMinorVersions, 
                    l => l.MajorWithMinorVersionsLimit, 
                    l => l.EnableModeration, 
                    l => l.ItemCount));
                context.ExecuteQuery();
                foreach (var list in web.Lists)
                {
                    if (list.BaseType == BaseType.DocumentLibrary && list.Hidden == false && list.ItemCount > 0)
                    {
                        var listInfo = new ListInfo
                        {
                            WebUrl = web.Url,
                            listId = list.Id,
                            EnableMinorVersions = list.EnableMinorVersions,
                            EnableModeration = list.EnableModeration,
                            EnableVersioning = list.EnableVersioning,
                            ItemCount  = list.ItemCount,
                            ListTitle = list.Title,
                            MajorVersionLimit = list.MajorVersionLimit,
                            MajorWithMinorVersionsLimit = list.MajorWithMinorVersionsLimit,
                            BigList = list.ItemCount>=5000
                        };
                        listsInfo.Add(listInfo);
                    }
                }
            }
        }
    }
}
