using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    public static class SiteCollection
    {
        public static bool IsBeta = false;

        public static void GetSiteCollectionInfoByUrl(string token, string siteUrl)
        {
            var siteUri = new Uri(siteUrl);
            string webApiUrl = string.Format("{0}/sites/{1}:{2}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, siteUri.Host, siteUri.AbsolutePath);
            var siteInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hostName">longgod.sharepoint.com</param>
        /// <param name="siteId">dd328351-9c84-4c36-b391-6673d2ce9ace</param>
        public static void GetSiteCollectionInfoById(string token, string hostName, Guid siteId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteId);
            var siteInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

        /// <summary>
        /// Get tenant Root SiteCollection
        /// </summary>
        /// <param name="token"></param>
        public static void GetTenantRootSiteCollectionInfo(string token)
        {
            string apiUrl = string.Format("{0}/sites/root", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1);
            var siteInfo = GraphApiCallHelper.GetApiJObject(token, apiUrl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hostName">longgod.sharepoint.com</param>
        public static void GetTenantRootSiteCollectionInfoByHostName(string token, string hostName)
        {
            string apiUrl = string.Format("{0}/sites/{1}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName);
            var siteInfo = GraphApiCallHelper.GetApiJObject(token, apiUrl);
        }

        /// <summary>
        /// 暂时还不清楚search query对应的是什么属性
        /// </summary>
        /// <param name="token"></param>
        /// <param name="searchQuery">"jqzhao1"</param>
        public static void SearchSiteCollection(string token, string searchQuery)
        {
            string apiUrl = string.Format("{0}/sites/?$search={1}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, searchQuery);
            var siteInfo = GraphApiCallHelper.GetApiJObject(token, apiUrl);
        }

        /// <summary>
        /// 对应Site.webs
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hostName">longgod.sharepoint.com</param>
        /// <param name="siteId">dd328351-9c84-4c36-b391-6673d2ce9ace</param>
        public static void GetAllSitesUnderSiteCollection(string token, string hostname, Guid siteId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2}/sites", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostname, siteId);
            var sitesInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }
    }
}
