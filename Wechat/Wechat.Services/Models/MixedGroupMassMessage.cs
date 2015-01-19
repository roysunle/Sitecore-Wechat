using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wechat.Services.Models.MassMessage;

namespace Wechat.Services.Models
{
    public class MixedGroupMassMessage:BaseGroupMassMessage
    {
        public Mpnews mpnews { get; set; }
    }

    
}
