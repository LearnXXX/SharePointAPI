using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class DocumentLevel
    {
        public static void DownLoadFile(ClientContext context)
        {
            string fileUrl = "";
            var file = context.Web.GetFileByUrl(fileUrl);
            var stream = file.OpenBinaryStream();
            context.ExecuteQuery();
            var memory = new MemoryStream(Convert.ToInt32(stream.Value.Length));
            stream.Value.CopyTo(memory);

        }
    }
}
