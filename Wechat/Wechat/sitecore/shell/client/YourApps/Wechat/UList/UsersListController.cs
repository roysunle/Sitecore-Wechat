using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using Sitecore.Web;
using Wechat.Controllers.Wechat;
using Wechat.Services;
using Wechat.Services.Models;

namespace Wechat.sitecore.shell.client.YourApps.UList
{
    public class UsersListController : Controller
    {
        //
        // GET: /UsersList/
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public JsonResult Find()
        {
            var input = WebUtil.GetFormValue("input");
            var pageIndex = int.Parse(WebUtil.GetFormValue("pageIndex").ToLower().Trim());
            var users = new List<WechatUser>();
            Role role = Role.FromName("extranet\\Wechat User");
            foreach (System.Web.Security.MembershipUser membershipUser in System.Web.Security.Membership.GetAllUsers())
            {
                User user = Sitecore.Security.Accounts.User.FromName(membershipUser.UserName, false);
                
                // process user
                if (user.Roles.Contains(role))
                {
                    //users.Add(user);
                    var subscribeTime = AppService.UnixTimeToTime(user.Profile["subscribe_time"]);
                    users.Add(new WechatUser
                    {
                        subscribe = user.Profile["subscribe"],
                        openid = user.Profile["openid"],
                        nickname = user.Profile["nickname"],
                        sex = user.Profile["sex"]=="1"?"男":"女",
                        city = user.Profile["city"],
                        country = user.Profile["country"],
                        province = user.Profile["province"],
                        language = user.Profile["language"],
                        headimgurl = user.Profile["headimgurl"],
                        subscribe_time = subscribeTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
            }
            PagerList = new List<MyPager>();
            if (string.IsNullOrEmpty(input.ToLower()))
            {
                for (int i = 1; i <= Math.Ceiling((double)users.Count() / pageSize); i++)
                {
                    var index = new MyPager();
                    index.number = i;
                    PagerList.Add(index);
                }
                return Json(users);
            }
            else
            {
                var users2 = users.Where(i => i.nickname.ToLower().Contains(input.ToLower()));
                for (int i = 1; i <= Math.Ceiling((double)users2.Count() / pageSize); i++)
                {
                    var index = new MyPager();
                    index.number = i;
                    PagerList.Add(index);
                }
                return Json(users2);
            }

            
            
            
        }

        public static List<MyPager> PagerList;
        protected static int pageSize = 10;

        public JsonResult GetPagerStr()
        {
            return Json(PagerList);
        }

        public JsonResult SetPreviewUser(string id)
        {
            //var result = new ();
            var baseResp = new BaseResponse()
            {
                errcode = "-1",
                errmsg = "保存失败!"
            };

            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var user = Sitecore.Security.Accounts.User.FromName(id, false);

            if (user != null)
            {
                using (new SecurityDisabler())
                {
                    var appRootItem = masterDb.GetItem(Constants.WechatAppId);
                    if (appRootItem!=null)
                    {
                        appRootItem.Editing.BeginEdit();
                        appRootItem.Fields["PreviewUser"].Value = id;
                        appRootItem.Editing.EndEdit();
                        baseResp.errcode = "0";
                        baseResp.errmsg = "保存成功!";
                    }
                    
                }

            }

            return Json(baseResp);
        }
    }



}