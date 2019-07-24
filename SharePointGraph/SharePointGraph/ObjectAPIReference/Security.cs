using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    class Security
    {
        public static bool IsBeta = false;
        public static void GetDriveItemPermissionById(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/permissions", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId);
            dynamic result = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }


        public static void GetDriveItemPermissionByPath(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemPath)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/root:/{5}:/permissions", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemPath);
            dynamic result = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

        public static void InviteUserToFile(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId, string userEmail)
        {

            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/invite", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId);

            var requestBody = JsonConvert.SerializeObject(new { recipients = new object[] { new { email = userEmail } }, message = "graph test", requireSignIn = true, sendInvitation = true, roles = new string[] { "write" } });
            var result = GraphApiCallHelper.PostApiJObject(token, webApiUrl, requestBody);
        }

        public static void GetPermissionById(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId, string permissionId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/permissions/{6}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId, permissionId);
            dynamic result = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }
        public static void UpdatePermissionById(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId, string permissionId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/permissions/{6}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId, permissionId);
            var requestBody = JsonConvert.SerializeObject(new { roles = new string[] { "read" } });
            dynamic result = GraphApiCallHelper.PatchApiJObject(token, webApiUrl, requestBody);
        }

        public static void DeletePermissionById(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId, string permissionId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/permissions/{6}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId, permissionId);
            GraphApiCallHelper.DeleteApi(token, webApiUrl);
        }
    }
}
