using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Web;
using Wechat.Controllers.Wechat;
using Wechat.Services;

namespace Wechat.sitecore.shell.client.YourApps.Wechat.MassMessageHistory
{
    public class MassMessageHistoryController : Controller
    {
        public ActionResult GetData()
        {
            var input = WebUtil.GetFormValue("input").ToLower().Trim();
            var pageIndex = int.Parse(WebUtil.GetFormValue("pageIndex").ToLower().Trim());
            var resultList = new List<MessageHistory>();
            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var folderTemplate = new ID("{093B6A4E-4B1E-4DF1-9331-EF742271F763}");
            var masscontainer = masterDb.GetItem(Constants.MassMessageContainerId);
            if (masscontainer!=null)
            {
                var messageList = masscontainer.Axes.GetDescendants().Where(o => o.TemplateID != folderTemplate );
                if (!string.IsNullOrEmpty(input))
                {
                    messageList = messageList.Where(o => o.Fields["Title"].Value.Contains(input)).ToList();
                }
                var messageListPaged = messageList.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                foreach (var message in messageListPaged)
                {
                    var singlemessage = new MessageHistory();
                    switch (message.TemplateName)
                    {
                        case "Mixed Message Type":
                            singlemessage.Type = "图文消息";
                            break;
                        case "MultiMedia Message Type":
                            singlemessage.Type = "多媒体消息";
                            break;
                        case "Text Message Type":
                            singlemessage.Type = "文本消息";
                            break;

                    }
                    DateField sendtime = message.Fields["SendTime"];
                    singlemessage.SendTime = sendtime.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    singlemessage.State = message.Fields["State"].Value;
                    var material = masterDb.GetItem(message.Fields["ContentId"].Value);
                    if (material!=null)
                    {
                        singlemessage.Title = material.Fields["Title"].Value;
                    }
                    else
                    {
                        singlemessage.Title = "";
                    }
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
            //resultList.OrderByDescending(o => o.SendTime);
            //if (!string.IsNullOrEmpty(input))
            //{
            //    resultList = resultList.Where(o => o.Title.Contains(input)).ToList();
            //}
            return Json(resultList.OrderByDescending(o => o.SendTime));
        }

        public static List<MyPager> PagerList;
        protected static int pageSize = 10;

        public JsonResult GetPagerStr()
        {
            return Json(PagerList);
        }
    }

    public class MessageHistory
    {
        public string Type { set; get; }
        public string SendTime { set; get; }
        public string State { set; get; }
        public string Title { set; get; }
    }
}