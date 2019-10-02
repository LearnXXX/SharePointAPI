using System;
using log4net;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;

namespace SharePointCSOMAPI.Utility
{
    public enum QueryFindOption
    {
        None,
        RecursiveAll
    }

    class LargeListQueryWorker
    {
        private static ILog logger = LogManager.GetLogger(typeof(LargeListQueryWorker));
        /// <summary>
        /// 在执行Query之前的操作，主要是Load属性
        /// BeforeQueryActionImp(ClientContext context,ListItemCollection items)
        /// </summary>
        public event Action<ClientContext, ListItemCollection> BeforeQueryAction;
        /// <summary>
        /// 在成功执行Query后的操作，主要是获取Item信息，组装数据对象
        /// AfterQueryActionImp(ClientContext context,ListItem item,bool isLibrary)
        /// </summary>
        //public event Action<ClientContext, ListItem, bool> AfterQueryAction;
        public event Action<ClientContext, ListItem, object> AfterQueryAction;
        private object afterQueryArgs;

        public Action ExceptionWhenQueryAction;
        private bool isLibrary;
        private string folderServerRelatedUrl;
        private uint perPage;
        private List<string> viewFields;
        private QueryFindOption findOption;

        private ClientContext context;
        private List list;
        private CamlQuery query;
        private int rowLimitCount;
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="list">list的ItemCount和BaseType属性必须初始化</param>
        /// <param name="folderServerRelatedUrl"></param>
        /// <param name="perPage"></param>
        /// <param name="getFolderMethod"></param>
        /// <param name="query">如果此参数不为null，则viewFields参数失效。</param>
        /// <param name="viewFields">如果查询所有column value，请保持此参数为null</param>
        public LargeListQueryWorker(ClientContext context, List list, string folderServerRelatedUrl, uint perPage, CamlQuery query, List<string> viewFields, object queryArgs, QueryFindOption findOption)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (list == null) throw new ArgumentNullException("list");
            if (perPage == 0) throw new ArgumentException("perPage must be great than 0.");
            if (string.IsNullOrEmpty(folderServerRelatedUrl)) throw new ArgumentNullException("folderServerRelatedUrl");

