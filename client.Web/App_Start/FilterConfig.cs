using System.Web;
using System.Web.Mvc;

namespace client.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
          filters.Add(new AuthorizeAttribute());

        }
    }
}