using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class StopListInheritingPermissionsOption
    {
        [Option('f', "SiteUrlFile", Required = true)]
        public string SiteUrlFile { get; set; }
        [Option('u', "UserName", Required = true)]
        public string UserName { get; set; }

        [Option('p', "Password", Required = true)]
        public string Password { get; set; }

        [Option('k', "KeyWord", Required = true)]
        public string KeyWord { get; set; }
    }
}
