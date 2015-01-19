using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Search;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using Sitecore.Tasks;
using Wechat.Services;
using Wechat.Services.AccessToken;
using Wechat.Services.Models;
using Wechat.Services.WechatFunctions;

namespace Wechat.Tasks
{
    public class SendMassMessage
    {
        
        public void Test()
        {
            Log.Info(" ======================================= SendMassMessage START =============================== ", this);

            //1. 获取所有待发群发消息
            var messages = GetMessagesToSend();
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    try
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
                                    title = "test message",
                                    content = message.Fields["MixedContent"].Value,
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
                                    UpdateState(message, "Sent");
                                }
                                else
                                {
                                    Log.Error("when send mass message:" + message.ID.ToString() + " error:" + resp.errcode + "|" + resp.errmsg, this);
                                    UpdateState(message, "Error");
                                }
                            }
                            else
                            {
                                Log.Error("when send mass message:" + message.ID.ToString() + " upload media error:" + uploadResult.errcode + "|" + uploadResult.errmsg, this);
                                UpdateState(message, "Error");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("send mass message:" + ex.Message, this);
                        UpdateState(message, "Error");
                        //throw;
                    }


                }
            }
            Log.Info(" ======================================= SendMassMessage END =============================== ", this);
        }

        public void Execute(Item[] itemArray, CommandItem commandItem, ScheduleItem scheduledItem)
        {
            Log.Info(" ======================================= SendMassMessage START =============================== ", this);
            string accesstokenstr = AccessTokenService.CreateInstance().GetAccessToken();
            //1. 获取所有待发群发消息
            var messages = GetMessagesToSend();
            if (messages!=null)
            {
                foreach (var message in messages)
                {
                    try
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
                                    title = "test message",
                                    content = message.Fields["MixedContent"].Value,
                                    show_cover_pic = "1"
                                };
                                var articleStr = JsonConvert.SerializeObject(article);
                                Log.Info("articleStr:" + articleStr, this);
                                articleStr = "{\"articles\": [" + articleStr + "]}";
                                var url = "https://api.weixin.qq.com/cgi-bin/media/uploadnews?access_token=" + accesstokenstr;
                                var result = AppService.webRequestPost(url, articleStr);
                                Log.Info("send mass message result:" + result, this);
                                var resp = (UploadMediaResp)JsonConvert.DeserializeObject(result, typeof(UploadMediaResp));
                                if (!string.IsNullOrEmpty(resp.media_id))
                                {
                                    UpdateState(message, "Sent");
                                }
                                else
                                {
                                    Log.Error("when send mass message:" + message.ID.ToString() + " error:" + resp.errcode + "|" + resp.errmsg, this);
                                    UpdateState(message, "Error");
                                }
                            }
                            else
                            {
                                Log.Error("when send mass message:" + message.ID.ToString() + " upload media error:" + uploadResult.errcode + "|" + uploadResult.errmsg, this);
                                UpdateState(message, "Error");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("send mass message:"+ex.Message,this);
                        UpdateState(message, "Error");
                        //throw;
                    }
                    
                    
                }
            }


            //var massMessage =
            //    Sitecore.Configuration.Factory.GetDatabase("master").GetItem("{F3A4639A-268D-484E-AC5F-39047C292AFE}");
            //if (massMessage != null)
            //{
            //    var content = massMessage.Fields["MixedContent"].Value;
            //    ImageField imageField = massMessage.Fields["Thumb"];
            //    if (imageField != null && imageField.MediaItem != null)
            //    {
            //        MediaItem image = new MediaItem(imageField.MediaItem);
            //        UploadMedia.UploadMediaFile("image", image);
            //    }
            //}


            Log.Info(" ======================================= SendMassMessage END =============================== ", this);
        }

        private List<Item> GetMessagesToSend()
        {
            var masterDB = Sitecore.Configuration.Factory.GetDatabase("master");
            List<Item> messages = null;
            var mixedMessageTypeTemplateId = new ID(Constants.MixedMessageTypeTemplateId);
            messages = masterDB.GetItem(Constants.MassMessageContainerId)
                    .Axes.GetDescendants()
                    .Where(o => o.TemplateID == mixedMessageTypeTemplateId && o.Fields["State"].Value == "PreSend").ToList();
            return messages;
        }

        private void UpdateState(Item item , string state)
        {
            using (new SecurityDisabler())
            {
                if (item != null)
                {
                    item.Editing.BeginEdit();
                    item.Fields["State"].Value = state;
                    item.Editing.EndEdit();

                }
            }
            
        }
    }
}
