using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools
{
    class CheckFilesExit
    {
        public static void Run()
        {
            StringBuilder ExistFiles = new StringBuilder();
            StringBuilder NoneFiles = new StringBuilder();
            StringBuilder FailedFiles = new StringBuilder();

            var sourceHostName = "http://companyweb";
            var DestinationHostName = "https://kieferusa.sharepoint.com";
            var context = Authentication.GetClientContext("https://kieferusa.sharepoint.com", "avepoint@kieferusa.onmicrosoft.com", "Av3P0int!!");
            using (var reader = new StreamReader(@"C:\Users\xluo\Desktop\Report\Failed.csv"))
            {
                int finishFileCount = 0;
                while (!reader.EndOfStream)
                {
                    var url = reader.ReadLine().Trim('"');
                    if (url.StartsWith(sourceHostName, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var fileUrl = new Uri(url.Replace(sourceHostName, DestinationHostName));
                            var file = context.Site.RootWeb.GetFileByServerRelativeUrl(fileUrl.AbsolutePath);
                            context.Load(file);
                            context.ExecuteQuery();
                            if (file.Exists)
                            {
                                ExistFiles.AppendLine(fileUrl.AbsoluteUri.ToString());
                            }
                            else
                            {
                                NoneFiles.AppendLine(fileUrl.AbsoluteUri.ToString());
                            }
                        }
                        catch (Exception e)
                        {
                            FailedFiles.AppendLine(url);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong url: {0}",url);
                        FailedFiles.AppendLine(url);
                    }
                    Console.WriteLine("Scan finish file count: {0}", ++finishFileCount);
                }
            }

            File.WriteAllText(@"C:\Users\xluo\Desktop\Report\ExistFiles.csv", ExistFiles.ToString());
            File.WriteAllText(@"C:\Users\xluo\Desktop\Report\NoneFiles.csv", NoneFiles.ToString());
            File.WriteAllText(@"C:\Users\xluo\Desktop\Report\Failed.csv", FailedFiles.ToString());
        }
    }
}
