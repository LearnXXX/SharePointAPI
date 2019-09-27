using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    [Serializable]
    public class ListInfo
    {
        public string WebUrl { get; set; }
        public Guid listId { get; set; }
        public string ListTitle { get; set; }
        public int MajorVersionLimit { get; set; }
        public bool EnableVersioning { get; set; }
        public bool EnableMinorVersions { get; set; }
        public int MajorWithMinorVersionsLimit { get; set; }
        public bool EnableModeration { get; set; }
        public int ItemCount { get; set; }
        public bool BigList { get; set; }//ItemCount>=5000
    }
}
