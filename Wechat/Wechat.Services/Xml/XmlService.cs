using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.Diagnostics;

namespace Wechat.Services.Xml
{
    public class XmlService
    {
        private const string RootNodeName = "xml";
        private Dictionary<string, string> _dic;

        public XmlService(Stream stream)
        {
            _dic = GenerateParameterDic(stream);
        }

        private Dictionary<string, string> GenerateParameterDic(Stream stream)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(stream);
            Log.Info("input str:" + xdoc.InnerXml,this);
            var dic = new Dictionary<string, string>();
            XmlNodeList nodeList = xdoc.SelectSingleNode(RootNodeName).ChildNodes;//获取节点的所有子节点
            foreach (XmlNode xn in nodeList)//遍历所有子节点
            {
                XmlElement xe = xn as XmlElement;//将子节点类型转换为XmlElement类型    
                dic.Add(xe.Name, xe.InnerText);

            }
            return dic;
        }

        public string GetParameter(string key)
        {
            var result = string.Empty;
            _dic.TryGetValue(key, out result);
            return result;
        }

        public string CreateXml(int bs, string text)
        {
            string str = "";
            str += "<xml><ToUserName><![CDATA[" + GetParameter("FromUserName") + "]]></ToUserName><FromUserName><![CDATA[" + GetParameter("ToUserName") + "]]></FromUserName><CreateTime>" + DateTime.Now.Ticks.ToString() + "</CreateTime>";
            if (bs == 0)
            { //回复文本信息

                str += "<MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + text + "]]></Content>";


            }
            else if (bs == 1)
            { //回复图片消息n b
                str += " <MsgType><![CDATA[image]]></MsgType><Image><MediaId><![CDATA[1111]]></MediaId></Image>";
            }
            
            str += "</xml>";

            return str;
        }
    }
}
