using Wechat.Services.Models.MassMessage;

namespace Wechat.Services.Models
{
    public class BaseGroupMassMessage
    {
        public Filter filter { get; set; }
        public string msgtype { get; set; }
        
    }

}
