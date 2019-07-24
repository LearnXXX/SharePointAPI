using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    public static class Site
    {
        public static bool IsBeta = false;

        public static void GetSiteInfoByUrl(string token, string siteUrl)
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
        /// <param name="siteCollectionId">dd328351-9c84-4c36-b391-6673d2ce9ace</param>
        public static void GetSiteInfoById(string token, string hostName, Guid siteCollectionId, Guid siteId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId);
            var siteInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

        /// <summary>
        /// 对应Site.webs
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hostName">longgod.sharepoint.com</param>
        /// <param name="siteId">dd328351-9c84-4c36-b391-6673d2ce9ace</param>
        public static void GetAllSitesUnderSite(string token, string hostname, Guid siteCollectionId, Guid siteId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/sites", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostname, siteCollectionId, siteId);
            var sitesInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

        /// <summary>
        /// 改方法当前不可用 返回的list 数量不足
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hostname"></param>
        /// <param name="siteCollectionId"></param>
        /// <param name="siteId"></param>
        public static void GetAllListsUnderSite(string token, string hostname, Guid siteCollectionId, Guid siteId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/lists", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostname, siteCollectionId, siteId);
            var sitesInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hostname">longgod.sharepoint.com</param>
        /// <param name="siteCollectionId">a4e0e820-b750-4b37-beee-fccdd463a3f2</param>
        /// <param name="siteId">0a09151c-3651-40b4-9d47-dd477e4b9dae</param>
        /// <param name="listId">bb3f653d-e66f-43fb-98db-db608d47ba78</param>
        public static void GetListUnderSite(string token, string hostname, Guid siteCollectionId, Guid siteId, Guid listId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/lists/{4}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostname, siteCollectionId, siteId, listId);
            var sitesInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

        public static void CreateListUnderSite(string token, string hostname, Guid siteCollectionId, Guid siteId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/lists", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostname, siteCollectionId, siteId);
            var requestBody = JsonConvert.SerializeObject(new { displayName = "CreateListTest", columns = new object[] { new { name = "test1", text = new { } }, new { name = "PageCount", number = new { } } }, list = new { template = "genericList" } });

            var sitesInfo = GraphApiCallHelper.PostApiJObject(token, webApiUrl, requestBody);
        }


        public static void GetAllDrivesUnderSite(string token, string hostname, Guid siteCollectionId, Guid siteId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/Drives", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostname, siteCollectionId, siteId);
            var drivesInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }
        /// <summary>
        /// DriveId当前仅知道可以通过GetAllDrives方法的返回值获取
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hostname"></param>
        /// <param name="siteCollectionId"></param>
        /// <param name="siteId"></param>
        /// <param name="driveId">b!IOjgpFC3N0u-7vzN1GOj8hwVCQpRNrRAnUfdR35Lna49ZT-7b-b7Q5jb22CNR7p4</param>
        public static void GetDriveByDriveId(string token, string hostname, Guid siteCollectionId, Guid siteId, string driveId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/Drives/{4}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostname, siteCollectionId, siteId, driveId);
            var driveInfo = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }
    }
}
