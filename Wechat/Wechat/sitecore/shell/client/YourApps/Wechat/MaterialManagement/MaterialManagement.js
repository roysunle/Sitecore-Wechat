define(["sitecore"], function (Sitecore) {
    var model = Sitecore.Definitions.Models.ControlModel.extend({
        initialize: function (options) {
            this._super();

            this.set("input", "");
            this.set("output", "");
            this.set("pager", "");
            this.on("change:input", this.getData(), this);
        },

        getData: function () {
            var input = this.get("input");

            $.ajax({
                url: "/api/sitecore/MaterialList/GetUploadedData",
                type: "POST",
                data: { input: input, pageIndex: 1 },
                context: this,
                success: function (data) {
                    this.set("output", data);
                }
            });

            $.ajax({
                url: "/api/sitecore/MaterialList/GetPagerStr",
                type: "POST",
                data: {},
                context: this,
                success: function (data) {
                    
                    this.set("pager", data);
                }
            });

        },

        updateData: function (index) {
            var input = this.get("input");
            
            $.ajax({
                url: "/api/sitecore/MaterialList/GetUploadedData",
                type: "POST",
                data: { input: input, pageIndex: index },
                context: this,
                success: function (data) {
                    this.set("output", data);
                }
            });

            $.ajax({
                url: "/api/sitecore/MaterialList/GetPagerStr",
                type: "POST",
                data: {},
                context: this,
                success: function (data) {
                    this.set("pager", data);
                }
            });

        },

        send: function (obj) {
            //alert("succeed!" + obj.value);
            $.ajax({
                url: "/api/sitecore/MaterialList/Send",
                type: "POST",
                data: "id=" + obj.value,

                context: this,
                success: function (data) {
                    //alert(data.errcode);
                    //this.set("output", data);
                    //var dataObj = eval("(" + data + ")");
                    //alert(dataObj);
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

    Sitecore.Factories.createComponent("MaterialManagement", model, view, ".sc-MaterialManagement");

});
