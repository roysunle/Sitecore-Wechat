using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Controllers;
using Sitecore.SecurityModel;
using Sitecore.Web.UI.WebControls;
using Wechat.Services;
using Wechat.Services.Signature;
using Wechat.Services.Xml;

namespace Wechat.Controllers.Wechat
{
    public class SignatureController : SitecoreController
    {
        [HttpGet]
        public string Signature(string signature, string timestamp, string nonce, string echostr)
        {
            
            var echoStr = echostr;
            if (SignatureService.CheckSignature(signature.ToUpper(), timestamp, nonce))
            {
                return echoStr;
            }
            return string.Empty;
        }

        [HttpPost]
        public string Signature()
        {
            XmlService xs = new XmlService(Request.InputStream);
            var resultXml = string.Empty;
            string msgType = xs.GetParameter("MsgType");

            switch (msgType)
            {
                case "text":
                    var message = AppService.IsKeyword(xs.GetParameter("Content"));
                    if (!string.IsNullOrEmpty(message))
                    {
                        resultXml = xs.CreateXml(0, message);
                    }
                    break;
                case "event":
                    string events = xs.GetParameter("Event");
                    switch (events)
                    {
                        case "subscribe":
                            var replyMessage = "感谢您的关注！--send from Sitecore Wechat Module";
                            var CoreDB = Sitecore.Configuration.Factory.GetDatabase("core");
                            using (new SecurityDisabler())
                            {
                                var item = CoreDB.GetItem(new ID("{A430A591-B58F-4AF1-BF35-769413B09D71}"));
                                if (item != null)
                                {
                                    replyMessage = item.Fields["Text"].Value;
                                }
                                else
                                {
                                    Log.Info("itme not found", this);
                                }
                            }

                            resultXml = xs.CreateXml(0, replyMessage);
                            break;
                        case "MASSSENDJOBFINISH":
                        default: break;

                    }
                    break;
                default:
                    break;
            }


            Log.Info("reply message:" + resultXml,this);
            return resultXml;

        }



    }
}