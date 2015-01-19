using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Web;
using Wechat.Controllers.Wechat;
using Wechat.Services;
using Wechat.Services.Models;

namespace Wechat.sitecore.shell.client.YourApps.Wechat.MaterialManagement
{
    public class MaterialListController : Controller 
    {
        public JsonResult GetData()
        {
            var input = WebUtil.GetFormValue("input").ToLower().Trim();
            
            var resultList = new List<MaterialList>();
            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var template = new ID(Constants.MaterialTemplateId.MixedTextMaterial);
            var folder = masterDb.GetItem(Constants.MaterialFolderId.MixedTextMaterialFolder);
            if (folder != null)
            {
                var material = folder.Axes.GetDescendants().Where(o => o.TemplateID == template && (o.Fields["Title"].Value.Contains(input)));
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

            for (int i = 1; i < Math.Ceiling((double)resultList.Count / 2); i++)
            {
                var index = new MyPager();
                index.number = i;
                PagerList.Add(index);
            }

            return Json(resultList);

        }

        public JsonResult GetUploadedData()
        {
            var input = WebUtil.GetFormValue("input").ToLower().Trim();
            var pageIndex = int.Parse(WebUtil.GetFormValue("pageIndex").ToLower().Trim());
            var resultList = new List<MaterialList>();
            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var template = new ID(Constants.MaterialTemplateId.MixedTextMaterial);
            var folder = masterDb.GetItem(Constants.MaterialFolderId.MixedTextMaterialFolder);
            if (folder != null)
            {
                var material = folder.Axes.GetDescendants().Where(o => o.TemplateID == template && o.Fields["MediaId"].Value != "" && (o.Fields["Title"].Value.Contains(input)));
                var materialPaged = material.Skip((pageIndex - 1)*pageSize).Take(pageSize);
                foreach (var message in materialPaged)
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
                PagerList = new List<MyPager>();
                for (int i = 1; i <= Math.Ceiling((double)material.Count() / pageSize); i++)
                {
                    var index = new MyPager();
                    index.number = i;
                    PagerList.Add(index);
                }
            }
            

            return Json(resultList);

        }

        public JsonResult GetPagerStr()
        {
            return Json(PagerList);
        }

        public static List<MyPager> PagerList;
        private static int pageSize = 10;
    }

    //public class MyPager
    //{
    //    public int number { set; get; }
    //}
}