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
        public static void FolderTest(ClientContext context)
        {
            var folder = context.Web.GetFolderByServerRelativeUrl("/sites/XluoTest1/Shared%20Documents/Folder2");
            context.Load(folder);
            context.ExecuteQuery();
        }
    }
}
