define(["sitecore"], function (Sitecore) {
  var model = Sitecore.Definitions.Models.ControlModel.extend({
      initialize: function (options) {
          this._super();
          
        this.set("input", "");
        this.set("output", "");
        this.getData();
        this.on("change:input", this.getData, this);
    },

    getData: function () {
        var input = this.get("input");
        
        $.ajax({
            url: "/api/sitecore/MaterialList/GetUploadedData",
            type: "POST",
            data: { input: input },
            context: this,
            success: function (data) {
                this.set("output", data);
            }
        });
    },

      send: function(obj) {
          //alert("succeed!" + obj.value);
          $.ajax({
              url: "/api/sitecore/SendMixedMessage/Send",
              type: "POST",
              data: "id=" + obj.value,
              //data: { id: obj.value },
              context: this,
              success: function (data) {
                  if (data.errcode == "0") {
                      Sitecore.app.MessageBar1.addMessage("notification", { text: data.errmsg, actions: [], closable: true });
                  } else {
                      Sitecore.app.MessageBar1.addMessage("error", { text: data.errmsg, actions: [], closable: true });
                  }
              }
          });
      },

      preview: function (obj) {
          //alert("succeed!" + obj.value);
          $.ajax({
              url: "/api/sitecore/SendMixedMessage/Preview",
              type: "POST",
              data: "id=" + obj.value,
              //data: { id: obj.value },
              context: this,
              success: function (data) {
                  if (data.errcode == "0") {
                      Sitecore.app.MessageBar1.addMessage("notification", { text: data.errmsg, actions: [], closable: true });
                  } else {
                      Sitecore.app.MessageBar1.addMessage("error", { text: data.errmsg, actions: [], closable: true });
                  }
              }
          });
      },

  });
    
    var view = Sitecore.Definitions.Views.ControlView.extend({
    initialize: function (options) {
      this._super();
    }
  });

    Sitecore.Factories.createComponent("SendMixedMessage", model, view, ".sc-SendMixedMessage");

});
