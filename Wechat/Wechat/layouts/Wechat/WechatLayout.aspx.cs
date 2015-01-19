using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.Dialogs.MediaBrowser;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XamlSharp.Ajax;

namespace Wechat.layouts.Wechat
{
    using System;
    using System.Web.UI;

    public partial class WechatLayout : Page
    {
        protected override void OnInit(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            Client.AjaxScriptManager.OnExecute += Current_OnExecute;
        }

        private static void Current_OnExecute(object sender, AjaxCommandEventArgs args)
        {
            switch (args.Command.Name)
            {
                case "item:open":
                    //string id = args.Parameters["id"];
                    //Item item = Client.ContentDatabase.GetItem(id);

                    //string sectionID = RootSections.GetSectionID(id);
                    //UrlString str2 = new UrlString();
                    //str2.Append("ro", sectionID);
                    //str2.Append("fo", id);
                    //str2.Append("id", id);
                    //str2.Append("la", item.Language.ToString());
                    //str2.Append("vs", item.Version.ToString());
                    //Windows.RunApplication("Content editor", str2.ToString());
                    break;
                case "wechat:mediabrowser":
                    Item itemNotNull = Client.GetItemNotNull("{3D6658D8-A0BF-4E75-B3E2-D050FABCF4E1}", Language.Parse("zh-cn"));
                    itemNotNull.Fields.ReadAll();
                    Field field = itemNotNull.Fields["img"];
                    Assert.IsNotNull(field, "field");
                    ImageField imageField = new ImageField(field, field.Value);
                    string text = StringUtil.GetString(new string[]
				{
					field.Source,
					"/sitecore/media library"
				});
                    string text2 = imageField.GetAttribute("mediaid");
                    if (text.StartsWith("~", System.StringComparison.InvariantCulture))
                    {
                        if (string.IsNullOrEmpty(text2))
                        {
                            text2 = StringUtil.Mid(text, 1);
                        }
                        text = "/sitecore/media library";
                    }
                    Language language = itemNotNull.Language;
                    MediaBrowserOptions mediaBrowserOptions = new MediaBrowserOptions();
                    Item item = Client.ContentDatabase.GetItem(text, language);
                    if (item == null)
                    {
                        throw new ClientAlertException("The source of this Image field points to an item that does not exist.");
                    }
                    mediaBrowserOptions.Root = item;
                    mediaBrowserOptions.AllowEmpty = true;
                    if (!string.IsNullOrEmpty(text2))
                    {
                        Item item2 = Client.ContentDatabase.GetItem(text2, language);
                        if (item2 != null)
                        {
                            mediaBrowserOptions.SelectedItem = item2;
                        }
                    }
                    SheerResponse.ShowModalDialog(mediaBrowserOptions.ToUrlString().ToString(), "1200px", "700px", string.Empty, true);
                    //args.WaitForPostBack();
                    break;
            }
        }
    }
}
