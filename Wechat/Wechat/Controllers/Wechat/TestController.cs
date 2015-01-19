using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Controllers;
using Sitecore.SecurityModel;
using Wechat.Services.AccessToken;
using Wechat.Services.WechatFunctions;
using Wechat.Tasks;

namespace Wechat.Controllers.Wechat
{
    public class TestController : SitecoreController
    {
        //
        // GET: /Test/
        //public override ActionResult Index()
        //{
        //    new SendMassMessage().Test();
        //    return View("/Views/Test/Index.cshtml");
        //}
	}

    partial class KeywordReply1
    {
        public string Keyword { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
    }
}