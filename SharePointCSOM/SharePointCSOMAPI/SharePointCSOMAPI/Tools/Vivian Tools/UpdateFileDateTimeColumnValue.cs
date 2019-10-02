using System;
using log4net;
using System.Linq;
using SharePointCSOMAPI.Utility;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;

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
                //if (listInfo.BigList)//暂时不处理 有这样的list再考虑处理
                //{
                //    logger.WarnFormat("the list item count exceed 5000, web Url: {0}, list title: {1}", listInfo.WebUrl, listInfo.ListTitle);
                //    throw new Exception(string.Format("the list item count exceed 5000, web Url: {0}, list title: {1}", listInfo.WebUrl, listInfo.ListTitle));
                //    //continue;
                //}

                if (context == null || !string.Equals(context.Url, listInfo.WebUrl, StringComparison.OrdinalIgnoreCase))
                {
                    logger.InfoFormat("Start process web: {0}", listInfo.WebUrl);
                    context = tokenHelper.GetClientContextForServiceAccount(listInfo.WebUrl, o.UserName, o.Password);
                    context.Load(context.Web, w => w.RegionalSettings.TimeZone);
                    context.ExecuteQuery();

                    if (context.Web.RegionalSettings.TimeZone.Id != 10)//(UTC-05:00) Eastern Time (US and Canada) 当前仅处理该时区的 如果存在其他时区有问题 再考虑其他时区
                    {
                        logger.ErrorFormat("Unexpected time zone for web, web url: {0}", listInfo.WebUrl);
                        throw new Exception(string.Format("Unexpected time zone for web, web url: {0}", listInfo.WebUrl));
                    }
                }

                var list = context.Web.Lists.GetById(listInfo.listId);
                context.Load(list.Fields);
                context.Load(list.RootFolder, f => f.ServerRelativeUrl);
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

                    List<FileInfo> changedfileInfos = null;
                    if (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), listInfo.listId.ToString())))
                    {
                        changedfileInfos = SerializerHelper.DeserializeObjectFromString<List<FileInfo>>(System.IO.File.ReadAllText(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), listInfo.listId.ToString())));
                        fileInfos.AddRange(changedfileInfos);
                    }

                    context.Load(list.RootFolder.Files, file => file.Include(f => f.ListItemAllFields, f => f.ServerRelativeUrl));
                    context.ExecuteQuery();


                    if (listInfo.BigList)
                    {
                        QueryLargeList(context, list, fileInfos, changedfileInfos, dateTimeFields);
                    }
                    else
                    {
                        UpdateFile(context, list.RootFolder, fileInfos, changedfileInfos, dateTimeFields);
                    }
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

        private static void QueryLargeList(ClientContext context, List list, List<FileInfo> fileInfos, List<FileInfo> ChangedfileInfos, List<string> dateTimeFields)
        {
            uint perPage = 5000;
            var queryArgs = new QueryArgs { FileInfos = fileInfos, ChangedfileInfos = ChangedfileInfos, DateTimeFields = dateTimeFields };
            var queryWorker = new LargeListQueryWorker(context, list, list.RootFolder.ServerRelativeUrl, perPage, null, null, queryArgs, QueryFindOption.RecursiveAll);

            queryWorker.BeforeQueryAction += LoadFileProperty;
            queryWorker.AfterQueryAction += UpdateFile;
            queryWorker.Run();

        }


        private static void LoadFileProperty(ClientContext context, ListItemCollection listItemsArg)
        {
            context.Load(listItemsArg, items => items.ListItemCollectionPosition,
                                      items => items.IncludeWithDefaultProperties(item => item["FSObjType"],
                                                                                  item => item.File.ServerRelativeUrl,
                                                                                  item => item.File.ListItemAllFields)
                                      .Where(item => (string)item["FSObjType"] == "0"));
        }


        private static void UpdateFile(ClientContext context, ListItem item, object args)
        {
            var updateArgs = args as QueryArgs;

            var file = item.File;
            if (file == null)
            {
                return;
            }

            UpdateFile(context, file, updateArgs.FileInfos, updateArgs.ChangedfileInfos, updateArgs.DateTimeFields);
        }

        private static void UpdateFile(ClientContext context, File file, List<FileInfo> fileInfos, List<FileInfo> changedfileInfos, List<string> dateTimeFields)
        {
            if (file.ListItemAllFields.FieldValues.Count == 0)
            {
                logger.InfoFormat("Skip file {0}, this file maybe a default file.", file.ServerRelativeUrl);
                return;
            }
            var tempFileInfo = new FileInfo { Url = file.ServerRelativeUrl };
            if (changedfileInfos != null && changedfileInfos.Contains(tempFileInfo))
            {
                return;
            }

            fileInfos.Add(tempFileInfo);
            logger.InfoFormat("start process {0}", file.ServerRelativeUrl);

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
        }

        private static void UpdateFile(ClientContext context, Folder folder, List<FileInfo> fileInfos, List<FileInfo> changedfileInfos, List<string> dateTimeFields)
        {
            context.Load(folder.Files, file => file.Include(f => f.ListItemAllFields, f => f.ServerRelativeUrl));
            context.ExecuteQuery();

            foreach (var file in folder.Files)
            {

                UpdateFile(context, file, fileInfos, changedfileInfos, dateTimeFields);
            }

            context.Load(folder.Folders);
            context.ExecuteQuery();

            foreach (var subFolder in folder.Folders)
            {
                UpdateFile(context, subFolder, fileInfos, changedfileInfos, dateTimeFields);
            }
        }
    }

    class QueryArgs
    {
        public List<FileInfo> FileInfos { get; set; }
        public List<FileInfo> ChangedfileInfos { get; set; }
        public List<string> DateTimeFields { get; set; }
    }
}
