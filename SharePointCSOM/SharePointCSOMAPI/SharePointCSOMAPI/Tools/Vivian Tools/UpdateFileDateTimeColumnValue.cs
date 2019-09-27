using log4net;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools
{
    class UpdateFileDateTimeColumnValue
    {
        private static ILog logger = LogManager.GetLogger(typeof(UpdateFileDateTimeColumnValue));

        public static void Run(Options o)
        {
            TokenHelper tokenHelper = new TokenHelper();
            var listInfos = SerializerHelper.DeserializeObjectFromString<List<ListInfo>>(System.IO.File.ReadAllText(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ScanResult.xml")));

            ClientContext context = null;
            foreach (var listInfo in listInfos)
            {
                List<FileInfo> fileInfos = new List<FileInfo>();
                logger.InfoFormat("Start process list, list title: {0}", listInfo.ListTitle);
                if (listInfo.BigList)//暂时不处理 有这样的list再考虑处理
                {
                    logger.WarnFormat("the list item count exceed 5000, web Url: {0}, list title: {1}", listInfo.WebUrl, listInfo.ListTitle);
                    continue;
                }

                if (context == null || !string.Equals(context.Url, listInfo.WebUrl, StringComparison.OrdinalIgnoreCase))
                {
                    logger.InfoFormat("Start process web: {0}", listInfo.WebUrl);
                    context = tokenHelper.GetClientContextForServiceAccount(listInfo.WebUrl, o.UserName, o.Password);
                    context.Load(context.Web, w => w.RegionalSettings.TimeZone);
                    context.ExecuteQuery();

                    if (context.Web.RegionalSettings.TimeZone.Id != 10)//(UTC-05:00) Eastern Time (US and Canada) 当前仅处理该时区的 如果存在其他时区有问题 再考虑其他时区
                    {

                        logger.ErrorFormat("Unexpected time zone for web, web url: {0}",listInfo.WebUrl);
                        throw new Exception(string.Format("Unexpected time zone for web, web url: {0}", listInfo.WebUrl));
                    }
                }
                var list = context.Web.Lists.GetById(listInfo.listId);
                context.Load(list.Fields);
                context.Load(list);
                context.ExecuteQuery();
                var dateTimeFields = new List<string>();
                foreach (var field in list.Fields)
                {
                    if (field.FieldTypeKind == FieldType.DateTime)
                    {
                        dateTimeFields.Add(field.InternalName);
                    }
                }

                try
                {
                    list.EnableMinorVersions = false;
                    list.EnableModeration = false;
                    list.EnableVersioning = false;
                    list.Update();
                    context.ExecuteQuery();
                    logger.InfoFormat("Turn off list version setting successfully, web url: {0}, list title: {1}", listInfo.WebUrl, listInfo.ListTitle);

                    List<FileInfo> ChangedfileInfos = null;
                    if (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), listInfo.listId.ToString())))
                    {
                        ChangedfileInfos = SerializerHelper.DeserializeObjectFromString<List<FileInfo>>(System.IO.File.ReadAllText(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), listInfo.listId.ToString())));
                        fileInfos.AddRange(ChangedfileInfos);
                    }

                    context.Load(list.RootFolder.Files, file => file.Include(f => f.ListItemAllFields, f => f.ServerRelativeUrl));
                    context.ExecuteQuery();

                    UpdateFile(context, list.RootFolder, fileInfos, ChangedfileInfos, dateTimeFields);

                }
                catch (Exception e)
                {

                    logger.ErrorFormat("An error occurred while update file date, error: {0}", e.ToString());
                    throw;
                }
                finally
                {


                    list.EnableVersioning = listInfo.EnableVersioning;
                    list.EnableMinorVersions = listInfo.EnableMinorVersions;
                    if (listInfo.MajorVersionLimit > 0)
                    {
                        list.MajorVersionLimit = listInfo.MajorVersionLimit;
                    }
                    if (listInfo.MajorWithMinorVersionsLimit > 0)
                    {
                        list.MajorWithMinorVersionsLimit = listInfo.MajorWithMinorVersionsLimit;
                    }
                    list.EnableModeration = listInfo.EnableModeration;
                    list.Update();
                    context.ExecuteQuery();
                    logger.InfoFormat("Revert list version setting successfully, web url: {0}, list title: {1}", listInfo.WebUrl, listInfo.ListTitle);
                    System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), listInfo.listId.ToString()), SerializerHelper.SerializeObjectToString(fileInfos));
                }
            }
        }
        private static void UpdateFile(ClientContext context, Folder folder, List<FileInfo> fileInfos, List<FileInfo> ChangedfileInfos, List<string> dateTimeFields)
        {
            context.Load(folder.Files, file => file.Include(f => f.ListItemAllFields, f => f.ServerRelativeUrl));
            context.ExecuteQuery();

            foreach (var file in folder.Files)
            {
                if (file.ListItemAllFields.FieldValues.Count == 0)
                {
                    logger.InfoFormat("Skip file {0}, this file maybe a default file.",file.ServerRelativeUrl);
                    continue;
                }
                var tempFileInfo = new FileInfo { Url = file.ServerRelativeUrl };
                if (ChangedfileInfos != null && ChangedfileInfos.Contains(tempFileInfo))
                {
                    continue;
                }

                fileInfos.Add(tempFileInfo);
                logger.InfoFormat("start process {0}", file.ServerRelativeUrl);

                foreach (var dateField in dateTimeFields)
                {
                    if (file.ListItemAllFields[dateField] != null)
                    {
                        var tempValue = (DateTime)file.ListItemAllFields[dateField];
                        //file.ListItemAllFields[dateField] = tempValue.AddHours(3);
                        file.ListItemAllFields[dateField] = tempValue.AddDays(-10);
                    }
                }
                file.ListItemAllFields["Modified_x0020_By"] = file.ListItemAllFields["Modified_x0020_By"];//keep modify by
                file.ListItemAllFields.Update();
                context.ExecuteQuery();
            }

            context.Load(folder.Folders);
            context.ExecuteQuery();

            foreach (var subFolder in folder.Folders)
            {
                UpdateFile(context, subFolder, fileInfos, ChangedfileInfos, dateTimeFields);
            }
        }

        public static void Update(ClientContext context)
        {
            var EnableMinorVersions = false;
            var EnableVersioning = false;
            var EnableModeration = false;
            var web = context.Site.OpenWeb("/departments/DGMO");
            var list = web.Lists.GetByTitle("COO_Weekly_Staff");
            context.Load(list.Fields);
            context.Load(list);
            context.ExecuteQuery();
            var dateTimeFields = new List<string>();
            foreach (var field in list.Fields)
            {
                if (field.FieldTypeKind == FieldType.DateTime)
                {
                    dateTimeFields.Add(field.InternalName);
                }
            }

            EnableMinorVersions = list.EnableMinorVersions;
            var majorVersionLimit = list.MajorVersionLimit;
            var majorWithMinorVersionsLimit = list.MajorWithMinorVersionsLimit;
            EnableVersioning = list.EnableVersioning;
            EnableModeration = list.EnableModeration;
            list.EnableMinorVersions = false;
            list.EnableModeration = false;
            list.EnableVersioning = false;
            list.Update();
            context.ExecuteQuery();

            context.Load(list.RootFolder.Files, file => file.Include(f => f.ListItemAllFields, f => f.Name));
            context.ExecuteQuery();
            foreach (var file in list.RootFolder.Files)
            {
                if (string.Equals(file.Name, "October 17 Agenda.pdf"))
                {
                    logger.InfoFormat("start process {0}", file.Name);

                    foreach (var dateField in dateTimeFields)
                    {
                        if (file.ListItemAllFields[dateField] != null)
                        {
                            var tempValue = (DateTime)file.ListItemAllFields[dateField];
                            file.ListItemAllFields[dateField] = tempValue.AddHours(3);
                        }
                    }
                    file.ListItemAllFields["Modified_x0020_By"] = file.ListItemAllFields["Modified_x0020_By"];//keep modify by
                    file.ListItemAllFields.Update();
                    context.ExecuteQuery();
                    list.EnableVersioning = EnableVersioning;
                    list.EnableMinorVersions = EnableMinorVersions;
                    if (majorVersionLimit > 0)
                    {
                        list.MajorVersionLimit = majorVersionLimit;
                    }
                    if (majorWithMinorVersionsLimit > 0)
                    {
                        list.MajorWithMinorVersionsLimit = majorWithMinorVersionsLimit;
                    }
                    list.EnableModeration = EnableModeration;
                    list.Update();
                    context.ExecuteQuery();
                }
            }

        }

    }
}
