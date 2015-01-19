using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Mvc.Controllers;
using Sitecore.SecurityModel;
using Sitecore.Web.UI.WebControls;
using Wechat.Models.Wechat;
using Wechat.Services;

namespace Wechat.Controllers.Wechat
{
    public class KeywordController : SitecoreController
    {
        public ActionResult Save(string propertyData)
        {
            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            //var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var language = Language.Parse("en");
            
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var property = jss.Deserialize<KeywordReply>(propertyData);
            
            using (new SecurityDisabler())
            {
                var rootItem = masterDb.GetItem(new ID(Constants.ReplyMessageRootId), language);
                using (new LanguageSwitcher("en"))
                {
                    Item editItem = null;
                    //Boolean isExist = false;
                    if (!string.IsNullOrEmpty(property.Id))
                    {
                        editItem = masterDb.GetItem(new ID(property.Id));
                        //isExist = true;
                    }
                    else
                    {

                        editItem = rootItem.Children.FirstOrDefault(o => o.Fields["Keyword"].Value.Equals(property.Keyword));
                        if (editItem == null)
                        {
                            //isExist = true;
                            TemplateItem template = masterDb.GetTemplate(Constants.ReplyMessageTemplateId);
                            //TemplateItem template = masterDb.GetTemplate("{11BCCE0B-A952-4928-AD89-91699CF11B67}");
                            editItem = rootItem.Add("KeywordReply_" + DateTime.Now.ToString("yyyyMMddHHmmss"), template);


                        }

                    }

                    editItem.Editing.BeginEdit();
                    editItem.Fields["Keyword"].Value = property.Keyword;
                    editItem.Fields["ReplyMessage"].Value = property.Message;
                    editItem.Editing.EndEdit();
                }

                //Sitecore.Search.SearchManager.GetIndex("Sitecore.ContentSearch.Lucene.Index.Core").Rebuild();
                //IndexCustodian.FullRebuild(ContentSearchManager.GetIndex(“[INDEX NAME]”, true);

            }
            //return View("/Views/Test/Index.cshtml");
            //Sitecore.Data.Fields.ReferenceField
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProperty(Guid id)
        {
            KeywordReply property = new KeywordReply();

            if (ID.IsID(id.ToString()))
            {
                var db = Sitecore.Configuration.Factory.GetDatabase("master");

                if (db != null)
                {
                    var propertyItem = db.GetItem(new ID(id));
                    property.Keyword = propertyItem.Fields["Keyword"].Value;
                    property.Message = propertyItem.Fields["ReplyMessage"].Value;
                    //property.Keyword = FieldRenderer.Render(propertyItem, "Keyword");
                    //property.Message = FieldRenderer.Render(propertyItem, "ReplyMessage");
                }
            }
            Sitecore.Context.SetLanguage(Language.Parse("en"),true);
            return Json(property, JsonRequestBehavior.AllowGet);
        }

        
    }

    partial class KeywordReply
    {
        public string Keyword { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
    }

    

}