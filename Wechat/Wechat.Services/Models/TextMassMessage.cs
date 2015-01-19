using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.Services.Models
{
    public class TextMassMessage:BaseGroupMassMessage
    {
        public WechatText text { get; set; }
    }

    public class WechatText
    {
        public string content { get; set; }
    }

    
}
