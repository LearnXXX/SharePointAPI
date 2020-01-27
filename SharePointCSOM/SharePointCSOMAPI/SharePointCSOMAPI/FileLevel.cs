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
        public static void LoadFileProperties(ClientContext context)
        {
            var list = context.Site.RootWeb.Lists.GetByTitle("Documents");
            var item = list.GetItemById(2);
            context.Load(item);
            context.Load(item.RoleAssignments, r => r.Include(a => a.PrincipalId, async => async.RoleDefinitionBindings, a => a.Member));
            context.Load(context.Site.RootWeb.SiteUsers);
            context.ExecuteQuery();
        }

        public static void Add1WFiles(ClientContext context)
        {
            var list = context.Site.RootWeb.Lists.GetByTitle("6KFiles");
            //Context.ExecuteQuery();
            context.Load(list.RootFolder.Folders);
            context.ExecuteQuery();
            foreach (var folder in list.RootFolder.Folders)
            {
                AddFiles(context, folder, 2000);
            }
        }

        private static void AddFiles(ClientContext context, Folder folder, int count)
        {
            DateTime date = DateTime.Now;
            try
            {
                for (int index = 0; index <= count; index++)
                {
                    date = date.AddHours(1);
                    if (index % 100 == 0)
                    {
                        Console.WriteLine("index = {0},Date: {1}", index, date.Ticks);
                        context.ExecuteQuery();
                    }

                    var info = new FileCreationInformation { Content = System.Text.Encoding.Default.GetBytes("1"), Url = date.ToString("yyyyMMddHHmmss") + ".txt", };
                    var file = folder.Files.Add(info);
                    file.ListItemAllFields["UniqueNumber"] = Guid.NewGuid().ToString();
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
