using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Query;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Mvc.Controllers;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using Wechat.Services;
using Wechat.Services.AccessToken;
using Wechat.Services.Models;

namespace Wechat.Controllers.Wechat
{
    public class SendMessageController : SitecoreController
    {
        //
        // GET: /SendMessage/
        //public ActionResult Index()
        //{
        //    return View();
        //}
        private Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
        
        public ActionResult Send(string propertyData)
        {
            

            JavaScriptSerializer jss = new JavaScriptSerializer();
            var property = jss.Deserialize<WechatMessage>(propertyData);

            using (new SecurityDisabler())
            {
                using (new LanguageSwitcher("zh-cn"))
                {
                    var rootItem = CheckFolder();
                    if (rootItem!=null)
                    {
                        var messageTypeTemplate = masterDb.GetTemplate(Constants.TextMessageTypeTemplateId);
                        var message = rootItem.Add(DateTime.Now.ToString("hhmmss"), messageTypeTemplate);
                        if (message!=null)
                        {
                            message.Editing.BeginEdit();
                            message.Fields["MessageText"].Value = property.Message;
                            message.Fields["SendTo"].Value = property.OpenId;
                            message.Editing.EndEdit();
                        }
                    }
                }
            }
            var result = SendWechatMessage(property);
            if (result.errcode=="0")
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            
        }

        private SendMessageResult SendWechatMessage(WechatMessage message)
        {
            string accesstokenstr = AccessTokenService.CreateInstance().GetAccessToken();
            var url = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + accesstokenstr;
            var data = "{\"touser\":\""+message.OpenId+"\",\"msgtype\":\"text\",\"text\":{\"content\":\""+message.Message+"\"}}";
            var result = AppService.webRequestPost(url,data);
            Log.Info("send message result:"+result,this);

            return (SendMessageResult)JsonConvert.DeserializeObject(result, typeof(SendMessageResult));
        }

        private Item CheckFolder()
        {
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString();
            var day = DateTime.Now.Date.ToString("dd");
            var messageFolderTemplate = masterDb.GetTemplate(Constants.MessageFolderTemplateId);
            using (new SecurityDisabler())
            {
                var yearFolder =
                masterDb.GetItem("{2786E29F-F47E-4FCA-B9EF-5B5B9EBDC171}").Children.FirstOrDefault(o => o.Name == year);
                if (yearFolder == null)
                {
                    yearFolder = masterDb.GetItem("{2786E29F-F47E-4FCA-B9EF-5B5B9EBDC171}")
                        .Add(year, messageFolderTemplate);

                }
                var monthFolder = masterDb.GetItem(yearFolder.ID).Children.FirstOrDefault(o => o.Name == month);
                if (monthFolder == null)
                {
                    monthFolder = masterDb.GetItem(yearFolder.ID)
                        .Add(month, messageFolderTemplate);

                }
                var dayFolder = masterDb.GetItem(monthFolder.ID).Children.FirstOrDefault(o => o.Name == day);
                if (dayFolder == null)
                {
                    dayFolder = masterDb.GetItem(monthFolder.ID)
                        .Add(day, messageFolderTemplate);

                }
                return dayFolder;
            }
            
        }

        public ActionResult GetNickname(string openid)
        {
            WechatUser property = new WechatUser();

            if (!string.IsNullOrEmpty(openid))
            {
                var member = System.Web.Security.Membership.FindUsersByName("extranet\\"+openid);//Sitecore.Security.Accounts.User.FromName(openid, false);
                if (member.Count>0)
                {

                    var user = Sitecore.Security.Accounts.User.FromName(member[openid].UserName, false);
                    if (user!=null)
                    {
                        property.nickname = user.Profile["nickname"];
                    }
                }
                else
                {
                    property.nickname = "测试";
                }
            }

            return Json(property, JsonRequestBehavior.AllowGet);
        }
    }

    public class WechatMessage
    {
        public string MessageType { get; set; }
        public string Message { get; set; }
        public string OpenId { get; set; }
    }

    public class SendMessageResult
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
    }

}