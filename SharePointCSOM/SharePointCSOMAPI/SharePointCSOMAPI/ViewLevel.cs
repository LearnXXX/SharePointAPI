namespace SharePointCSOMAPI
{
    using Microsoft.SharePoint.Client;
    using System;

    class ViewLevel
    {
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
