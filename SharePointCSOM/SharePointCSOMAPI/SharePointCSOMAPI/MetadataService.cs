using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class MetadataService
    {
        public static void Test1(ClientContext context)
        {
            var termStoreId = new Guid("0b77086dbdd74f9daa23235581e09cef");
            var termId = new Guid("df07a9c0-245f-4170-9fd7-c05ab77ca320");
            TaxonomySession session = TaxonomySession.GetTaxonomySession(context);
            TermStore store = session.TermStores.GetById(termStoreId);
            Term term = store.GetTerm(termId);
            ClientResult<string> defaultLabel = term.GetDefaultLabel(1033);
            context.ExecuteQuery();
        }
    }
}
