using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class FolderLevel
    {
        public static void CreateMultiFolders(ClientContext context)
        {
            var list = context.Web.Lists.GetByTitle("Documents");
            context.Load(list);
            context.ExecuteQuery();
            for (var count = 0; count < 6000; count++)
            {
                list.RootFolder.AddSubFolder($"Folder_{count}", new ListItemUpdateParameters { });

                if (count % 50 == 0)
                {
                    Console.WriteLine($"Count: {count}");
                    context.ExecuteQuery();
                }
            }
            context.ExecuteQuery();
        }
        public static void FolderTest(ClientContext context)
        {
            var folder = context.Web.GetFolderByServerRelativeUrl("/sites/XluoTest1/Shared%20Documents/Folder2");
            context.Load(folder);
            context.ExecuteQuery();
        }
    }
}
