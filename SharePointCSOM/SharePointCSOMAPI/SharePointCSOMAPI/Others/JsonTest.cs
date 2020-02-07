using Microsoft.SharePoint.Client.Taxonomy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Others
{
    class JsonTest
    {
        public static void Test()
        {
            dynamic abc = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(@"C:\Users\xluo\Desktop\listProperties.txt"));
            foreach (dynamic role in abc.d.results)
            {
                var principalId = Convert.ToInt32(role.PrincipalId.Value);

                foreach (dynamic roleDefinitionbinding in role.RoleDefinitionBindings.results)
                {
                    var roleId = Convert.ToInt32(roleDefinitionbinding.Id.Value);
                }
                var url = role.RoleDefinitionBindings.__deferred.uri.Value;
                var description = role.Description.Value;
                var Id = Convert.ToInt32(role.Id.Value);
                var name = role.Name.Value;
                var order = Convert.ToInt32(role.Order.Value);
                var basePermissionsHigh = Convert.ToUInt32(role.BasePermissions.High.Value);
                var basePermissionsLow = Convert.ToUInt32(role.BasePermissions.Low.Value);
            }
            var valuese = JsonConvert.DeserializeObject<TaxonomyFieldValue>(System.IO.File.ReadAllText(@"C:\Users\xluo\Desktop\value.txt"));

            var Label = valuese.Label;
            var termGuid = valuese.TermGuid;
            var Wssid = valuese.WssId;


        }
    }
}
