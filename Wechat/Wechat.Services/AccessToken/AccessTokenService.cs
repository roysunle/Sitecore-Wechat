using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;

namespace Wechat.Services.AccessToken
{
    public class AccessTokenService
    {
        private volatile static AccessTokenService _instance = null;
        private static readonly object lockHelper1 = new object();
        private static readonly object lockHelper2 = new object();
        private string appId;
        private string appsecret;
        private AccessTokenService() { }
        public static AccessTokenService CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper1)
                {
                    if (_instance == null)
                    {
                        //Log.Info("111", typeof(AccessTokenService));
                        _instance = new AccessTokenService();
                        var CoreDB = Sitecore.Configuration.Factory.GetDatabase("master");
                        using (new SecurityDisabler())
                        {
                            var item = CoreDB.GetItem(new ID(Constants.WechatAppId));
                            _instance.appId = item.Fields["appId"].Value;
                            _instance.appsecret = item.Fields["appsecret"].Value;
                            //Log.Info(_instance.appId + " | " + _instance.appsecret, typeof(AccessTokenService));
                        }
                    }
                    //Log.Info("222", typeof(AccessTokenService));
                    
                }
            }
            return _instance;
        }

        public string GetAccessToken()
        {
            //Log.Info("333", typeof(AccessTokenService));
            var AccessTokenStr = string.Empty;
            lock (lockHelper2)
            {

                var coreDb = Sitecore.Configuration.Factory.GetDatabase("core");
                using (new SecurityDisabler())
                {
                    //Log.Info("444", typeof(AccessTokenService));
                    var item = coreDb.GetItem(new ID(Constants.AccessTokenId));
                    if (item != null)
                    {
                        //Log.Info("555", typeof(AccessTokenService));
                        AccessTokenStr = item.Fields["AccessToken"].Value;
                        var updateField = (DateField)item.Fields["__Updated"];
                        var updateDateTime = updateField.DateTime;
                        var timedifff = DateTime.Now.Subtract(updateDateTime);
                        Log.Info("timedifff:" + timedifff.TotalSeconds, typeof(AccessTokenService));
                        if (string.IsNullOrEmpty(AccessTokenStr) || timedifff.TotalSeconds > 7200)
                        {
                            Log.Info("send to get access token", this);
                            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid="+appId+"&secret="+appsecret;
                            string result = AppService.HttpRequestGet(url);
                            var deserializedProduct = (accessToken)JsonConvert.DeserializeObject(result, typeof(accessToken));

                            item.Editing.BeginEdit();
                            item.Fields["AccessToken"].Value = deserializedProduct.access_Token;
                            item.Fields["ExpiresIn"].Value = deserializedProduct.expires_In.ToString();
                            item.Editing.EndEdit();
                            
                            AccessTokenStr = deserializedProduct.access_Token;
                        }
                        else
                        {
                            Log.Info("do not need to send to get access token", this);
                        }
                    }
                    else
                    {
                        Log.Info("access token item not found", this);
                    }
                }
                return AccessTokenStr;
            }
        }
    }

    public class accessToken
    {
        public string access_Token { get; set; }

        public int expires_In { get; set; }
    }
}
