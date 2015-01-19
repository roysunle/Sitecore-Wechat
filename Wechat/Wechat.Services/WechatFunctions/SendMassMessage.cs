using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sitecore.Diagnostics;
using Wechat.Services.AccessToken;
using Wechat.Services.Models;
using Wechat.Services.Models.MassMessage;

namespace Wechat.Services.WechatFunctions
{
    public class SendMassMessage
    {
        //private static readonly 

        public static SendMassMessageResp Send(string _media_id,string _type)
        {
            string accesstokenstr = AccessTokenService.CreateInstance().GetAccessToken();
            //根据分组群发
            string url = "https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token=" + accesstokenstr;

            //根据openid群发
            //string url = "https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token=" + accesstokenstr;

            //预览
            //string url = "https://api.weixin.qq.com/cgi-bin/message/mass/preview?access_token=" + accesstokenstr;

            var message = new MixedGroupMassMessage()
            {
                filter = new Filter()
                {
                    is_to_all = true,
                    group_id = ""
                },
                //touser = "oITl7uNDo1vrVVTrtkOw7QacbBMo",
                mpnews = new Mpnews()
                {
                    media_id = _media_id
                },
                msgtype = _type
            };
            //var message = new TextMassMessage()
            //{
            //    touser = "oITl7uNDo1vrVVTrtkOw7QacbBMo",
            //    text = new WechatText()
            //    {
            //        content = "test"
            //    },
            //    msgtype = "text"
            //};
            var data = JsonConvert.SerializeObject(message);
            Log.Info("send mass message:" + data, typeof(SendMassMessage));
            var result = AppService.webRequestPost(url, data);
            Log.Info("send mass message result:" + result, typeof(SendMassMessage));
            var resp = (SendMassMessageResp)JsonConvert.DeserializeObject(result, typeof(SendMassMessageResp));
            return resp;

        }

        public static SendMassMessageResp Preview(string _media_id, string _type)
        {
            string accesstokenstr = AccessTokenService.CreateInstance().GetAccessToken();
            
            //预览
            string url = "https://api.weixin.qq.com/cgi-bin/message/mass/preview?access_token=" + accesstokenstr;

            var message = new MixedTouserMassMessage()
            {
                touser = GetPreviewUser(),//"oITl7uNDo1vrVVTrtkOw7QacbBMo",
                mpnews = new Mpnews()
                {
                    media_id = _media_id
                },
                msgtype = _type
            };
            
            var data = JsonConvert.SerializeObject(message);
            Log.Info("preview mass message:" + data, typeof(SendMassMessage));
            var result = AppService.webRequestPost(url, data);
            Log.Info("preview mass message result:" + result, typeof(SendMassMessage));
            var resp = (SendMassMessageResp)JsonConvert.DeserializeObject(result, typeof(SendMassMessageResp));
            return resp;

        }

        private static string GetPreviewUser()
        {
            var result = string.Empty;
            result =
                Sitecore.Configuration.Factory.GetDatabase("master").GetItem(Constants.WechatAppId).Fields["PreviewUser"
                    ].Value;
            return result;
        }
    }

   
}
