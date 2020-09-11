using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class ListLevel
    {
        private static Field CreateSingleLineTextFiled(List list, string internalName, string title)
        {
            return list.Fields.AddFieldAsXml($"<Field Type='User'  Name='{internalName}' StaticName='{internalName}' DisplayName='{title}'/>", true, AddFieldOptions.AddFieldInternalNameHint | AddFieldOptions.AddFieldToDefaultView);
        }

        private static object GetData()
        {
            return new List<string> { "1", "1", "1", "1", };
        }
        public static void ListFieldTest(ClientContext context)
        {
            var lll = context.Web.Lists.GetByTitle("Contract Ledger");
            var column = lll.Fields.GetFieldByInternalName("LMSChanges");
            context.Load(column);
            context.ExecuteQuery();
            var tempItem2= lll.GetItemById(590);
            tempItem2["LMSContractDescription"] = null;
            tempItem2.Update();
            context.ExecuteQuery();
            var country = lll.Fields.GetFieldByInternalName("LMSLoanNo");
            context.Load(country);
            context.ExecuteQuery();

            var role = context.Web.RoleDefinitions.GetById(1073741826);
            context.Load(role);
            context.ExecuteQuery();

            StringBuilder stringBuilder = new StringBuilder();
            var keyFields = new List<int> { 1, 2 };
            keyFields.ForEach(keyField => stringBuilder.Append($"{keyField}, "));
            stringBuilder.Length--;

            var tempList = context.Web.Lists.GetByTitle("PPP");
            var tempItems = tempList.GetItems(CamlQuery.CreateAllItemsQuery());

            context.Load(tempItems, temp => temp.IncludeWithDefaultProperties(ttt => ttt.HasUniqueRoleAssignments));
            context.ExecuteQuery();
            foreach (var aa in tempItems.Where(bb => object.Equals(bb["_ModerationStatus"], 0)))
            {

            }
            tempItems[0].Recycle();
            context.ExecuteQuery();


            var tempItem = tempList.GetItemById(1);
            tempItem.ResetRoleInheritance();
            tempItem.BreakRoleInheritance(false, false);
            context.Load(tempItem);
            context.ExecuteQuery();


            //permissionManagementList.Retrieve("Id");
            //context.Load(permissionManagementList.Fields);
            //context.ExecuteQuery();
            //context.Load(permissionManagementList);
            //context.Load(permissionManagementList.Fields);
            //var loanN = permissionManagementList.Fields.GetFieldByInternalName("UUU");
            //context.Load(loanN);
            //context.ExecuteQuery();
            //var permissionQuery = CamlQuery.CreateAllItemsQuery();
            //permissionQuery.ViewXml = $"<View Scope='RecursiveAll'><Query><Where><Eq><FieldRef Name='LoanNO'/><Value Type='Text'>2</Value></Eq></Where></Query></View>";
            //var permissionItems = permissionManagementList.GetItems(permissionQuery);
            //context.Load(permissionItems);
            //context.ExecuteQuery();


            var list = context.Web.Lists.GetByTitle("Permission Management");
            var item1 = list.GetItemById(17);
            var item2 = list.GetItemById(22);
            context.Load(item1);
            context.Load(item2);
            context.ExecuteQuery();

            var query = CamlQuery.CreateAllItemsQuery();
            query.ViewXml = $"<View Scope='RecursiveAll'><Query><Where><Eq><FieldRef Name='_ModerationStatus'/><Value Type='ModStat'>0</Value></Eq></Where></Query></View>";
            var allItems = list.GetItems(query);
            context.Load(allItems);
            context.ExecuteQuery();
            foreach (var itemssdf in allItems)
            { }

            context.Load(list.Fields);
            var field1 = list.Fields.GetByTitle("BooleanTest");
            context.Load(field1);
            context.ExecuteQuery();
            var item = list.GetItemById(1);
            context.Load(item);
            context.ExecuteQuery();
            var items = list.GetItems(CamlQuery.CreateAllItemsQuery());
            context.Load(items);
            context.ExecuteQuery();

            var field = list.Fields.AddFieldAsXml(System.IO.File.ReadAllText(@"C:\Users\xluo\Desktop\111.txt"), true, AddFieldOptions.AddFieldInternalNameHint);
            context.Load(field);
            context.ExecuteQuery();


        }
        public static void GetItemsTest(ClientContext context)
        {
            var list = context.Web.Lists.GetByTitle("InternalNameTest");
            var field = list.Fields.GetByTitle("Loan NO.");
            context.Load(field);
            context.ExecuteQuery();
            var query = CamlQuery.CreateAllItemsQuery();
            //var viewXml = $"<View Scope='RecursiveAll'><Query><OrderBy><FieldRef Name='{field.InternalName}' Ascending='FALSE'/></OrderBy></Query><RowLimit>2</RowLimit></View>";
            //query.ViewXml = viewXml;
            var items = list.GetItems(query);
            context.Load(items);
            context.ExecuteQuery();

            query.ListItemCollectionPosition = new ListItemCollectionPosition { PagingInfo = "Paged=TRUE&p_Title=4&p_ID=4" };
            var item2 = list.GetItems(query);
            context.Load(item2);
            context.ExecuteQuery();
        }

        public static void LoadListProperty(ClientContext context)
        {
            string title = "sd";
            string fieldXml = $"<Field Type='Choice' DisplayName='{title}' <CHOICES>{{0}}</CHOICES> />";

            var list = context.Web.Lists.GetByTitle("TestList");

            context.Load(list.Fields);
            context.ExecuteQuery();




            context.Load(list.Fields);
            context.Load(list);
            context.Load(list.RootFolder);
            context.ExecuteQuery();
            context.Load(list, l => l.HasUniqueRoleAssignments);
            context.ExecuteQuery();
            context.Load(list.Fields);
            var filed = list.Fields.GetFieldByInternalName("_ModerationStatus");
            context.Load(filed);
            var fields = list.Fields.Where(f => !f.Hidden && "_Hidden" != f.Group && !f.ReadOnlyField);
            foreach (var field in list.Fields.Where(f => !f.Hidden && "_Hidden" != f.Group && !f.ReadOnlyField))
            { }
            context.ExecuteQuery();
        }
    }
}
