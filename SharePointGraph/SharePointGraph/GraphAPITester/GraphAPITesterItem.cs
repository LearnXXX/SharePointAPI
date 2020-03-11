using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    partial class GraphAPITester
    {

        public List<DriveItem> GetAllItemsUnderFolder(string driveId, string folderId)
        {
            List<DriveItem> allOfFolders = new List<DriveItem>();
            var currentPage = graphServiceClient.Drives[driveId].Items[folderId].Children.Request().Select("sharepointids,id,lastModifiedDateTime,name,webUrl,folder,createdDateTime").Top(int.MaxValue).GetAsync().Result;
            var subItems = GraphCommonUtility.GetRequestAllOfDatas<DriveItem>(currentPage).ToList();

            foreach (var child in subItems)
            {
                if (child.Folder != null && child.Folder.ChildCount > 0)
                {
                    allOfFolders.AddRange(GetAllItemsUnderFolder(driveId, child.Id));
                }
            }
            allOfFolders.AddRange(subItems);
            return allOfFolders;
        }

        public List<DriveItem> GetAllSubFolders(string driveId, string folderId)
        {
            List<DriveItem> allOfFolders = new List<DriveItem>();
            var currentPage = graphServiceClient.Drives[driveId].Items[folderId].Children.Request().Select("sharepointids,id,lastModifiedDateTime,name,webUrl,folder,createdDateTime").GetAsync().Result;
            var subItems = GraphCommonUtility.GetRequestAllOfDatas<DriveItem>(currentPage);
            var subFolders = subItems.Where(item => item.Folder != null).ToList();

            foreach (var child in subFolders)
            {
                if (child.Folder.ChildCount > 0)
                {
                    allOfFolders.AddRange(GetAllSubFolders(driveId, child.Id));
                }
            }
            allOfFolders.AddRange(subFolders);
            return allOfFolders;
        }


        public IEnumerable<DriveItem> GetDriveFiles(string siteId, string listId, string parentFolderUrl)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            IDriveItemChildrenCollectionPage currentPage = null;
            const string SelectValue = "SharePointIds,File";
            if (string.IsNullOrEmpty(parentFolderUrl))
            {
                currentPage = graphServiceClient.Sites[siteId].Lists[listId].Drive.Root.Children.Request().Select(SelectValue).GetAsync().Result;
            }
            else
            {
                currentPage = graphServiceClient.Sites[siteId].Lists[listId].Drive.Root.ItemWithPath(parentFolderUrl).Children.Request().Select(SelectValue).GetAsync().Result;
            }

            var items = GraphCommonUtility.GetRequestAllOfDatas<DriveItem>(currentPage).Where(item => item.File != null);
            watch.Stop();
            Console.WriteLine($"GetDriveFiles:{watch.Elapsed.TotalSeconds}");
            return items;
        }

    }
}
