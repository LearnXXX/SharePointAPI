using CommandLine;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools.PEQA
{
    public class AddMultipleItemsOptions
    {
        [Option('c', "Count", Required = true, HelpText = "Input count for create items")]
        public int Count { get; set; }

        //[Option('u', "Url", Required = true, HelpText = "Input site url")]
        //public string Url { get; set; }

        [Option('n', "Username", Required = true, HelpText = "Input user name for create site collection")]
        public string UserName { get; set; }

        [Option('p', "Password", Required = true, HelpText = "Input password for create site collection")]
        public string Password { get; set; }

        //[Option('t', "Title", Required = true, HelpText = "Input List Title")]
        //public string Title { get; set; }

        [Option('a', "AdminUrl", Required = true, HelpText = "Input adimin site url for search site collection")]
        public string AdminUrl { get; set; }

        [Option('k', "KeyWord", Required = true, HelpText = "Input key word to search site collection")]
        public string KeyWord { get; set; }

        [Option('l', "LimiteCount", Required = false, HelpText = "Input limite count to limite the count of site collection")]
        public int LimiteCount { get; set; }

    }
    public static class AddMultipleItemsOptionsExtention
    {
        private static ILog logger = LogManager.GetLogger(typeof(AddMultipleItemsOptions));

        public static void CheckArguement(this AddMultipleItemsOptions option)
        {
            option.CheckCountValue();
            option.CheckPasswordValue();
            //option.CheckUrlValue();
            option.CheckUserNameValue();
            //option.CheckTitleValue();

        }

        private static void CheckCountValue(this AddMultipleItemsOptions option)
        {
            if (option.Count == 0)
            {
                logger.Info("please input create item count:");
                option.Count = int.Parse(Console.ReadLine());
            }
        }
        //private static void CheckUrlValue(this AddMultipleItemsOptions option)
        //{
        //    if (string.IsNullOrEmpty(option.Url))
        //    {
        //        logger.Info("please input site url:");
        //        option.Url = Console.ReadLine();
        //    }
        //}
        private static void CheckUserNameValue(this AddMultipleItemsOptions option)
        {
            if (string.IsNullOrEmpty(option.UserName))
            {
                logger.Info("please input user name:");
                option.UserName = Console.ReadLine();
            }
        }
        private static void CheckPasswordValue(this AddMultipleItemsOptions option)
        {
            if (string.IsNullOrEmpty(option.Password))
            {
                logger.Info("please input user password :");
                option.Password = Console.ReadLine();
            }
        }
        //private static void CheckTitleValue(this AddMultipleItemsOptions option)
        //{
        //    if (string.IsNullOrEmpty(option.Title))
        //    {
        //        logger.Info("please input list title:");
        //        option.Title = Console.ReadLine();
        //    }
        //}
    }
}
