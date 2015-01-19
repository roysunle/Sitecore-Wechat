using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Search;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using Sitecore.Tasks;
using Wechat.Services;
using Wechat.Services.AccessToken;
using Wechat.Services.Models;


namespace Wechat.Tasks
{
    public class GetWechatUsers
    {
        public void Execute(Item[] itemArray, CommandItem commandItem, ScheduleItem scheduledItem)
        {
            Log.Info(" ======================================= GetWechatUsers START =============================== ", this);

            string accesstokenstr = AccessTokenService.CreateInstance().GetAccessToken();
            if (!string.IsNullOrEmpty(accesstokenstr))
            {
                getUsers();
            }
            else
            {
                Log.Error("GetWechatUsers Task get access token error", this);
            }


            Log.Info(" ======================================= GetWechatUsers END =============================== ", this);
        }
        
        private void getUsers(string nextopenid = null)
        {
            string accesstokenstr = AccessTokenService.CreateInstance().GetAccessToken();
            string url = "https://api.weixin.qq.com/cgi-bin/user/get?access_token=" + accesstokenstr;
            if (!string.IsNullOrEmpty(nextopenid))
            {
                url += "&next_openid=" + nextopenid;
            }
            string result = AppService.HttpRequestGet(url);
            Log.Info("http result:" + result, this);
            if (result.Contains("data"))
            {
                WechatUserList resultList = (WechatUserList)JsonConvert.DeserializeObject(result, typeof(WechatUserList));
                if (resultList != null)
                {
                    if (resultList.data != null)
                    {
                        foreach (var openid in resultList.data.openid)
                        {
                            //Log.Info("openid:" + openid, this);
                            using (new SecurityDisabler())
                            {

                                var domainUser = @"extranet\" + openid;
                                if (!User.Exists(domainUser))
                                {
                                    //create
                                    User user = User.Create(domainUser, "b");

                                    //bind a role
                                    const string member = @"extranet\Wechat User";
                                    //Role role = Sitecore.Context.Domain.GetRoles().FirstOrDefault(a => a.Name.Contains(member));
                                    Role role = Sitecore.Configuration.Factory.GetDomain("extranet").GetRoles().FirstOrDefault(a => a.Name.Contains(member));
                                    if (role != null)
                                    {
                                        user.Roles.Add(role);
                                    }

                                    //bind profile
                                    const string profilePath = "/sitecore/system/Settings/Security/Profiles/Wechat User Profile";
                                    Database dbCore = Sitecore.Configuration.Factory.GetDatabase("core");
                                    Item profileItem = dbCore.GetItem(profilePath);
                                    user.Profile.ProfileItemId = profileItem.ID.ToString();

                                    var userInfo = GetUserInfo(openid);
                                    if (userInfo!=null)
                                    {
                                        user.Profile["subscribe"] = userInfo.subscribe;
                                        user.Profile["openid"] = userInfo.openid;
                                        user.Profile["nickname"] = userInfo.nickname;
                                        user.Profile["sex"] = userInfo.sex;
                                        user.Profile["city"] = userInfo.city;
                                        user.Profile["country"] = userInfo.country;
                                        user.Profile["province"] = userInfo.province;
                                        user.Profile["language"] = userInfo.language;
                                        user.Profile["headimgurl"] = userInfo.headimgurl;
                                        user.Profile["subscribe_time"] = userInfo.subscribe_time;
                                        //user.Profile["unionid"] = userInfo.unionid;
                                        user.Profile.Save();
                                    }
                                    
                                }
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(resultList.next_openid))
                    {
                        getUsers(resultList.next_openid);
                    }
                }
            }
            
        }

        private WechatUser GetUserInfo(string openId, string language = "zh_CN")
        {
            string accesstokenstr = AccessTokenService.CreateInstance().GetAccessToken();
            string url =
                "https://api.weixin.qq.com/cgi-bin/user/info?access_token=" + accesstokenstr + "&openid=" + openId + "&lang=" + language;
            string result = AppService.HttpRequestGet(url);
            WechatUser resultList = (WechatUser)JsonConvert.DeserializeObject(result, typeof(WechatUser));
            return resultList;
        }
    }

    //public class UserInfo
    //{
    //    public string subscribe { get; set; }
    //    public string openid { get; set; }
    //    public string nickname { get; set; }
    //    public string sex { get; set; }
    //    public string city { get; set; }
    //    public string country { get; set; }
    //    public string province { get; set; }
    //    public string language { get; set; }
    //    public string headimgurl { get; set; }
    //    public string subscribe_time { get; set; }
    //    public string unionid { get; set; }
    //}

    public class WechatUserList
    {
        public int total { get; set; }
        public int count { get; set; }
        public UserOpenId data { get; set; }
        public string next_openid { get; set; }
    }

    public class UserOpenId
    {
        public string[] openid { get; set; }
    }

}
