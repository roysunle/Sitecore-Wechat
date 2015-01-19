using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.Web;
using Wechat.Services;
using Wechat.Services.Models;
using Wechat.Services.WechatFunctions;

namespace Wechat.sitecore.shell.client.YourApps.Wechat.SendMixedMessage
{
    public class SendMixedMessageController : Controller
    {
        public JsonResult GetUploadedData()
        {
            var input = WebUtil.GetFormValue("input").ToLower().Trim();
            var resultList = new List<MaterialList>();
            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var template = new ID(Constants.MaterialTemplateId.MixedTextMaterial);
            var folder = masterDb.GetItem(Constants.MaterialFolderId.MixedTextMaterialFolder);
            if (folder != null)
            {
                var material = folder.Axes.GetDescendants().Where(o => o.TemplateID == template && o.Fields["MediaId"].Value != "" && (o.Fields["Title"].Value.Contains(input)));
                foreach (var message in material)
                {
                    DateField updateTime = message.Fields["UploadTime"];
                    string updateTimeStr = string.Empty;
                    if (updateTime != null)
                    {
                        if (!string.IsNullOrEmpty(updateTime.Value))
                        {
                            DateTime dt = updateTime.DateTime;
                            updateTimeStr = dt.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            updateTimeStr = "未上传";
                        }

                    }
                    var singleMaterial = new MaterialList()
                    {
                        id = message.ID.ToString(),
                        title = message.Fields["Title"].Value,
                        updateTime = updateTimeStr
                    };
                    resultList.Add(singleMaterial);
                }
            }

            return Json(resultList);

        }

        public JsonResult Send(string id)
        {
            //var result = new ();
            var baseResp = new BaseResponse()
            {
                errcode = "-1",
                errmsg = "发送失败!"
            };

            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var mixedMaterial = masterDb.GetItem(id);
            if (mixedMaterial != null)
            {
                using (new SecurityDisabler())
                {
                    var mixedMessageContainer = CheckFolder();
                    var mixedmessageTypeTemplate = masterDb.GetTemplate(Constants.MessageTemplateId.MixedTextMessage);
                    var message = mixedMessageContainer.Add(DateTime.Now.ToString("HHmmss"), mixedmessageTypeTemplate);
                    if (message != null)
                    {
                        var result = SendMassMessage.Send(mixedMaterial.Fields["MediaId"].Value, "mpnews");
                        if (result.errcode == "0")
                        {
                            message.Editing.BeginEdit();
                            message.Fields["ContentId"].Value = mixedMaterial.Paths.Path;
                            message.Fields["MsgId"].Value = result.msg_id;
                            message.Fields["State"].Value = "Submited";
                            message.Editing.EndEdit();
                            baseResp.errcode = "0";
                            baseResp.errmsg = "发送成功!";

                        }
                        else
                        {
                            message.Editing.BeginEdit();
                            message.Fields["ContentId"].Value = mixedMaterial.Paths.Path;
                            message.Fields["MsgId"].Value = result.errmsg;
                            message.Fields["State"].Value = "Error";
                            message.Editing.EndEdit();
                        }

                    }
                }

            }

            return Json(baseResp);
        }

        public JsonResult Preview(string id)
        {
            //var result = new ();
            var baseResp = new BaseResponse()
            {
                errcode = "-1",
                errmsg = "发送失败!"
            };

            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var mixedMaterial = masterDb.GetItem(id);
            if (mixedMaterial != null)
            {
                using (new SecurityDisabler())
                {
                    var result = SendMassMessage.Preview(mixedMaterial.Fields["MediaId"].Value, "mpnews");
                    if (result.errcode == "0")
                    {
                        baseResp.errcode = "0";
                        baseResp.errmsg = "发送成功!";

                    }
                }

            }

            return Json(baseResp);
        }


        private Item CheckFolder()
        {
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString();
            var day = DateTime.Now.Date.ToString("dd");
            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var messageFolderTemplate = masterDb.GetTemplate(Constants.MessageFolderTemplateId);
            using (new SecurityDisabler())
            {
                var yearFolder =
                masterDb.GetItem(Constants.MassMessageContainerId).Children.FirstOrDefault(o => o.Name == year);
                if (yearFolder == null)
                {
                    yearFolder = masterDb.GetItem(Constants.MassMessageContainerId)
                        .Add(year, messageFolderTemplate);

                }
                var monthFolder = masterDb.GetItem(yearFolder.ID).Children.FirstOrDefault(o => o.Name == month);
                if (monthFolder == null)
                {
                    monthFolder = masterDb.GetItem(yearFolder.ID)
                        .Add(month, messageFolderTemplate);

                }
                var dayFolder = masterDb.GetItem(monthFolder.ID).Children.FirstOrDefault(o => o.Name == day);
                if (dayFolder == null)
                {
                    dayFolder = masterDb.GetItem(monthFolder.ID)
                        .Add(day, messageFolderTemplate);

                }
                return dayFolder;
            }

        }
    }

    


}