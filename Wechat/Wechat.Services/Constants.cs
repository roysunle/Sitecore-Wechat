using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.Services
{
    public class Constants
    {
        //微信应用
        //public static string WechatAppId = "{BF49C333-13F9-4C37-B513-94747999D992}";  //core db
        public static string WechatAppId = "{081FA81B-83D2-4035-BDE5-9B4885E10E9B}";    //master db

        //AccessToken
        public static string AccessTokenId = "{DE6948CF-D217-4302-9DFD-2227D47F0D9C}";

        //public static string ReplyMessageRootId = "{CE0D49A8-6103-4EBB-B33A-F89E33430454}";    //core db
        public static string ReplyMessageRootId = "{1FD85AAB-ADC8-4F98-B83F-59262A69C92D}";  //master db
        public static string ReplyMessageTemplateId = "{13A8F2C6-43F7-44D7-BB0F-8344C1D984D0}";

        //text message type
        public static string TextMessageTypeTemplateId = "{63D9D9DD-6F13-4629-A2F5-7BDE89501B96}";

        //mixed messgage type
        public static string MixedMessageTypeTemplateId = "{94F73D0F-4ABF-43FF-8D89-1F62CE66D9AF}";


        //message folder
        public static string MessageFolderTemplateId = "{093B6A4E-4B1E-4DF1-9331-EF742271F763}";

        //mass message container
        public static string MassMessageContainerId = "{4DDE77F5-2D94-4A30-88AF-12DE25AB44C8}";

        public class MaterialFolderId
        {
            public static string ImageMaterialFolder = "{39F87A02-2B6A-4219-8CA2-AD388A3A7300}";
            public static string MixedTextMaterialFolder = "{CA677974-A1F9-44DF-9F0C-8BF4B28134D4}";
            public static string MultiMediaMaterialFolder = "{C55078B0-F862-43A1-8DF1-ED7C1DB707E6}";
        }

        public class MaterialTemplateId
        {
            public static string ImageMaterial = "{7A1AEB9E-DF14-476A-84A4-91026B815951}";
            public static string MixedTextMaterial = "{2A3A63DE-69BF-4576-8163-B6D85365577B}";
            public static string MultiMediaMaterial = "{22A46233-0075-4742-BA6D-7C4B893B48AF}";
        }

        public class MessageTemplateId
        {
            public static string ImageMessage = "{7A1AEB9E-DF14-476A-84A4-91026B815951}";
            public static string MixedTextMessage = "{94F73D0F-4ABF-43FF-8D89-1F62CE66D9AF}";
            public static string MultiMediaMessage = "{22A46233-0075-4742-BA6D-7C4B893B48AF}";
        }

        
    }
}
