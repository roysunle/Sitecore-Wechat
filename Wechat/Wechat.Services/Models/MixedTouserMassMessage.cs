using Wechat.Services.Models.MassMessage;

namespace Wechat.Services.Models
{
    public class MixedTouserMassMessage:BaseTouserMassMessage
    {
        public Mpnews mpnews { get; set; }
    }

}