            this.context = context;
            this.list = list;
            this.folderServerRelatedUrl = folderServerRelatedUrl;
            this.perPage = perPage;
            this.isLibrary = list.BaseType == BaseType.DocumentLibrary;
            this.query = query;
            this.viewFields = viewFields;
            this.findOption = findOption;
            this.afterQueryArgs = queryArgs;
            rowLimitCount = GetRowLimited(query);
        }
        public ListItemCollectionPosition Position
        {
            get;
            private set;
        }
        private int GetRowLimited(CamlQuery query)
        {
            try
            {
                int rowLimitedCount = 0;
                if (query == null || string.IsNullOrEmpty(query.ViewXml))
                {
                    return rowLimitedCount;
                }
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(query.ViewXml);
                var node = xd.SelectSingleNode(".//*[name() = 'RowLimit']");
                if (node != null)
                {
                    int.TryParse(node.FirstChild.Value, out rowLimitCount);
                }
            }
            catch (Exception e)
            {
                logger.WarnFormat("Get query row limited value failed. xml: {0}, Error: {1}", query.ViewXml, e);
            }
            return rowLimitCount;
        }
        private void SetRowLimit(CamlQuery query, int rowLimit)
        {
            if (query == null || string.IsNullOrEmpty(query.ViewXml) || rowLimit < 1)
            {
                return;
            }
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(query.ViewXml);
                var node = xd.SelectSingleNode(".//*[name() = 'RowLimit']");
                if (node != null)
                {
                    node.FirstChild.Value = rowLimit.ToString();
                    query.ViewXml = xd.OuterXml;
                }
            }
            catch (Exception e)
            {
                logger.WarnFormat("Set query row limited value failed. xml: {0}, Error: {1}", query.ViewXml, e);
            }
        }
        public void Run()
        {
            if (this.BeforeQueryAction == null) throw new ArgumentNullException("BeforeQueryAction");
            if (this.AfterQueryAction == null) throw new ArgumentNullException("AfterQueryAction");
            try
            {
                InitQueryStringForList();
                if (this.isLibrary)
                {
                    try
                    {
                        //Query library 的效率最好，但是无法支持带条件的query，如果发现是带条件的query，那么就使用query list
                        if (this.query == null && findOption != QueryFindOption.RecursiveAll)
                        {
                            QueryItemsInLibrary();
                        }
                        else
                        {
                            QueryItemsUseIdIndex();
                        }
                    }
                    catch (Exception e)//如果Library里的Document个数超过5000,并且Disable folder。需要用ID方式查询。
                    {
                        logger.InfoFormat("An error occurred while query items in library. Error: {0}", e.ToString());
                        if (ExceptionWhenQueryAction != null)
                        {
                            ExceptionWhenQueryAction();
                        }
                        QueryItemsUseIdIndex();
                    }
                }
                else
                {
                    QueryItemsUseIdIndex();
                }
            }
            catch (Exception e)
            {
                logger.WarnFormat("An error occurred while query large list, query string: {0}, error: {1}", this.query == null ? string.Empty : this.query.ViewXml, e);
                throw;
            }
        }
        #region Init query string
        private XmlNode InitWhereQuery(XmlNode whereNode)
        {
            string queryString = whereNode == null ? string.Format(@"<Where><And><Geq><FieldRef Name='ID'/><Value Type='Integer'>{0}</Value></Geq><Lt><FieldRef Name='ID'/><Value Type='Integer'>{1}</Value></Lt></And></Where>", "{0}", "{1}")
                                                  : string.Format(@"<And><And><Geq><FieldRef Name='ID'/><Value Type='Integer'>{0}</Value></Geq><Lt><FieldRef Name='ID'/><Value Type='Integer'>{1}</Value></Lt></And>{2}</And>", "{0}", "{1}", whereNode.InnerXml);
            XmlDocument document = new XmlDocument();
            document.LoadXml(queryString);
            return document.DocumentElement;
        }
        private void InitQueryStringForList()
        {
            if (this.query == null)
            {
                return;
            }
            XmlDocument document = new XmlDocument();
            document.LoadXml(this.query.ViewXml);

            var whereNode = document.DocumentElement.SelectSingleNode("//Where");

            var importNode = document.ImportNode(InitWhereQuery(whereNode), true);
            if (whereNode == null)
            {
                var queryNode = document.DocumentElement.SelectSingleNode("//Query");
                if (queryNode != null)
                {
                    queryNode.AppendChild(importNode);
                }
            }
            else
            {
                whereNode.RemoveAll();
                whereNode.AppendChild(importNode);
            }

            this.query.ViewXml = document.OuterXml;
        }
        #endregion
        #region Query for List
        #region Mutil Thread for list
        //使用多线程可以显著提高执行效率, 必要时可以使用多线程来提升效率, 从测试结果看限制最大线程数=2~3即可。
        //测试数据结构如下:
        //Custom List(without additional columns)
        //  |-RootFolder(6000Items)
        //      |-SubFoler(7000Items)
        //查询RootFolder下Item记录效率测试结果如下:
        //Threads	1	    2	    3	    4	    5	    10	    Unlimited
        //TEST1	    00:15.2	00:10.7	00:11.3	00:13.2	00:12.2	00:12.2	00:11.6
        //TEST2 	00:11.4	00:08.1	00:08.4	00:09.4	00:07.9	00:08.8	00:08.1
        //TEST3 	00:12.9	00:07.8	00:08.2	00:09.3	00:08.6	00:08.9	00:08.9
        //TEST4	    00:11.7	00:07.9	00:08.8	00:10.8	00:08.0	00:08.5	00:08.2
        //TEST5	    00:11.4	00:08.1	00:08.3	00:08.0	00:08.1	00:08.0	00:09.0
        //TEST6	    00:17.2	00:08.0	00:08.2	00:08.0	00:07.7	00:08.6	00:09.2
        //TEST7	    00:11.6	00:08.0	00:08.5	00:09.8	00:08.4	00:10.2	00:08.9
        //TEST8	    00:13.1	00:08.8	00:09.8	00:08.2	00:08.2	00:08.8	00:09.8
        //TEST9	    00:11.6	00:08.2	00:08.8	00:08.1	00:09.9	00:08.7	00:08.6
        //TEST10	00:10.8	00:08.0	00:09.9	00:10.0	00:08.0	00:12.1	00:08.0
        //Average	00:12.7	00:08.3	00:09.0	00:09.5	00:08.7	00:09.5	00:09.0
        #endregion
        private void QueryItemsUseIdIndex()
        {
            int minId;
            int maxId;
            int queryCount = 0;
            GetListItemMinAndMaxId(out minId, out maxId);
            int startIndex = GetPageStartIndex(minId);//冗余增加可读性
            while (startIndex <= maxId)
            {
                logger.InfoFormat("Start index: {0}, max id: {1}", startIndex, maxId);
                var query = BuildCamlQueryById(startIndex, maxId);
                SetCamlQueryUrl(query, this.folderServerRelatedUrl);
                var listItems = this.list.GetItems(query);
                this.BeforeQueryAction(this.context, listItems);
                context.ExecuteQuery();
                foreach (ListItem item in listItems)
                {
                    this.AfterQueryAction(context, item, afterQueryArgs);
                    queryCount++;
                    if (rowLimitCount > 0 && queryCount >= rowLimitCount)
                    {
                        Position = listItems.ListItemCollectionPosition;
                        return;
                    }
                }
                SetRowLimit(this.query, rowLimitCount - queryCount);
                startIndex += (int)this.perPage;
                Position = listItems.ListItemCollectionPosition;
            }
        }

        private int GetPageStartIndex(int defaultValue)
        {
            if (this.query == null || this.query.ListItemCollectionPosition == null)
            {
                return defaultValue;
            }
            try
            {
                var pageInfo = this.query.ListItemCollectionPosition.PagingInfo;
                var arguements = pageInfo.Split('&');
                foreach (var arguement in arguements)
                {
                    if (string.IsNullOrEmpty(arguement))
                    {
                        continue;
                    }

                    if (arguement.StartsWith("p_ID", StringComparison.OrdinalIgnoreCase))
                    {
                        return int.Parse(arguement.Split('=')[1]);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Warn("Anerror occurred while get p_ID property from pageinfo, error: {0}", e);
            }
            return defaultValue;
        }

        private Folder GetFolder(Web web, string url)
        {
            var path = ResourcePath.FromDecodedUrl(url);
            return web.GetFolderByServerRelativePath(path);
        }
        private void SetCamlQueryUrl(CamlQuery camlquery, string folderUrl)
        {
            var filePath = ResourcePath.FromDecodedUrl(folderUrl);
            camlquery.FolderServerRelativePath = filePath;
        }

        private void GetListItemMinAndMaxId(out int minId, out int maxId)
        {
            var folder = GetFolder(this.list.ParentWeb, this.folderServerRelatedUrl);
            var query = new CamlQuery()
            {
                ViewXml = "<View Scope='RecursiveAll'><Query><OrderBy><FieldRef Ascending='FALSE' Name='ID' /></OrderBy></Query><RowLimit>1</RowLimit></View>",
            };
            SetCamlQueryUrl(query, list.RootFolder.ServerRelativeUrl);
            var items = this.list.GetItems(query);
            this.context.Load(folder, f => f.ListItemAllFields.Id);
            this.context.Load(items, itemsArg => itemsArg.Include(itemArg => itemArg.Id));
            this.context.ExecuteQuery();

            var isRootFolder = string.Equals(this.folderServerRelatedUrl, list.RootFolder.ServerRelativeUrl, StringComparison.OrdinalIgnoreCase);
            minId = isRootFolder ? 1 : folder.ListItemAllFields.Id;//Root folder无法获取ListItem对象
            if (items.Count <= 0) throw new InvalidDataException("Failed to get list item max id.");
            maxId = items[0].Id;
        }

        private CamlQuery BuildCamlQueryById(int startIndex, int maxId)
        {
            var endIndex = (startIndex + this.perPage) > maxId ? maxId + 1 : startIndex + this.perPage;
            if (this.query == null)
            {
                return new CamlQuery()
                {
                    // Id in [startInde, startIndex + perPage)
                    ViewXml = string.Format(
@"<View Scope='{0}'>
    {1}
    <Query>
        <OrderBy><FieldRef Name='ID'/></OrderBy>
            <Where>
                <And>
                    <Geq><FieldRef Name='ID'/><Value Type='Integer'>{2}</Value></Geq>
                    <Lt><FieldRef Name='ID'/><Value Type='Integer'>{3}</Value></Lt>
                </And>    
            </Where>
    </Query>
</View>", findOption == QueryFindOption.None ? string.Empty : findOption.ToString(), GenerateViewFieldsString(), startIndex, endIndex),
                };
            }
            else
            {
                return new CamlQuery
                {
                    ViewXml = string.Format(this.query.ViewXml, startIndex, endIndex),
                    ListItemCollectionPosition = this.query.ListItemCollectionPosition,
                };
            }
        }
        #endregion

        #region Query for Library
        private void QueryItemsInLibrary()
        {
            var query = BuildCamlQueryByLeafName();
            do
            {
                SetCamlQueryUrl(query, this.folderServerRelatedUrl);

                query.ListItemCollectionPosition = Position;

                var listItems = this.list.GetItems(query);
                this.BeforeQueryAction(this.context, listItems);
                context.ExecuteQuery();
                foreach (ListItem item in listItems)
                {
                    this.AfterQueryAction(context, item, isLibrary);
                }
                Position = listItems.ListItemCollectionPosition;
            }
            while (Position != null);
        }

        private CamlQuery BuildCamlQueryByLeafName()
        {
            return new CamlQuery()
            {
                ViewXml = string.Format(
@"<View Scope='{0}'>
        {1}
        <Query>
            <OrderBy>
              <FieldRef Name='FileLeafRef'/>
            </OrderBy>
        </Query>
         <RowLimit>{2}</RowLimit>
</View>", findOption == QueryFindOption.None ? string.Empty : findOption.ToString(), GenerateViewFieldsString(), this.perPage)
            };
        }
        #endregion
        private string GenerateViewFieldsString()
        {
            var viewFieldsString = new StringBuilder();
            if (viewFields != null && viewFields.Count > 0)
            {
                viewFieldsString.AppendLine("<ViewFields>");
                foreach (var field in viewFields)
                {
                    viewFieldsString.AppendLine(string.Format("<FieldRef Name='{0}'/>", field));
                }
                viewFieldsString.AppendLine("</ViewFields>");
            }
            return viewFieldsString.ToString();
        }
    }
}
