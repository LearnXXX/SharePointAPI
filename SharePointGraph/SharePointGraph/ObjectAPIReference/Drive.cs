using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    class Drive
    {
        public static bool IsBeta = false;
        public static void ListChildrenInRoot(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/root/children", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId);
            var info = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

        public static void Copy(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId, string folderId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/copy", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId);
            var requestBody = JsonConvert.SerializeObject(new { parentReference = new { driveId = driveId, id = folderId }, name = "copytest.txt" });
            var info = GraphApiCallHelper.PostApiJObject(token, webApiUrl, requestBody);
        }

        public static void Move(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId, string folderId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId);
            var requestBody = JsonConvert.SerializeObject(new { parentReference = new { id = folderId }, name = "movetest.txt" });
            var info = GraphApiCallHelper.PatchApiJObject(token, webApiUrl, requestBody);
        }


        public static void Delete(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId);

            GraphApiCallHelper.DeleteApi(token, webApiUrl);
        }



        /// <summary>
        /// 当前该方法不可用
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hostName"></param>
        /// <param name="siteCollectionId"></param>
        /// <param name="siteId"></param>
        /// <param name="driveId"></param>
        /// <param name="driveItemId"></param>
        public static void ListDriveItemChildrenByItemId(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, int driveItemId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/children", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, driveItemId);
            var info = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

        public static void GetDriveItemByPath(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string path)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/root:/{5}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, path);
            var info = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }


        public static void GetDriveItemById(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId);
            var info = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

        public static void GetDriveItemContentByPath(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string filePath)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/root:/{5}:/content", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, filePath);
            var info = GraphApiCallHelper.GetApiResponseContent(token, webApiUrl);
        }


        public static void UploadNewFile(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string parentFolderId, string fileName)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}:/{6}:/content", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, parentFolderId, fileName);
            GraphApiCallHelper.PutApi(token, webApiUrl, System.Text.Encoding.Default.GetBytes("Test"));
        }

        public static void ReplaceExistFile(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/content", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId);
            GraphApiCallHelper.PutApi(token, webApiUrl, System.Text.Encoding.Default.GetBytes("Test2"));
        }


        /// <summary>
        /// the maximum bytes in any given request is less than 60 MiB.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hostName"></param>
        /// <param name="siteCollectionId"></param>
        /// <param name="siteId"></param>
        /// <param name="driveId"></param>
        /// <param name="fileName"></param>
        public static void UploadNewLargeFile(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string fileName, string filePath)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/root:/{5}:/createUploadSession", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, fileName);

            dynamic result = GraphApiCallHelper.PostApiJObject(token, webApiUrl, "");
            var uploadUrl = result.uploadUrl.ToString();

            using (var stream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader br = new BinaryReader(stream))
            {
                var length = stream.Length;
                byte[] buffer = new byte[1024];
                //byte[] buffer = new byte[2 * 1024 * 1024];
                int bytesRead = 0;
                long totalBytesRead = 0;
                while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
                {
                    dynamic uploadResult = null;
                    totalBytesRead = totalBytesRead + bytesRead;
                    if (totalBytesRead == length)
                    {
                        // Copy to a new buffer that has the correct size
                        var lastBuffer = new byte[bytesRead];
                        Array.Copy(buffer, 0, lastBuffer, 0, bytesRead);
                        buffer = lastBuffer;

                    }
                    uploadResult = GraphApiCallHelper.PutApiUploadLargeFileJObject(token, uploadUrl, buffer, string.Format("bytes {0}-{1}/{2}", totalBytesRead - bytesRead, totalBytesRead - 1, length));
                }
            }
        }

        public static void ReplaceExistLargeFile(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId, string filePath)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/createUploadSession", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId);

            dynamic result = GraphApiCallHelper.PostApiJObject(token, webApiUrl, "");
            var uploadUrl = result.uploadUrl.ToString();

            using (var stream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader br = new BinaryReader(stream))
            {
                var length = stream.Length;
                byte[] buffer = new byte[1 * 1024 * 1024];
                int bytesRead = 0;
                long totalBytesRead = 0;
                while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
                {
                    dynamic uploadResult = null;
                    totalBytesRead = totalBytesRead + bytesRead;
                    if (totalBytesRead == length)
                    {
                        // Copy to a new buffer that has the correct size
                        var lastBuffer = new byte[bytesRead];
                        Array.Copy(buffer, 0, lastBuffer, 0, bytesRead);
                        buffer = lastBuffer;

                    }
                    uploadResult = GraphApiCallHelper.PutApiUploadLargeFileJObject(token, uploadUrl, buffer, string.Format("bytes {0}-{1}/{2}", totalBytesRead - bytesRead, totalBytesRead - 1, length));
                }
            }
        }

        public static void Search(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string searchQuery)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/root/search(q='{5}')", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, searchQuery);
            dynamic result = GraphApiCallHelper.GetApiJObject(token, webApiUrl);

        }

        public static void GetVersions(string token, string hostName, Guid siteCollectionId, Guid siteId, string driveId, string itemId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/drives/{4}/items/{5}/versions", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, driveId, itemId);
            dynamic result = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

    }
}
