using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class FileLevel
    {
        public static void GetFiles(ClientContext context)
        {
            var file = context.Site.RootWeb.GetFileByServerRelativeUrl(@"/personal/xluo3_xluov_onmicrosoft_com/Documents/New Text Document.txt");
            context.Load(file);
            context.Load(file.ListItemAllFields);
            context.ExecuteQuery();

        }
        public static void LoadFileProperties(ClientContext context)
        {
            var list = context.Site.RootWeb.Lists.GetByTitle("DeltaTest");
            //var item = list.GetItemByUniqueId(new Guid("4acbb960-da5d-4e09-95ba-1106a157e0a4"));
            var item = list.GetItemById(7);
            context.Load(item);
            context.ExecuteQuery();
            context.Load(item.File);
            context.Load(item.File.Properties);
            context.ExecuteQuery();
            context.Load(item.RoleAssignments, r => r.Include(a => a.PrincipalId, async => async.RoleDefinitionBindings, a => a.Member));
            context.Load(context.Site.RootWeb.SiteUsers);
            context.ExecuteQuery();
        }

        public static void Add1WFiles(ClientContext context)
        {
            var list = context.Site.RootWeb.Lists.GetByTitle("Documents");
            context.Load(list);
            context.ExecuteQuery();
            for (int index = 1; index <= 5; index++)
            {
                var folder1 = list.RootFolder.Folders.Add($"Folder{index}");
                folder1.Update();
                if (index % 100 == 0)
                {
                    context.ExecuteQuery();
                }
            }
            context.Load(list.RootFolder.Folders);
            context.ExecuteQuery();
            DateTime date = DateTime.Now;
            foreach (var folder in list.RootFolder.Folders)
            {

                if (folder.ItemCount == 0&& folder.Name!= "Forms")
                {
                    AddFiles(context, folder, 2000);
                }
                
            }
            context.ExecuteQuery();
        }

        private static void AddFiles(ClientContext context, Folder folder, int count)
        {
            DateTime date = DateTime.Now;
            try
            {
                for (int index = 0; index < count; index++)
                {
                    date = date.AddHours(1);
                    if (index % 100 == 0)
                    {
                        Console.WriteLine("index = {0},Date: {1}", index, date.Ticks);
                        context.ExecuteQuery();
                    }

                    var info = new FileCreationInformation { Content = System.Text.Encoding.Default.GetBytes("1"), Url = date.ToString("yyyyMMddHHmmss") + ".txt", };
                    var file = folder.Files.Add(info);
                    //file.ListItemAllFields["UniqueNumber"] = Guid.NewGuid().ToString();
                    file.ListItemAllFields.Update();

                }
                context.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
