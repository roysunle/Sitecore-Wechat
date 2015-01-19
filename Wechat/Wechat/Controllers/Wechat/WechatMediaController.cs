using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;
using Sitecore;
using Sitecore.Shell.Data;
using Sitecore.Shell.Framework;
using Sitecore.Text;
using Sitecore.Web.UI.XamlSharp.Ajax;

namespace Wechat.Controllers.Wechat
{
    public class WechatMediaController : SitecoreController
    {
        //
        // GET: /WechatMedia/
        public override ActionResult Index()
        {

            Client.AjaxScriptManager.OnExecute += Current_OnExecute;
            return View("~/Views/Wechat/SubLayouts/WechatMedia.cshtml");
        }

        private static void Current_OnExecute(object sender, AjaxCommandEventArgs args)
        {
            switch (args.Command.Name)
            {
                case "item:open":
                    string id = args.Parameters["id"];
                    Item item = Client.ContentDatabase.GetItem(id);

                    string sectionID = RootSections.GetSectionID(id);
                    UrlString str2 = new UrlString();
                    str2.Append("ro", sectionID);
                    str2.Append("fo", id);
                    str2.Append("id", id);
                    str2.Append("la", item.Language.ToString());
                    str2.Append("vs", item.Version.ToString());
                    Windows.RunApplication("Content editor", str2.ToString());
                    break;
            }
        }
	}
}