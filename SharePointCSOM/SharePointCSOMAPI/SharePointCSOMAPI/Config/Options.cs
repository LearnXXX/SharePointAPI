using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class Options
    {
        [Option('u', "UserName", Required = true)]
        public string UserName { get; set; }

        [Option('p', "Password", Required = true)]
        public string Password { get; set; }

        [Option('s', "SiteUrl", Required = true)]
        public string SiteUrl { get; set; }
    }
}
