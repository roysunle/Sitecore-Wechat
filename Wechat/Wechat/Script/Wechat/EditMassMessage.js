define(["sitecore"], function (Sitecore) {
  var EditMassMessage = Sitecore.Definitions.App.extend({
      initialized: function () {
          var id = Sitecore.Helpers.url.getQueryParameters(window.location.href)['id'];
          this.Frame1.set("sourceUrl", "/?sc_itemid="+id+"&sc_mode=edit");
          
      },

      save: function () {
          var app = this;
          var id = this.getID();
          
          jQuery.ajax({
              type: "POST",
              url: "/api/sitecore/EditMixedMessage/Save",
              data: "id="+id,
              dataType: "text",
              success: function (data) {
                  var dataObj = eval("(" + data + ")");
                  if (dataObj.errcode == "0") {
                      app.MessageBar1.addMessage("notification", { text: "上传成功!", actions: [], closable: true });
                  } else {
                      app.MessageBar1.addMessage("error", { text: dataObj.errmsg, actions: [], closable: true });
                  }
              },
              error: function () {
                  console.log("There was an error. Try again please!");

                  app.MessageBar1.addMessage("error", { text: "上传失败!", actions: [], closable: true });

              }
          });

      },

      getID: function () {
          var id = Sitecore.Helpers.url.getQueryParameters(window.location.href)['id'];
          if (Sitecore.Helpers.id.isId(id)) {
              return id;
          }
          return "";
      },
  });

  return EditMassMessage;
});

$('.sc-dialogWindow-close').click(function() {
    location.reload();
})