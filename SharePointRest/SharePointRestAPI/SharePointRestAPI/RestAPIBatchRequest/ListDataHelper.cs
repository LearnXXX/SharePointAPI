using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharePointRestAPI
{
    public class ListDataHelper
    {
        public static List<XElement> ExtractListItemsFromATOMResponse(Stream stream)
        {
            XDocument oDataXML = XDocument.Load(stream, LoadOptions.None);
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            XNamespace m = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

            // The ATOM markup for a SharePoint list nests field elements under <entry> <content> <properties>.
            List<XElement> entries = oDataXML.Descendants(atom + "entry")
                                     .Elements(atom + "content")
                                     .Elements(m + "properties")
                                     .ToList();

            return entries;
        }

        public static IEnumerable<SharePointListItemTitle> GetItemTitles(List<XElement> entries)
        {
            XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            IEnumerable<SharePointListItemTitle> entryTitles = from entry in entries
                                                               select new SharePointListItemTitle()
                                                               {
                                                                   Title = entry.Element(d + "Title").Value,
                                                               };
            return entryTitles;
        }
    }

    public class SharePointListItemTitle
    {
        public String Title { get; set; }
    }
}
