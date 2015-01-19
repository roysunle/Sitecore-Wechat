using System.Web.Mvc;
using System.Web.Script.Serialization;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.SecurityModel;
using Wechat.Models.Wechat;
namespace Wechat.Controllers.Wechat
{
    public class ProcessController : Controller
    {
        public ActionResult SaveProperty(string propertyData)
        {
            var CoreDB = Sitecore.Configuration.Factory.GetDatabase("core");

            JavaScriptSerializer jss = new JavaScriptSerializer();
            Property property = jss.Deserialize<Property>(propertyData);

            using (new SecurityDisabler())
            {
                //using (new EventDisabler())
                //{
                    var item = CoreDB.GetItem(new ID("{A430A591-B58F-4AF1-BF35-769413B09D71}"));

                    item.Editing.BeginEdit();
                    item.Fields["Text"].Value = property.Content;

                    item.Editing.EndEdit();
                //}
                
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}