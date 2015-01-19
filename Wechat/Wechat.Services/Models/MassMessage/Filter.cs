using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.Services.Models.MassMessage
{
    public class Filter
    {
        public bool is_to_all { get; set; }
        public string group_id { get; set; }
    }
}
