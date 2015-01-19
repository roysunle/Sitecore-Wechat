<%@ Page validateRequest="false" language="c#" Codepage="65001" AutoEventWireup="true" Inherits="Wechat.layouts.Wechat.WechatLayout" CodeBehind="WechatLayout.aspx.cs" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.HtmlControls" Assembly="Sitecore.Kernel" %>
<%@ OutputCache Location="None" VaryByParam="none" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1"  runat="server">
    <title>Prototype Dialog</title>
 
    <sc:Script ID="Script1" Src="/sitecore/shell/Controls/Sitecore.js" runat="server" />
 
    <script type="text/javascript" language="javascript">
        function btnClickMe_onClick() {
            var id = document.getElementById("ItemID").value;
            //scForm.postRequest("", "", "", "item:open(id=" + id + ")");
            scForm.postRequest("", "", "", "item:open");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">     
 
      <sc:AjaxScriptManager ID="AjaxScriptManager1" runat="server"/>
      
      <input type="text" id="ItemID" />
      <asp:Button ID="btnClickMe" runat="server"
                  Text="Click Me!"
                   OnClientClick="btnClickMe_onClick()" />
    </form>
</body>
</html>