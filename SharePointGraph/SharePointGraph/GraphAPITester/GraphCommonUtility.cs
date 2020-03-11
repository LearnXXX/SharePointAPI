using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    public class GraphCommonUtility
    {
        public static List<T> GetRequestAllOfDatas<T>(dynamic currentPage)
        {
            var datas = new List<T>();
            datas.AddRange(currentPage.CurrentPage as List<T>);
            if (currentPage.NextPageRequest != null)
            {
                var nextPage = currentPage.NextPageRequest.GetAsync().Result;
                datas.AddRange(GetRequestAllOfDatas<T>(nextPage));
            }
            return datas;

        }
    }
}
