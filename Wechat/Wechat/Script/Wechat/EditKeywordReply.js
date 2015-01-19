define(["sitecore"], function (Sitecore) {
  var EditKeywordReply = Sitecore.Definitions.App.extend({
      initialized: function () {
          
          var id = this.getID();
          if (id != null) {
              
              console.log('ID processed');
              this.getProperty(id);
          } else {
              
              console.log('ID not processed');
          }

    },

    save: function () {
        var app = this;
        var id = this.getID();
        //var content = app.AutoReplyText.get('text');
        var property = {
            Keyword: app.TextBoxKeyword.get('text'),
            Message: app.TextAreaMessage.get('text'),
            Id: id

        };
        //alert(id);
        jQuery.ajax({
            type: "POST",
            url: "/api/sitecore/Keyword/Save",
            data: { "propertyData": JSON.stringify(property) },
            success: function (success) {
                //alert("succeed!!!");

                app.MessageBar1.addMessage("notification", { text: "保存成功!", actions: [], closable: true });
                //app.MessageBar1.viewModel.$el.delay(2000).fadeOut();

            },
            error: function () {
                console.log("There was an error. Try again please!");

                app.MessageBar1.addMessage("error", { text: "保存失败!", actions: [], closable: true });
                //app.MessageBar1.viewModel.$el.delay(2000).fadeOut();
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

    getProperty: function (id) {
        var app = this;
        //alert(id);
        jQuery.ajax({
            type: "GET",
            dataType: "text",
            url: "/api/sitecore/Keyword/GetProperty",
            data: "id="+id,
            cache: false,
            success: function (data) {
                var dataObj = eval("(" + data + ")");
                app.TextBoxKeyword.set('text', dataObj.Keyword);
                app.TextAreaMessage.set('text', dataObj.Message);

            },
            error: function () {
                console.log("There was an error. Try again please!");
            }
        });
    },


  });

  return EditKeywordReply;
});