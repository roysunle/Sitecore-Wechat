using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.Services.Models
{
    public class WechatUser
    {
        public string subscribe { get; set; }
        public string openid { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string language { get; set; }
        public string headimgurl { get; set; }
        public string subscribe_time { get; set; }
        public string unionid { get; set; }
    }
}
