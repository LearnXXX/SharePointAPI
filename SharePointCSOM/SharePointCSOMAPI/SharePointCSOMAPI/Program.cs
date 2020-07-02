using CommandLine;
using HtmlAgilityPack;
using log4net;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Newtonsoft.Json;
using SharePointCSOMAPI.Tools;
using SharePointCSOMAPI.Tools.PEQA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SharePointCSOMAPI
{
    class Program
    {
        private static ILog logger = LogManager.GetLogger(typeof(Program));
        //private static string siteUrl = "https://xluov-admin.sharepoint.com";
        //private static string siteUrl = "https://xluov-my.sharepoint.com/personal/xluo1_xluov_onmicrosoft_com";
        private static string siteUrl = "https://xluov-my.sharepoint.com/personal/xluo3_xluov_onmicrosoft_com";
        //private static string siteUrl = "https://xluov.sharepoint.com/sites/Test5";
        //private static string siteUrl = "https://xluov.sharepoint.com/sites/Janpanese";
        //private static string siteUrl = "https://xluov.sharepoint.com/sites/Test1/GermanLanguage/";

        private static TokenHelper tokenHelper = new TokenHelper();

        private static void ArgTest(int number)
        {
            Console.WriteLine("Number: {0}", number);
        }
        public class D
        {
            public DateTime LastRunTime { get; set; }
        }

        private static string GetColumnName(string encodestring)
        {
            var dta = XmlConvert.EncodeName("_UIVersion");
            var value = XmlConvert.DecodeName(encodestring);
            if (string.Equals(value, encodestring))
            {
                if (value.StartsWith("OData_"))
                {
                    return value.Substring("OData_".Length);
                }
                return value;
            }
            return GetColumnName(value);
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class TableAttribute : Attribute
        {
            public string TableName { get; set; }

            public TableAttribute(string tableName)
            {
                TableName = tableName;
            }
        }
        [Table("123")]
        public class TestC
        {

        }
        static void Main(string[] args)
        {
            FileLevel.GetFiles(tokenHelper.GetClientContextForServiceAccount(siteUrl,SPUsers.UserName, SPUsers.Password));
            WebLevel.CheckListExist(tokenHelper.GetClientContextForServiceAccount(siteUrl, SPUsers.UserName, SPUsers.Password),"");
            SiteLevel.GetSiteOwner(tokenHelper.GetClientContextForServiceAccount(siteUrl, SPUsers.UserName, SPUsers.Password));
            AddMultipleItems.Run(args);

            //var ca = TypeDescriptor.GetAttributes(typeof(TestC))
            // .OfType<TableAttribute>().FirstOrDefault();
            //Console.WriteLine(ca.TableName); // <=== nice
            //TypeDescriptor.AddAttributes(typeof(TestC), new TableAttribute("naughty"));
            //ca = TypeDescriptor.GetAttributes(typeof(TestC))
            //      .OfType<TableAttribute>().FirstOrDefault();
            //Console.WriteLine(ca.TableName); // <=== naughty

            //SiteTool.Run(args);

            //FileLevel.GetFiles(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //AnalysisIndexDBSize.Start(@"D:\Exchange Index\indexb4dbad7758dc79d0ba2032e43fc87f5c_d.db");
            //UserLevel.SiteUsers(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //ViewLevel.Test1(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //AnalysisIndexDBSize.Start(@"C:\Users\xluo\Desktop\indexf0a006eb76b59a36621c941ca77f72f5 - Copy.db");
            //ListLevel.LoadListProperty(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //SiteLevel.GetSiteUserAndGroups(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));


            //Workflow.Load13ModeWorklfow(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password)); 
            //FileLevel.Add1WFiles(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //ListItemLevel.LoadItemProperties(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //UserLevel.SiteUsers(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            ////ListLevel.LoadListProperty(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //var value = GetColumnName("OData__x005f_ModerationStatus");
            //Process proc = Process.GetCurrentProcess();
            //var meoryery = proc.PrivateMemorySize64;
            //WebRequest.DefaultWebProxy = new System.Net.WebProxy("127.0.0.1", 8888);
            //FileLevel.LoadFileProperties(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //ListItemLevel.LoadItemProperties(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            ////SiteLevel.GetSiteUserAndGroups(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            ////FileLevel.Add1WFiles(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            ////TenantLevel.Test(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //FileLevel.LoadFileProperties(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //UserLevel.SiteUsers(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //FolderLevel.CreateMultiFolders(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //var uri = new Uri("https://m365x157144-my.sharepoint.com/personal/admin_m365x157144_onmicrosoft_com");
            //SiteLevel.GetSiteOwner(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //WebLevel.CheckListExist(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password), string.Format("{0}/{1}", siteUrl, "SiteAssets"));
            //ViewLevel.CreateViewWithBaseViewId(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //ViewLevel.UpdateContentTypeId(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //OpenXmlTest.Test();
            //var datsdfe = DateTime.FromOADate(43794.2714930556);

            //WebLevel.GetLitByTitle(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));

            //var guid = Guid.NewGuid();

            //Navigation.NavigationTest(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //int number = int.MaxValue;
            //ArgTest(number -= 1);
            //try
            //{
            //    var date = new DateTime(1563418812947);
            //    Initalize();

            //    //FileLevel.Add1WFiles(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //    //ScanSubSiteDocumentLibrary.Scan(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //    //UpdateFileDateTimeColumnValue.Update(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //    if (args.Length == 0)
            //    {
            //        ScanSubSiteDocumentLibrary.Scan(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //        //Navigation.NavigationTest(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //        //FolderLevel.FolderTest(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //        //HtmlTest();
            //        //var text = System.IO.File.ReadAllText(@"C:\Users\xluo\Desktop\111.txt");
            //        //var result = System.Web.HttpUtility.HtmlDecode(text);

            //        //SiteLevel.GetSiteSize(tokenHelper.GetClientContextForAppToken(siteUrl));
            //        //SiteLevel.GetSiteSize(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //        //UserLevel.GetUserByLoginName(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //        //MetadataService.Test1(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //        //WebLevel.GetAllListsInWeb(tokenHelper.GetClientContextForServiceAccount(siteUrl, userName, password));
            //    }
            //    else
            //    {

            //        Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            //        {
            //            //UpdateFileDateTimeColumnValue.RunForLargeList(o);
            //            UpdateFileDateTimeColumnValue.Run(o);
            //            //UpdateFileDateTimeColumnValue.Update(tokenHelper.GetClientContextForServiceAccount(o.SiteUrl, o.UserName, o.Password));
            //            //ScanSubSiteDocumentLibrary.Scan(tokenHelper.GetClientContextForServiceAccount(o.SiteUrl, o.UserName, o.Password));
            //        });

            //    }
            //}
            //catch (Exception e)
            //{
            //    logger.ErrorFormat("An error occurred: {0}", e);
            //}
            //logger.Info("Press any key to esc...");
            //Console.ReadKey();
        }
        private static void HtmlTest()
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(@"C:\Users\xluo\Downloads\Exchange Mail Flow - 2.mht", Encoding.UTF8);
            var dss = "=3D";
            var sdrr = FromQuotedPrintable(System.IO.File.ReadAllText(@"C:\Users\xluo\Desktop\Test1.txt"));
            var ssdfsf = FromQuotedPrintable(System.IO.File.ReadAllText(@"C:\Users\xluo\Downloads\Destination.mht"));
            var dsfdf = DecodeQP("d=123123");
            FromQuotedPrintable(dss);
            var ccc = FromQuotedPrintable(System.IO.File.ReadAllText(@"C:\Users\xluo\Desktop\Test1.txt"));
            ccc = FromQuotedPrintable(System.IO.File.ReadAllText(@"C:\Users\xluo\Desktop\aaa.txt"));
            var content = FromQuotedPrintable(htmlDocument.ParsedText);
            System.IO.File.AppendAllText(@"C:\Users\xluo\Desktop\222.html", content);

        }

        private static string DecodeQuotedPrintables(string input, string charSet)
        {
            if (string.IsNullOrEmpty(charSet))
            {
                var charSetOccurences = new Regex(@"=\?.*\?Q\?", RegexOptions.IgnoreCase);
                var charSetMatches = charSetOccurences.Matches(input);
                foreach (Match match in charSetMatches)
                {
                    charSet = match.Groups[0].Value.Replace("=?", "").Replace("?Q?", "");
                    input = input.Replace(match.Groups[0].Value, "").Replace("?=", "");
                }
            }

            Encoding enc = new ASCIIEncoding();
            if (!string.IsNullOrEmpty(charSet))
            {
                try
                {
                    enc = Encoding.GetEncoding(charSet);
                }
                catch
                {
                    enc = new ASCIIEncoding();
                }
            }

            //decode iso-8859-[0-9]
            var occurences = new Regex(@"=[0-9A-Z]{2}", RegexOptions.Multiline);
            var matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                try
                {
                    byte[] b = new byte[] { byte.Parse(match.Groups[0].Value.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier) };
                    char[] hexChar = enc.GetChars(b);
                    input = input.Replace(match.Groups[0].Value, hexChar[0].ToString());
                }
                catch { }
            }

            //decode base64String (utf-8?B?)
            occurences = new Regex(@"\?utf-8\?B\?.*\?", RegexOptions.IgnoreCase);
            matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                byte[] b = Convert.FromBase64String(match.Groups[0].Value.Replace("?utf-8?B?", "").Replace("?UTF-8?B?", "").Replace("?", ""));
                string temp = Encoding.UTF8.GetString(b);
                input = input.Replace(match.Groups[0].Value, temp);
            }

            input = input.Replace("=\r\n", "");
            return input;
        }

        public static string DecodeQP(string codeString)
        {
            //编码的字符集
            string mailEncoding = "utf-8";

            StringBuilder strBud = new StringBuilder();
            for (int i = 0; i < codeString.Length; i++)
            {
                if (codeString[i] == '=')
                {
                    if (codeString[i + 1] == '\r' && codeString[i + 2] == '\n')
                    {
                        i += 2;
                        continue;
                    }

                    if (Convert.ToInt32((codeString[i + 1] + codeString[i + 2]).ToString(), 16) < 127)
                    {
                        strBud.Append(
                        Encoding.GetEncoding(mailEncoding).GetString(
                        new byte[] { Convert.ToByte((codeString[i + 1] + codeString[i + 2]).ToString(), 16) }));

                        i += 2;
                        continue;
                    }

                    if (codeString[i + 3] == '=')
                    {
                        strBud.Append(
                        Encoding.GetEncoding(mailEncoding).GetString(
                        new byte[] { Convert.ToByte((codeString[i + 1].ToString() + codeString[i + 2].ToString()), 16),
                 Convert.ToByte((codeString[i + 4].ToString() + codeString[i + 5].ToString()), 16) }));

                        i += 5;
                        continue;
                    }

                    strBud.Append(codeString[i]);
                }
                else
                {
                    strBud.Append(codeString[i]);
                }
            }
            return strBud.ToString();
        }

        public static string FromQuotedPrintable(string decode)
        {
            // Don't bother if there's nothing to decode
            if (decode == null || decode.Length == 0 || decode.IndexOf('=') == -1)
                return decode;
            try
            {

                var enc = Encoding.GetEncoding("utf-8");
                StringBuilder sb = new StringBuilder(decode.Length);

                string hexDigits = "0123456789ABCDEF";
                int pos1, pos2, pos3;

                for (int idx = 0; idx < decode.Length; idx++)
                {
                    Console.WriteLine("{0}", idx);
                    // Is it an encoded character?
                    if (decode[idx] == '=' && idx + 2 <= decode.Length)
                    {
                        // Ignore a soft line break
                        if (decode[idx + 1] == '\r' && decode[idx + 2] == '\n')
                        {
                            idx += 2;
                            continue;
                        }

                        ////repalce =C2=A0 with ' '
                        //if ((idx + 5 < decode.Length) && (decode[idx + 1] == 'C' && decode[idx + 2] == '2' && decode[idx + 3] == '=' && decode[idx + 4] == 'A' && decode[idx + 5] == '0'))
                        //{
                        //    sb.Append(' ');
                        //    idx += 5;
                        //    continue;
                        //}

                        pos1 = hexDigits.IndexOf(decode[idx + 1]);
                        pos2 = hexDigits.IndexOf(decode[idx + 2]);
                        pos3 = hexDigits.IndexOf(decode[idx + 3]);
                        var encodeData = new List<byte>();
                        GetString(decode, encodeData, ref idx);
                        if (encodeData.Count == 0)
                        {
                            sb.Append(decode[idx]);
                        }
                        else if (encodeData.Count == 1 && pos3 != -1)
                        {
                            sb.Append(decode[idx - 2]);
                            sb.Append(decode[idx - 1]);
                            sb.Append(decode[idx]);
                        }
                        else
                        {
                            sb.Append(enc.GetString(encodeData.ToArray()));
                        }

                        continue;
                        //=C2=A0
                        if (pos1 != -1 && pos2 != -1 && (idx + 5 < decode.Length && decode[idx + 3] == '=') && hexDigits.IndexOf(decode[idx + 4]) != -1 && hexDigits.IndexOf(decode[idx + 5]) != -1)
                        {
                            sb.Append(enc.GetString(
                         new byte[] { Convert.ToByte((decode[idx + 1].ToString() + decode[idx + 2].ToString()), 16),
                 Convert.ToByte((decode[idx + 4].ToString() + decode[idx + 5].ToString()), 16) }));
                            idx += 5;
                            continue;
                        }
                        else if (pos1 != -1 && pos2 != -1 && pos3 == -1)//avoid =sdfs
                        {
                            byte[] b = new byte[] { byte.Parse(decode[idx + 1].ToString() + decode[idx + 2].ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) };
                            char[] hexChar = enc.GetChars(b);
                            sb.Append(hexChar);
                            idx += 2;
                        }
                        else
                        {
                            sb.Append(decode[idx]);
                        }
                    }
                    else
                        sb.Append(decode[idx]);
                }

                return sb.ToString();
            }
            catch (Exception e)
            {
                //LoggerUtility.WriteMessage(Severity.Warning, "Encode QP error. String: {0}, Error: {1}", decode, e);
                return decode;
            }
        }
        private static bool GetString(string content, List<byte> encode, ref int index)
        {
            string hexDigits = "0123456789ABCDEF";
            if (index + 2 < content.Length)
            {
                var pos1 = hexDigits.IndexOf(content[index + 1]);
                var pos2 = hexDigits.IndexOf(content[index + 2]);
                if (pos1 != -1 && pos2 != -1)
                {
                    encode.Add(Convert.ToByte(content[index + 1].ToString() + content[index + 2].ToString(), 16));
                    if (index + 3 < content.Length && content[index + 3] == '=')
                    {
                        index += 3;
                        if (!GetString(content, encode, ref index)) //处理=是该行最后一个字符的情况
                        {
                            index--;
                        }
                    }
                    else
                    {
                        index += 2;
                    }
                    return true;
                }
            }
            return false;
        }
        private static void Initalize()
        {
            if (Configuration.Config.EnableProxy)
            {
                logger.InfoFormat("Use proxy with {0}", Configuration.Config.Proxy.Address);
                WebRequest.DefaultWebProxy = new System.Net.WebProxy(Configuration.Config.Proxy.Address) { Credentials = new NetworkCredential(Configuration.Config.Proxy.Username, Configuration.Config.Proxy.Password) };
            }
            else
            {
                logger.InfoFormat("Use system default proxy");
                WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();
            }
        }

    }
}
