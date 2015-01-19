define(["sitecore"], function (Sitecore) {
  var SendMessage = Sitecore.Definitions.App.extend({
      initialized: function () {
          //var openid = this.getOpenId();
          //this.getNickname(openid);
          this.LabelMessage.set("text", "发送内容：");
          
      },

    send: function () {
        var app = this;
        var openid = this.getOpenId();
        var property = {
            Message: app.TextAreaMessage.get('text'),
            MessageType: "text",
            OpenId: openid
        };
        
        jQuery.ajax({
            type: "POST",
            url: "/api/sitecore/SendMessage/Send",
            data: { "propertyData": JSON.stringify(property) },
            success: function (data) {
                if (data.success) {
                    app.MessageBar1.addMessage("notification", { text: "发送成功!", actions: [], closable: true });
                } else {
                    app.MessageBar1.addMessage("error", { text: "发送失败!", actions: [], closable: true });
                }
                
                
            },
            error: function () {
                console.log("There was an error. Try again please!");

                app.MessageBar1.addMessage("error", { text: "保存失败!", actions: [], closable: true });
                
            }
        });

    },

    getOpenId: function () {
        var id = Sitecore.Helpers.url.getQueryParameters(window.location.href)['openid'];
        if (id!="") {
            return id;
        }
        return "";
    },

    getNickname: function (id) {
        var app = this;

        jQuery.ajax({
            type: "GET",
            dataType: "json",
            url: "/api/sitecore/SendMessage/GetNickname",
            data: { 'openid': id },
            cache: false,
            success: function (data) {
                //app.TextBoxKeyword.set('text', data.Keyword);
                //app.TextAreaMessage.set('text', data.Message);
                app.LabelMessage.set("text", "与 " + data.nickname + " 的聊天：");
            },
            error: function () {
                console.log("There was an error. Try again please!");
            }
        });
    }
  });

  return SendMessage;
});