using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.Services.Models
{
    public enum MassMessageState:byte
    {
        [Description("草稿")]
        Draft = 0,

        [Description("查询中")]
        Checking = 2,

        [Description("查询完成")]
        Checked = 4,

        [Description("下单")]
        Ordered = 6,

        [Description("付款成功")]
        Payed = 8,

        [Description("发货中")]
        Delivering = 10,

        [Description("送达")]
        Delivered = 12,

        [Description("成功")]
        Success = 14, 
    }
}
