using CommandLine;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools.PEQA
{
    public class SiteToolOptions
    {

        [Option('c', "Count", Required = true, HelpText = "Input count for create site collection")]
        public int Count { get; set; }

        [Option('u', "Url", Required = true, HelpText = "Input url for create site collection")]
        public string Url { get; set; }

        [Option('a', "AdminUrl", Required = true, HelpText = "Input adimin site url for create site collection")]
        public string AdminUrl { get; set; }

        [Option('n', "Username", Required = true, HelpText = "Input user name for create site collection")]
        public string UserName { get; set; }

        [Option('p', "Password", Required = true, HelpText = "Input password for create site collection")]
        public string Password { get; set; }

        [Option('t', "Template", Required = true, HelpText = "Input Template for create site collection example: STS#0")]
        public string Template { get; set; }

        [Option('s', "Section", Required = false)]
        public int Section { get; set; }


    }
    public static class OptionsExtention
    {
        private static ILog logger = LogManager.GetLogger(typeof(SiteToolOptions));

        public static void CheckArguement(this SiteToolOptions option)
        {
            option.CheckAdminUrlValue();
            option.CheckCountValue();
            option.CheckPasswordValue();
            option.CheckUrlValue();
            option.CheckUserNameValue();
            option.CheckTemplateValue();
            option.CheckPageCountValue();

        }
        private static void CheckPageCountValue(this SiteToolOptions option)
        {
            if (option.Section == 0)
            {
                option.Section = 10;
            }
        }

        private static void CheckCountValue(this SiteToolOptions option)
        {
            if (option.Count == 0)
            {
                logger.Info("please input create group count:");
                option.Count = int.Parse(Console.ReadLine());
            }
        }
        private static void CheckUrlValue(this SiteToolOptions option)
        {
            if (string.IsNullOrEmpty(option.Url))
            {
                logger.Info("please input site url:");
                option.Url = Console.ReadLine();
            }
        }
        private static void CheckAdminUrlValue(this SiteToolOptions option)
        {
            if (string.IsNullOrEmpty(option.AdminUrl))
            {
                logger.Info("please input admin site url:");
                option.AdminUrl = Console.ReadLine();
            }
        }
        private static void CheckUserNameValue(this SiteToolOptions option)
        {
            if (string.IsNullOrEmpty(option.UserName))
            {
                logger.Info("please input user name:");
                option.UserName = Console.ReadLine();
            }
        }
        private static void CheckPasswordValue(this SiteToolOptions option)
        {
            if (string.IsNullOrEmpty(option.Password))
            {
                logger.Info("please input user password :");
                option.Password = Console.ReadLine();
            }
        }
        private static void CheckTemplateValue(this SiteToolOptions option)
        {
            if (string.IsNullOrEmpty(option.Template))
            {
                logger.Info("please input site collection template:");
                option.Template = Console.ReadLine();
            }
        }
    }
}
