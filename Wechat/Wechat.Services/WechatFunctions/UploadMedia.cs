using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Wechat.Services.AccessToken;
using Wechat.Services.Models;

namespace Wechat.Services.WechatFunctions
{
    public class UploadMedia
    {
        
        public static UploadMediaResp UploadMediaFile(string type, MediaItem media)
        {
            string accesstokenstr = AccessTokenService.CreateInstance().GetAccessToken();
            //var result = false;
            //string filename = media;//System.Web.HttpContext.Current.Server.MapPath("/Images/carrot1.jpg");
            string url = "http://file.api.weixin.qq.com/cgi-bin/media/upload?access_token=" + accesstokenstr + "&type=" + type;
            var resultStr = HttpUploadFile(url, media);
            var resp = (UploadMediaResp)JsonConvert.DeserializeObject(resultStr, typeof(UploadMediaResp));
            //Log.Info("created at:" + DateTime.Parse(resp.created_at).ToString("yyyy-MM-dd hh:mm:ss"), typeof(UploadMedia));
            Log.Info("upload image result :" + resultStr, typeof(UploadMedia)); 
            //Log.Info("upload image result :" + HttpUploadFile(url, @"D:\inetpub\wwwroot\Wechat\Website\App_Data\MediaCache\scheduler\238\wcf to create user result.jpg"), typeof(UploadMedia)); 
            return resp;
        }

        private static string GetContentType(FileInfo fileInfo)
        {
            var contentType = "";
            switch (fileInfo.Extension.ToLower())
            {
                case ".jpg":
                    contentType = "image/jpeg"; //image/jpeg
                    break;
                case ".mp3":
                    contentType = "audio/mp3";
                    break;
                case ".amr":
                    contentType = "audio/amr";
                    break;
                case ".mp4":
                    contentType = "video/mp4";
                    break;
                default:
                    throw new NotSupportedException("文件格式不支持");
            }

            return contentType;
        }

        public static string HttpUploadFile(string url, MediaItem file)
        {
            //判断文件是否存在
            //if (!File.Exists(file))
            //{
            //    throw new FileNotFoundException();
            //}

            //FileInfo fileInfo = new FileInfo(file);
            string filePath;
            if (string.IsNullOrEmpty(file.FilePath))
            {
                filePath = System.Web.HttpRuntime.AppDomainAppPath + file.Name + "." + file.Extension;
            }
            else
            {
                filePath = System.Web.HttpRuntime.AppDomainAppPath + (file.FilePath.Replace('/', '\\').Substring(1));
            }
             
            //构造数据包前半段
            string result = string.Empty;
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            var stream = request.GetRequestStream();
            stream.Write(boundarybytes, 0, boundarybytes.Length);
            //将文件写进流
            var headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            //var header = string.Format(headerTemplate, file.Name, file, GetContentType(fileInfo));
            var header = string.Format(headerTemplate, file.Name, filePath, file.MimeType);
            //Log.Info("image file type:" + file.MimeType, typeof(UploadMedia));
            var headerbytes = Encoding.UTF8.GetBytes(header);
            stream.Write(headerbytes, 0, headerbytes.Length);

            try
            {
                if (file.HasMediaStream("blob"))
                {
                    Stream s = file.GetMediaStream();
                    Byte[] info = new Byte[s.Length];
                    s.Read(info, 0, Convert.ToInt32(s.Length));
                    s.Close();
                    stream.Write(info, 0, info.Length);
                    //FileStream fs = new FileStream(physicalPath, FileMode.CreateNew);
                }
                else
                {
                    
                    if (!string.IsNullOrEmpty(file.FilePath))
                    {
                        //Log.Info("file path:" + System.Web.HttpRuntime.AppDomainAppPath + (file.FilePath.Replace('/','\\').Substring(1)), typeof(UploadMedia));
                        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        var buffer = new byte[4096];
                        var bytesRead = 0;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                        fileStream.Close();
                    }
                }
                //stream.CopyTo(file.GetMediaStream());
            }
            catch (Exception ex)
            {
                //FileInfo fileInfo = new FileInfo(file);
                Log.Error("exception:"+ex.Message, typeof(UploadMedia));

                //throw;
            }
            //fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);

            //封闭数据包并发送
            var trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            stream.Write(trailer, 0, trailer.Length);
            stream.Close();
            //发送并显示返回结果
            WebResponse wresp = null;
            try
            {
                wresp = request.GetResponse();

                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);

                result = reader2.ReadToEnd();
            }
            finally
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }

            return result;
        }

        public static string HttpUploadFile(string url, string file)
        {   //判断文件是否存在
            if (!File.Exists(file))
            {
                throw new FileNotFoundException();
            }


            FileInfo fileInfo = new FileInfo(file);

            //构造数据包前半段
            string result = string.Empty;
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            var stream = request.GetRequestStream();
            stream.Write(boundarybytes, 0, boundarybytes.Length);
            //将文件写进流
            var headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            var header = string.Format(headerTemplate, fileInfo.Name, file, GetContentType(fileInfo));
            var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            stream.Write(headerbytes, 0, headerbytes.Length);

            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var buffer = new byte[4096];
            var bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                stream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
            //封闭数据包并发送
            var trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            stream.Write(trailer, 0, trailer.Length);
            stream.Close();
            //发送并显示返回结果
            WebResponse wresp = null;
            try
            {
                wresp = request.GetResponse();

                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);

                result = reader2.ReadToEnd();
            }
            finally
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }

            return result;
        }
    }
}
