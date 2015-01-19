using System.Collections.Generic;
using System.Web.Mvc;

namespace Wechat.Controllers.Wechat
{
    public class BaseController : Controller
    {
        public List<MyPager> PagerList;
        protected static int pageSize = 10;

        public JsonResult GetPagerStr()
        {
            return Json(PagerList);
        }
    }

    public class MyPager
    {
        public int number { set; get; }
    }
}