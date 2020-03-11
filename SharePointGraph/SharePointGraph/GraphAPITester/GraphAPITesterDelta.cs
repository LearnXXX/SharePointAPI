using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    public partial class GraphAPITester
    {
        /// <summary>
        /// Delta 每页最多1000
        /// </summary>
        public List<DriveItem> DeltaTest(string driveId)
        {
            //var deltaData = graphServiceClient.Sites["xluov.sharepoint.com,2a3dda8f-3fc1-4d2d-a7fc-a10ea44aa310,144a5a37-0b91-4754-8459-f2b4a01c93d4"].Drive.Root.Delta().Request().Select("sharepointids,id,lastModifiedDateTime,name,webUrl,folder,createdDateTime").Top(1000).GetAsync().Result;
            var deltaData = graphServiceClient.Drives[driveId].Root.Delta().Request().Select("sharepointids,id,lastModifiedDateTime,name,webUrl,folder,createdDateTime").Top(1000).GetAsync().Result;
            //var deltaData = graphServiceClient.Sites["m365x157144-my.sharepoint.com,47b37a14-09dd-407f-9509-6fa9b4ad20d4,08d6db6f-5165-4fd4-ad22-7a61201f766a"].Drive.Root.Delta().Request().GetAsync().Result;
            return GraphCommonUtility.GetRequestAllOfDatas<DriveItem>(deltaData);
        }
    }
}
