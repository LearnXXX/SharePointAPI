using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.VivianTools
{
    public class FileInfo
    {
        public string Url { get; set; }

        public override bool Equals(object obj)
        {
            var temp = obj as FileInfo;

            return string.Equals(this.Url, temp.Url, StringComparison.OrdinalIgnoreCase);
        }
    }
}
