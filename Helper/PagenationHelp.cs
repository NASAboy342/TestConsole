using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Helper
{
    public class PagenationHelp
    {
        public static List<Tobject> GetPage<Tobject>(int page, int itemsPerPage, List<Tobject> items, out int pages)
        {
            if (items == null || items.Count == 0 || page <= 0 || itemsPerPage <= 0)
            {
                pages = 0;
                return new List<Tobject>();
            }
            pages = (int)Math.Round(((double)items.Count / (double)itemsPerPage), MidpointRounding.ToPositiveInfinity);
            if (page > pages)
            {
                return new List<Tobject>();
            }
            return items.Skip((page-1) * itemsPerPage).Take(itemsPerPage).ToList();
        }
    }
}
