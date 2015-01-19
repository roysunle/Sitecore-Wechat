define(["sitecore", "jquery"], function (Sitecore, jQuery) {
  var Wechat = Sitecore.Definitions.App.extend({
      initialized: function () {
          //console.log('I am running.')
          
      },

      saveProperty: function () {
          var app = this;
          
          //var content = app.AutoReplyText.get('text');
          var property = {
              Content: app.AutoReplyText.get('text'),
              //Id: id

          };
          //alert(id);
          jQuery.ajax({
              type: "POST",
              url: "/api/sitecore/Process/SaveProperty",
              data: { "propertyData": JSON.stringify(property) },
              success: function (success) {
                  //alert("succeed!!!");
                  
                  app.MessageBar1.addMessage("notification", { text: "保存成功!",actions:[],closable:true });
                  //app.MessageBar1.viewModel.$el.delay(2000).fadeOut();
                  
              },
              error: function () {
                  console.log("There was an error. Try again please!");
                  
                  app.MessageBar1.addMessage("error", { text: "保存失败!", actions: [], closable: true });
                  //app.MessageBar1.viewModel.$el.delay(2000).fadeOut();
              }
          });

      },

      



  });

  return Wechat;
});

