using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Query;
using Sitecore.Links;
using Sitecore.Mvc.Controllers;
using Sitecore.SecurityModel;
using Wechat.Services;

namespace Wechat.Controllers.Wechat
{
    public class MaterialManagementController : SitecoreController
    {
        //
        // GET: /MaterialManagement/
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult AddMessage(string propertyData)
        {
            var result = new Result(){isSucceed = false,url = ""};
            using (new SecurityDisabler())
            {
                var rootItem = CheckFolder();
                //masterDb.GetItem(Constants.MaterialFolderId.MixedTextMaterialFolder)
                if (rootItem != null)
                {
                    var messageTypeTemplate = masterDb.GetTemplate(Constants.MaterialTemplateId.MixedTextMaterial);
                    var message = rootItem.Add(DateTime.Now.ToString("HHmmss"), messageTypeTemplate);
                    if (message != null)
                    {
                        result.isSucceed = true;
                        //result.url = LinkManager.GetItemUrl(message) + "?sc_mode=edit";
                        result.url = message.ID.ToString();
                    }
                }
            }
            return Json(result);
        }
        private Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
        private Item CheckFolder()
        {
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString();
            var day = DateTime.Now.Date.ToString("dd");
            var messageFolderTemplate = masterDb.GetTemplate(Constants.MessageFolderTemplateId);
            using (new SecurityDisabler())
            {
                var yearFolder =
                masterDb.GetItem(Constants.MaterialFolderId.MixedTextMaterialFolder).Children.FirstOrDefault(o => o.Name == year);
                if (yearFolder == null)
                {
                    yearFolder = masterDb.GetItem(Constants.MaterialFolderId.MixedTextMaterialFolder)
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

    public class Result
    {
        public Boolean isSucceed { get; set; }
        public string url { get; set; }
    }
}