using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    class List
    {
        public static bool IsBeta = false;

        public static void DeleteItem(string token, string hostName, Guid siteCollectionId, Guid siteId, Guid listId, int itemId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/lists/{4}/items/{5}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, listId, itemId);
            GraphApiCallHelper.DeleteApi(token, webApiUrl);
        }

        public static void UpdateListItemFieldValues(string token, string hostName, Guid siteCollectionId, Guid siteId, Guid listId, int itemId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/lists/{4}/items/{5}/fields", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, listId, itemId);
            var requestBody = JsonConvert.SerializeObject(new { Title = "UpdateTest" });
            var result = GraphApiCallHelper.PatchApiJObject(token, webApiUrl, requestBody);

        }

        public static void GetListItem(string token, string hostName, Guid siteCollectionId, Guid siteId, Guid listId, int itemId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/lists/{4}/items/{5}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, listId, itemId);
            var result = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }
        /// <summary>
        /// query 指定 field value
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hostName"></param>
        /// <param name="siteCollectionId"></param>
        /// <param name="siteId"></param>
        /// <param name="listId"></param>
        /// <param name="itemId"></param>
        /// <param name="fields"></param>
        public static void GetListItemSpecifiedFields(string token, string hostName, Guid siteCollectionId, Guid siteId, Guid listId, int itemId, params string[] fields)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/lists/{4}/items/{5}", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, listId, itemId);
            webApiUrl = string.Format("{0}{1}", webApiUrl, CreateQueryParamFormQueryListItemField(fields));
            var result = GraphApiCallHelper.GetApiJObject(token, webApiUrl);
        }

        private static string CreateQueryParamFormQueryListItemField(params string[] fields)
        {
            string queryParam = "?expand=fields(select=";
            fields.ToList().ForEach(item => { queryParam = queryParam + item + ","; });

            return queryParam.TrimEnd(',') + ")";
        }


        public static void CreateListItem(string token, string hostName, Guid siteCollectionId, Guid siteId, Guid listId)
        {
            string webApiUrl = string.Format("{0}/sites/{1},{2},{3}/lists/{4}/items", IsBeta ? GraphAPIVersion.BETA : GraphAPIVersion.V1, hostName, siteCollectionId, siteId, listId);
            var requestBody = JsonConvert.SerializeObject(new { fields = new { Title = "CreateTest" } });
            GraphApiCallHelper.PostApiJObject(token, webApiUrl, requestBody);
        }

    }
}
