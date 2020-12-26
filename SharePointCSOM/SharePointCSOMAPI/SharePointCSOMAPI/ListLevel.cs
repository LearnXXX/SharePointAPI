using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class ListLevel
    {
        public static List<dynamic> GetDynamics()
        {
            object o1 = new { ID = "1", JobTime = DateTime.Now.ToString() };
            object o2 = new { ID = "1", JobTime = DateTime.Now.ToString() };
            List<dynamic> datas = new List<dynamic>();
            datas.Add(o1);
            datas.Add(o2);
            return datas;
            return new List<dynamic> {
                o1,o2
                //new { ID = "1", JobTime = DateTime.Now.ToString() },
                //new { ID = "1", JobTime = DateTime.Now.ToString() },
                //new { ID = "1", JobTime = DateTime.Now.ToString() },
                //new { ID = "1", JobTime = DateTime.Now.ToString() },
            };
        }

        public static string[] CountryCollection = new string[] { "Afghanistan", "Albania", "Algeria", "American Samoa", "Andorra", "Angola", "Anguilla", "Antarctica", "Antigua and Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahamas (the)", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia (Plurinational State of)", "Bonaire, Sint Eustatius and Saba", "Bosnia and Herzegovina", "Botswana", "Bouvet Island", "Brazil", "British Indian Ocean Territory (the)", "Brunei Darussalam", "Bulgaria", "Burkina Faso", "Burundi", "Cabo Verde", "Cambodia", "Cameroon", "Canada", "Cayman Islands (the)", "Central African Republic (the)", "Chad", "Chile", "China", "Christmas Island", "Cocos (Keeling) Islands (the)", "Colombia", "Comoros (the)", "Congo (the Democratic Republic of the)", "Congo (the)", "Cook Islands (the)", "Costa Rica", "Croatia", "Cuba", "Curaçao", "Cyprus", "Czechia", "Côte d'Ivoire", "Denmark", "Djibouti", "Dominica", "Dominican Republic (the)", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Eswatini", "Ethiopia", "Falkland Islands (the) [Malvinas]", "Faroe Islands (the)", "Fiji", "Finland", "France", "French Guiana", "French Polynesia", "French Southern Territories (the)", "Gabon", "Gambia (the)", "Georgia", "Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guadeloupe", "Guam", "Guatemala", "Guernsey", "Guinea", "Guinea-Bissau", "Guyana", "Haiti", "Heard Island and McDonald Islands", "Holy See (the)", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Iran (Islamic Republic of)", "Iraq", "Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Korea (the Democratic People's Republic of)", "Korea (the Republic of)", "Kuwait", "Kyrgyzstan", "Lao People's Democratic Republic (the)", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Macao", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands (the)", "Martinique", "Mauritania", "Mauritius", "Mayotte", "Mexico", "Micronesia (Federated States of)", "Moldova (the Republic of)", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Myanmar", "Namibia", "Nauru", "Nepal", "Netherlands (the)", "New Caledonia", "New Zealand", "Nicaragua", "Niger (the)", "Nigeria", "Niue", "Norfolk Island", "North Macedonia", "Northern Mariana Islands (the)", "Norway", "Oman", "Pakistan", "Palau", "Palestine, State of", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines (the)", "Pitcairn", "Poland", "Portugal", "Puerto Rico", "Qatar", "Romania", "Russian Federation (the)", "Rwanda", "Réunion", "Saint Barthélemy", "Saint Helena, Ascension and Tristan da Cunha", "Saint Kitts and Nevis", "Saint Lucia", "Saint Martin (French part)", "Saint Pierre and Miquelon", "Saint Vincent and the Grenadines", "Samoa", "San Marino", "Sao Tome and Principe", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Sint Maarten (Dutch part)", "Slovakia", "Slovenia", "Solomon Islands", "Somalia", "South Africa", "South Georgia and the South Sandwich Islands", "South Sudan", "Spain", "Sri Lanka", "Sudan (the)", "Suriname", "Svalbard and Jan Mayen", "Sweden", "Switzerland", "Syrian Arab Republic (the)", "Taiwan (Province of China)", "Tajikistan", "Tanzania, the United Republic of", "Thailand", "Timor-Leste", "Togo", "Tokelau", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks and Caicos Islands (the)", "Tuvalu", "Uganda", "Ukraine", "United Arab Emirates (the)", "United Kingdom of Great Britain and Northern Ireland (the)", "United States Minor Outlying Islands (the)", "United States of America (the)", "Uruguay", "Uzbekistan", "Vanuatu", "Venezuela (Bolivarian Republic of)", "Viet Nam", "Virgin Islands (British)", "Virgin Islands (U.S.)", "Wallis and Futuna", "Western Sahara*", "Yemen", "Zambia", "Zimbabwe", "Åland Islands", };
        private static Field CreateSingleLineTextFiled(List list, string internalName, string title)
        {
            return list.Fields.AddFieldAsXml($"<Field Type='User'  Name='{internalName}' StaticName='{internalName}' DisplayName='{title}'/>", true, AddFieldOptions.AddFieldInternalNameHint | AddFieldOptions.AddFieldToDefaultView);
        }

        private static object GetData()
        {
            return new List<string> { "1", "1", "1", "1", };
        }

        public static void AddPermissionItem(ClientContext context)
        {
            var list = context.Web.Lists.GetByTitle("Permission Management");
            var newItem = list.AddItem(new ListItemCreationInformation());
            newItem["LMSEditPermissionGroup"] = new FieldLookupValue { LookupId = 6 };
            newItem["LMSReadPermissionGroup"] = new FieldLookupValue { LookupId = 6 };
            newItem["LMSCountry"] = CountryCollection;
            newItem.Update();
            context.ExecuteQuery();
        }
        public static void LMSListTest(ClientContext context)
        {

            var settings = context.Web.RegionalSettings;
            context.Load(settings, s => s.LocaleId, s => s.TimeZone);
            context.ExecuteQuery();
            var bias = context.Web.RegionalSettings.TimeZone.Information.Bias;
            CultureInfo culture = new CultureInfo((int)settings.LocaleId);

            var list = context.Web.Lists.GetByTitle("PPP");

            var item0 = list.GetItemById(0);
            context.Load(item0);
            context.ExecuteQuery();

            context.Load(list);
            context.Load(list.Fields);
            context.ExecuteQuery();
            var item716 = list.GetItemById(716);
            var item718 = list.GetItemById(718);
            context.Load(item716);
            context.Load(item718);
            context.ExecuteQuery();
        }
        public static void ListFieldTest(ClientContext context)
        {

            var sdada = "{\"additionalRowClass\":{\"operator\":\":\",\"operands\":[{\"operator\":\"!=\",\"operands\":[\"[${0}]\",\"ACTIVE\"]},\"sp-css-backgroundColor-blockingBackground50\",\"\"]},\"rowClassTemplateId\":\"ConditionalView\"}";
            var tl = context.Web.Lists.GetByTitle("Contract Ledger");
            context.Load(tl.DefaultView);
            context.ExecuteQuery();

            context.Load(context.Site);
            context.ExecuteQuery();
            context.Site.DisableAppViews = true;
            context.Site.DisableFlows = true;
            context.ExecuteQuery();
            var itemsss = tl.GetItems(new CamlQuery { ViewXml = $"<View Scope='RecursiveAll'><Query><Where><IsNotNull><FieldRef Name='LMSEDIContractID' /></IsNotNull></Where></Query><RowLimit>30</RowLimit></View>" });
            context.Load(itemsss);
            context.ExecuteQuery();
            AddPermissionItem(context);
            var date = Convert.ToDateTime("2020-09-15T16:00:00.000Z");
            //context.Load(context.Web.RegionalSettings.TimeZone);
            //context.ExecuteQuery();
            var utcDate = context.Web.RegionalSettings.TimeZone.LocalTimeToUTC(date);
            context.ExecuteQuery();
            //AddPermissionItem(context);
            var lll = context.Web.Lists.GetByTitle("Sync Logs");
            context.Load(lll, l => l.DefaultView.ViewFields);
            context.ExecuteQuery();
            var column = lll.Fields.GetFieldByInternalName("ccc");
            context.Load(column);
            context.ExecuteQuery();
            var tempItem2 = lll.GetItemById(590);
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
            var list = context.Web.Lists.GetByTitle("Contract Ledger");
            //var tempItem = list.GetItemById(591);
            var tempItem = list.GetItemById(590);
            context.Load(tempItem);
            context.ExecuteQuery();
            var date = (DateTime)tempItem["LMSContractSigningDate"];

            var tempDate = "2020-10-6";
            //tempItem["LMSContractSigningDate"] = 

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
