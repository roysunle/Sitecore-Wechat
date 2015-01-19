using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.SecurityModel;

namespace Wechat.Services
{
    public class AppService
    {
        
        public static string HttpRequestGet(string url)
        {
            var req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "Get";
            req.Timeout = 120 * 1000;
            using (WebResponse wr = req.GetResponse())
            {
                var strm = wr.GetResponseStream();
                var sr = new StreamReader(strm, Encoding.UTF8);
                string line;
                var sb = new StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                return sb.ToString();
            }
        }

        public static string webRequestPost(string url, string param)
        {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(param);


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "Post";
            req.Timeout = 120 * 1000;
            req.ContentType = "application/x-www-form-urlencoded;";
            req.ContentLength = bs.Length;


            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Flush();
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 


                Stream strm = wr.GetResponseStream();


                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);


                string line;


                System.Text.StringBuilder sb = new System.Text.StringBuilder();


                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                return sb.ToString();
            }
        }

        //判断是否是关键字
        public static string IsKeyword(string str)
        {
            string result = string.Empty;
            //Log.Info("to be checked str:" + str, typeof(AppService));
            var language = Language.Parse("en");
            var coreDb = Sitecore.Configuration.Factory.GetDatabase("core");
            using (new SecurityDisabler())
            {
                var rootItem = coreDb.GetItem(new ID(Constants.ReplyMessageRootId), language);
                //Log.Info("to be checked str:" + rootItem.ID, typeof(AppService));
                var keywordItem = rootItem.Children.FirstOrDefault(o => o.Fields["Keyword"].Value.Equals(str));
                if (keywordItem != null)
                {
                    result = keywordItem.Fields["ReplyMessage"].Value;
                }
            }
            
            return result;
        }

        public static DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
    }
}
