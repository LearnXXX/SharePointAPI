namespace SharePointCSOMAPI
{
    using Microsoft.SharePoint.Client;
    using OfficeDevPnP.Core;
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    class ViewLevel
    {
        public static void CreateViewWithBaseViewId(ClientContext context)
        {

            //var file = context.Web.GetFileByUrl("/sites/XLuoTest1/Lists/DDD/Threaded.aspx");
            //var manager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);
            //context.Load(manager.WebParts);
            //context.ExecuteQuery();
            //foreach (var webpart in manager.WebParts)
            //{
            //    context.Load(webpart.WebPart);
            //    context.ExecuteQuery();
            //}
            var list = context.Web.Lists.GetByTitle("DD");
            context.Load(list.Views);
            context.ExecuteQuery();
            foreach (var view in list.Views)
            {
                Console.WriteLine($"{view.Title}, {view.BaseViewId}");
                //view.DefaultView = true;
                //view.Update();
                //context.ExecuteQuery();
                //var view00 = list.Views.Add(new ViewCreationInformation { baseViewId = 1, Title = "XLUO110" });
                //context.ExecuteQuery();

            }
            var view3 = list.Views.GetByTitle("Flat");
            var view4 = list.Views.GetByTitle("XLUO11");
            context.Load(view3);
            context.Load(view4);
            context.ExecuteQuery();
            //view4.ListViewXml = view4.ListViewXml.Replace("BaseViewID=\"3\"", "BaseViewID=\"5\"");
            //view4.Update();
            //context.ExecuteQuery();

            //var view5 = list.Views.GetByTitle("XLUO11");
            //context.Load(view5);
            //context.ExecuteQuery();

            //var view8 = list.Views.Add(new ViewCreationInformation { Title = "XLUO999" });
            //context.ExecuteQuery();


            context.Load(context.Web);
            var tempList = context.Web.Lists.GetByTitle("DD");
            context.Load(tempList);
            context.ExecuteQuery();
            CreateView(context.Web, view3, tempList.Views, tempList);
        }

        private static void CreateView(Web web, View view, Microsoft.SharePoint.Client.ViewCollection existingViews, List createdList)
        {
            try
            {
                //have to maintain raw displayNameElement for displayNameElement.Value.ContainsResourceToken() at Line 717
                var viewElementRaw = XElement.Parse(view.ListViewXml);
                //var displayNameElement = viewElementRaw.Attribute("DisplayName");
                var displayNameElement = "XluoTest";
                if (displayNameElement == null)
                {
                    throw new ApplicationException("Invalid View element, missing a valid value for the attribute DisplayName.");
                }

                //for all other functions need Parsed SchemaXML
                var viewElement = XElement.Parse(view.ListViewXml);

                //WriteSubProgress($"Views for list {createdList.Title}", displayNameElement.Value, currentViewIndex, total);
                //monitoredScope.LogDebug(CoreResources.Provisioning_ObjectHandlers_ListInstances_Creating_view__0_, displayNameElement.Value);

                //get from resource file
                var viewTitle = displayNameElement;
                //var existingView = existingViews.FirstOrDefault(v => v.Title == viewTitle);
                //if (existingView != null)
                //{
                //    existingView.DeleteObject();
                //    web.Context.ExecuteQueryRetry();
                //}

                // Type
                var viewTypeString = viewElement.Attribute("Type") != null ? viewElement.Attribute("Type").Value : "None";
                viewTypeString = viewTypeString[0].ToString().ToUpper() + viewTypeString.Substring(1).ToLower();
                var viewType = (ViewType)Enum.Parse(typeof(ViewType), viewTypeString);

                // Fix the calendar recurrence
                if (viewType == ViewType.Calendar)
                {
                    viewType = ViewType.Calendar | ViewType.Recurrence;
                }

                // Fields
                string[] viewFields = null;
                var viewFieldsElement = viewElement.Descendants("ViewFields").FirstOrDefault();
                if (viewFieldsElement != null)
                {
                    viewFields = (from field in viewElement.Descendants("ViewFields").Descendants("FieldRef") select field.Attribute("Name").Value).ToArray();
                }

                // Default view
                var viewDefault = viewElement.Attribute("DefaultView") != null && bool.Parse(viewElement.Attribute("DefaultView").Value);

                // Hidden view
                var hidden = viewElement.Attribute("Hidden") != null && bool.Parse(viewElement.Attribute("Hidden").Value);

                // Row limit
                var viewPaged = true;
                uint viewRowLimit = 30;
                var rowLimitElement = viewElement.Descendants("RowLimit").FirstOrDefault();
                if (rowLimitElement != null)
                {
                    if (rowLimitElement.Attribute("Paged") != null)
                    {
                        viewPaged = bool.Parse(rowLimitElement.Attribute("Paged").Value);
                    }
                    viewRowLimit = uint.Parse(rowLimitElement.Value);
                }

#if !SP2013 && !SP2016
                //BaseViewID
                int BaseViewID = 1;
                var baseviewIDElement = viewElementRaw.Attribute("BaseViewID");
                if (baseviewIDElement != null)
                {
                    BaseViewID = int.Parse(baseviewIDElement.Value);
                }
#endif
                // Query
                var viewQuery = new StringBuilder();
                foreach (var queryElement in viewElement.Descendants("Query").Elements())
                {
                    viewQuery.Append(queryElement.ToString());
                }

                var viewCI = new ViewCreationInformation
                {
#if !SP2013 && !SP2016
                    baseViewId = BaseViewID,
#endif
                    ViewFields = viewFields,
                    RowLimit = viewRowLimit,
                    Paged = viewPaged,
                    Title = viewTitle,
                    Query = viewQuery.ToString(),
                    ViewTypeKind = viewType,
                    PersonalView = false,
                    SetAsDefaultView = viewDefault
                };

                // Allow to specify a custom view url. View url is taken from title, so we first set title to the view url value we need,
                // create the view and then set title back to the original value
                var urlAttribute = viewElement.Attribute("Url");
                var urlHasValue = urlAttribute != null && !string.IsNullOrEmpty(urlAttribute.Value);
                if (urlHasValue)
                {
                    //set Title to be equal to url (in order to generate desired url)
                    viewCI.Title = urlAttribute.Value;
                }

                var reader = viewElement.CreateReader();
                reader.MoveToContent();
                var viewInnerXml = reader.ReadInnerXml();

                var createdView = createdList.Views.Add(viewCI);
                createdView.ListViewXml = viewInnerXml;
                if (hidden) createdView.Hidden = hidden;
                createdView.Update();
#if SP2013 || SP2016
                createdView.EnsureProperties(v => v.Scope, v => v.JSLink, v => v.Title, v => v.Aggregations, v => v.MobileView, v => v.MobileDefaultView, v => v.ViewData);
#else
                createdView.EnsureProperties(v => v.Scope, v => v.JSLink, v => v.Title, v => v.Aggregations, v => v.MobileView, v => v.MobileDefaultView, v => v.ViewData, v => v.CustomFormatter);
#endif
                web.Context.ExecuteQueryRetry();

                if (urlHasValue)
                {
                    //restore original title
                    createdView.Title = viewTitle;
                    createdView.Update();
                }

                // ContentTypeID
                var contentTypeID = (string)viewElement.Attribute("ContentTypeID");
                if (!string.IsNullOrEmpty(contentTypeID) && (contentTypeID != BuiltInContentTypeId.System))
                {
                    ContentTypeId childContentTypeId = null;
                    if (contentTypeID == BuiltInContentTypeId.RootOfList)
                    {
                        var childContentType = web.GetContentTypeById(contentTypeID);
                        childContentTypeId = childContentType != null ? childContentType.Id : null;
                    }
                    else
                    {
                        childContentTypeId = createdList.ContentTypes.BestMatch(contentTypeID);
                    }
                    if (childContentTypeId != null)
                    {
                        createdView.ContentTypeId = childContentTypeId;
                        createdView.Update();
                    }
                }

                // Default for content type
                bool parsedDefaultViewForContentType;
                var defaultViewForContentType = (string)viewElement.Attribute("DefaultViewForContentType");
                if (!string.IsNullOrEmpty(defaultViewForContentType) && bool.TryParse(defaultViewForContentType, out parsedDefaultViewForContentType))
                {
                    createdView.DefaultViewForContentType = parsedDefaultViewForContentType;
                    createdView.Update();
                }

                // Scope
                var scope = (string)viewElement.Attribute("Scope");
                var parsedScope = ViewScope.DefaultValue;
                if (!string.IsNullOrEmpty(scope) && Enum.TryParse<ViewScope>(scope, out parsedScope))
                {
                    createdView.Scope = parsedScope;
                    createdView.Update();
                }

                // MobileView
                var mobileView = viewElement.Attribute("MobileView") != null && bool.Parse(viewElement.Attribute("MobileView").Value);
                if (mobileView)
                {
                    createdView.MobileView = mobileView;
                    createdView.Update();
                }

                // MobileDefaultView
                var mobileDefaultView = viewElement.Attribute("MobileDefaultView") != null && bool.Parse(viewElement.Attribute("MobileDefaultView").Value);
                if (mobileDefaultView)
                {
                    createdView.MobileDefaultView = mobileDefaultView;
                    createdView.Update();
                }

                // Aggregations
                var aggregationsElement = viewElement.Descendants("Aggregations").FirstOrDefault();
                if (aggregationsElement != null && aggregationsElement.HasElements)
                {
                    var fieldRefString = "";
                    foreach (var fieldRef in aggregationsElement.Descendants("FieldRef"))
                    {
                        fieldRefString += fieldRef.ToString();
                    }
                    if (createdView.Aggregations != fieldRefString)
                    {
                        createdView.Aggregations = fieldRefString;
                        createdView.Update();
                    }
                }

                // JSLink
                var jslinkElement = viewElement.Descendants("JSLink").FirstOrDefault();
                if (jslinkElement != null)
                {
                    var jslink = jslinkElement.Value;
                    if (createdView.JSLink != jslink)
                    {
                        createdView.JSLink = jslink;
                        createdView.Update();

                        // Only push the JSLink value to the web part as it contains a / indicating it's a custom one. So we're not pushing the OOB ones like clienttemplates.js or hierarchytaskslist.js
                        // but do push custom ones down to th web part (e.g. ~sitecollection/Style Library/JSLink-Samples/ConfidentialDocuments.js)
                        if (jslink.Contains("/"))
                        {
                            createdView.EnsureProperty(v => v.ServerRelativeUrl);
                            createdList.SetJSLinkCustomizations(createdView.ServerRelativeUrl, jslink);
                        }
                    }
                }

#if !ONPREMISES || SP2019
                // CustomFormatter
                var customFormatterElement = viewElement.Descendants("CustomFormatter").FirstOrDefault();
                if (customFormatterElement != null)
                {
                    var customFormatter = customFormatterElement.Value;
                    customFormatter = customFormatter.Replace("&", "&amp;");
                    if (createdView.CustomFormatter != customFormatter)
                    {
                        createdView.CustomFormatter = customFormatter;
                        createdView.Update();
                    }
                }
#endif

                // View Data
                var viewDataElement = viewElement.Descendants("ViewData").FirstOrDefault();
                if (viewDataElement != null && viewDataElement.HasElements)
                {
                    var fieldRefString = "";
                    foreach (var fieldRef in viewDataElement.Descendants("FieldRef"))
                    {
                        fieldRefString += fieldRef.ToString();
                    }
                    if (createdView.ViewData != fieldRefString)
                    {
                        createdView.ViewData = fieldRefString;
                        createdView.Update();
                    }
                }


                createdList.Update();
                web.Context.ExecuteQueryRetry();

                // Add ListViewId token parser
                createdView.EnsureProperty(v => v.Id);
                //parser.AddToken(new ListViewIdToken(web, createdList.Title, createdView.Title, createdView.Id));

#if !SP2013
                // Localize view title
                //if (displayNameElement.Value.ContainsResourceToken())
                {
                    //createdView.LocalizeView(web, displayNameElement.Value, parser, monitoredScope);
                }
#endif
            }
            catch (Exception ex)
            {
                //monitoredScope.LogError(CoreResources.Provisioning_ObjectHandlers_ListInstances_Creating_view_failed___0_____1_, ex.Message, ex.StackTrace);
                throw;
            }
        }



        public static void UpdateContentTypeId(ClientContext context)
        {
            context.Load(context.Web);
            context.ExecuteQuery();
            string SiteUrl = "https://jingge.sharepoint.com/sites/1118_WorkFlow_Mix";
            var uri = new Uri(SiteUrl);
            var list = context.Web.Lists.GetByTitle("Dis");
            var view1 = list.Views.GetByTitle("Featured Discussions");
            var view2 = list.Views.GetByTitle("Test2");
            var view3 = list.Views.GetByTitle("Subject");
            context.Load(view1);
            context.Load(view2);
            context.Load(view3);
            context.ExecuteQuery();
            var ct1 = context.Web.ContentTypes.GetById(view1.ContentTypeId.StringValue);
            context.Load(ct1);
            var ct3 = context.Web.ContentTypes.GetById(view3.ContentTypeId.StringValue);
            context.Load(ct3);
            var ct2 = list.ContentTypes.GetById(view2.ContentTypeId.StringValue);
            context.Load(ct2);
            context.Load(ct3);
            context.ExecuteQuery();
            view2.ContentTypeId = ct1.Id;
            view2.Update();
            context.ExecuteQuery();

            view2.ContentTypeId = view1.ContentTypeId;
            view2.Update();
            context.ExecuteQuery();
        }
    }
}
