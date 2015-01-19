using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using Sitecore.Web;
using Wechat.Controllers.Wechat;
using Wechat.Services;
using Wechat.Services.Models;

namespace Wechat.sitecore.shell.client.YourApps.Wechat.KeyReplyMessage
{
    public class KeyReplyMessageController : Controller
    {
        public ActionResult GetData()
        {
            var input = WebUtil.GetFormValue("input").ToLower().Trim();
            var pageIndex = int.Parse(WebUtil.GetFormValue("pageIndex").ToLower().Trim());
            var resultList = new List<ReplyMessage>();

            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var rootItem = masterDb.GetItem(new ID(Constants.ReplyMessageRootId));
            if (rootItem != null)
            {
                var messageList = rootItem.Axes.GetDescendants().ToList();
                if (!string.IsNullOrEmpty(input))
                {
                    messageList = messageList.Where(o => o.Fields["Keyword"].Value.Contains(input)).ToList();
                }
                var messageListPaged = messageList.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                foreach (var message in messageListPaged)
                {
                    var singlemessage = new ReplyMessage()
                    {
                        id = message.ID.ToString(),
                        keyword = message.Fields["Keyword"].Value,
                        replyContent = message.Fields["ReplyMessage"].Value
                    };
                    resultList.Add(singlemessage);
                }
                PagerList = new List<MyPager>();
                for (int i = 1; i <= Math.Ceiling((double)messageList.Count() / pageSize); i++)
                {
                    var index = new MyPager();
                    index.number = i;
                    PagerList.Add(index);
                }
            }
            return Json(resultList);
        }


        public static List<MyPager> PagerList;
        protected static int pageSize = 10;

        public JsonResult GetPagerStr()
        {
            return Json(PagerList);
        }
    }


    public class ReplyMessage
    {
        public string id { set; get; }
        public string keyword { set; get; }
        public string replyContent { set; get; }
        
    }
}