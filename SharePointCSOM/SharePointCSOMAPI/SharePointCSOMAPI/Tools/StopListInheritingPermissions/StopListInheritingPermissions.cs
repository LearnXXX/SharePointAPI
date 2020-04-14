using CommandLine;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class StopListInheritingPermissions
    {
        private static ILog logger = LogManager.GetLogger(typeof(StopListInheritingPermissions));

        private static TokenHelper tokenHelper = new TokenHelper();

        public static void Run(string[] args)
        {
            StopListInheritingPermissionsOption option = null;
            Parser.Default.ParseArguments<StopListInheritingPermissionsOption>(args).WithParsed<StopListInheritingPermissionsOption>(o =>
            {
                option = o;
            });

            using (var reader = new System.IO.StreamReader(option.SiteUrlFile))
            {
                while (!reader.EndOfStream)
                {
                    try
                    {
                        var siteUrl = reader.ReadLine();
                        var context = tokenHelper.GetClientContextForServiceAccount(siteUrl, option.UserName, option.Password);
                        context.Load(context.Site.RootWeb.Lists);
                        context.ExecuteQuery();
                        foreach (var list in context.Site.RootWeb.Lists)
                        {
                            if (list.Title.IndexOf(option.KeyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                logger.Info($"Start stop {list.Title} inheriting permissions");
                                list.BreakRoleInheritance(true, false);
                            }
                        }
                        context.ExecuteQuery();
                        logger.Info($"stop {siteUrl} lists inheriting permissions over.");
                    }
                    catch (Exception e)
                    {
                        logger.Warn($"An error occurred while running jobs, error: {e.ToString()}");
                    }
                }
            }
        }
    }
}
