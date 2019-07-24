using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    class Folder
    {
        public static bool IsBeta = false;

        public static void CreateSubFolder(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string parentDriveItemId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/children", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, parentDriveItemId);
            var requestBody = JsonConvert.SerializeObject(new { name = "subfolder", folder = new { } });
            var info = GraphApiCallHelper.PostApiJObject(token, webApiUrl, requestBody);
        }
    }
}
