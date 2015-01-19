using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Controllers;
using Sitecore.SecurityModel;
using Sitecore.Sites;
using Wechat.Services;
using Wechat.Services.AccessToken;
using Wechat.Services.Models;
using Wechat.Services.WechatFunctions;

namespace Wechat.Controllers.Wechat
{
    public class EditMixedMessageController : SitecoreController
    {
        //
        // GET: /EditMixedMessage/
        public ActionResult Edit(string id)
        {
            Item item;
            if (string.IsNullOrEmpty(id))
            {
                item = Sitecore.Context.Item;
            }
            else
            {
                item = Sitecore.Configuration.Factory.GetDatabase("master").GetItem(id);
            }
            return View("~/Views/Wechat/SubLayouts/MixedMessage.cshtml", item);
        }

        
        public ActionResult Save(string id)
        {
            
            var baseResp = new BaseResponse()
            {
                errcode = "-1",
                errmsg = "上传失败!"
            };
            using (new SecurityDisabler())
            {
                var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
                var message = masterDb.GetItem(id);
                if (message != null)
                {

                    try
                    {
                        if (string.IsNullOrEmpty(message.Fields["Title"].Value))
                        {
                            baseResp.errcode = "-1";
                            baseResp.errmsg = "请填写标题！";
                        }
                        else if (string.IsNullOrEmpty(message.Fields["MixedContent"].Value))
                        {
                            baseResp.errcode = "-1";
                            baseResp.errmsg = "请填写文章内容！";
                        }
                        else
                        {
                            //上传thumb media，获取media id
                            ImageField imageField = message.Fields["Thumb"];
                            if (imageField != null && imageField.MediaItem != null)
                            {
                                MediaItem image = new MediaItem(imageField.MediaItem);
                                var uploadResult = UploadMedia.UploadMediaFile("image", image);
                                if (!string.IsNullOrEmpty(uploadResult.media_id))
                                {
                                    var article = new Article()
                                    {
                                        thumb_media_id = uploadResult.media_id,
                                        title = message.Fields["Title"].Value,
                                        //检查内容中如果含有图片，则先将图片发布到web数据库，然后修改src路径为绝对路径
                                        content = ModifyContent(message.Fields["MixedContent"].Value),
                                        //content = (message.Fields["MixedContent"].Value),
                                        show_cover_pic = "1"
                                    };

                                    var articleStr = JsonConvert.SerializeObject(article);
                                    Log.Info("articleStr:" + articleStr, this);
                                    articleStr = "{\"articles\": [" + articleStr + "]}";
                                    string accesstokenstr = AccessTokenService.CreateInstance().GetAccessToken();
                                    var url = "https://api.weixin.qq.com/cgi-bin/media/uploadnews?access_token=" + accesstokenstr;
                                    var result = AppService.webRequestPost(url, articleStr);
                                    Log.Info("send mass message result:" + result, this);
                                    var resp = (UploadMediaResp)JsonConvert.DeserializeObject(result, typeof(UploadMediaResp));
                                    if (!string.IsNullOrEmpty(resp.media_id))
                                    {
                                        //UpdateState(message, "Sent");
                                        message.Editing.BeginEdit();
                                        message.Fields["UploadTime"].Value = DateTime.Now.ToString("yyyyMMddTHHmmss");
                                        message.Fields["MediaId"].Value = resp.media_id;
                                        message.Editing.EndEdit();
                                        baseResp.errcode = "0";
                                        //return true;
                                    }
                                    else
                                    {
                                        Log.Error("when send mass message:" + message.ID.ToString() + " error:" + resp.errcode + "|" + resp.errmsg, this);
                                        baseResp.errcode = "-1";

                                        //UpdateState(message, "Error");
                                    }
                                }
                                else
                                {
                                    Log.Error("when send mass message:" + message.ID.ToString() + " upload media error:" + uploadResult.errcode + "|" + uploadResult.errmsg, this);
                                    baseResp.errcode = "-1";
                                    //UpdateState(message, "Error");
                                }
                            }
                            else
                            {
                                baseResp.errcode = "-1";
                                baseResp.errmsg = "请添加缩略图！";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("send mass message:" + ex.Message, this);
                        baseResp.errcode = "-1";
                        //UpdateState(message, "Error");
                        //throw;
                    }

                    
                }
                return Json(baseResp);
            }
        }

        //发布图片item，修改img标签中src内容，添加host name
        private string ModifyContent(string content)
        {
            var newContent = content;
            if (content.Contains("<img") && content.Contains("src="))
            {
                var s = "src=\"";
                var e = "\"";
                Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                var src = rg.Match(content).Value;
                if (!string.IsNullOrEmpty(src))
                {
                    var medidItemId = new ID(src.Substring(src.LastIndexOfAny(new char[] { '/' })+1, 32));
                    var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
                    var mediaItem = masterDb.GetItem(medidItemId);
                    if (mediaItem!=null)
                    {
                        //publish to web db
                        using (new SecurityDisabler())
                        {
                            try
                            {
                                Database target = Sitecore.Configuration.Factory.GetDatabase("web");
                                Database[] targetDatabases = { target };
                                Sitecore.Globalization.Language[] languages = masterDb.Languages;
                                bool deep = true;
                                bool compareRevisions = true;
                                Sitecore.Publishing.PublishManager.PublishItem(mediaItem, targetDatabases, languages, deep, compareRevisions);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex.Message, this);

                            }

                            newContent = content.Replace(src, "http://" + Request.Url.Host + "/" + src);
                        }
                    }
                }
                
            }
            Log.Info("new content:"+newContent,this);
            return newContent;
        }
    }
}