define(["sitecore"], function (Sitecore) {
    var model = Sitecore.Definitions.Models.ControlModel.extend({
        initialize: function (options) {
            this._super();
            this.set("input", "");
            this.set("output", "");
            this.set("pager", "");
            //this.getusers();
            this.on("change:input", this.getusers(1), this);
        },

        getusers: function (index) {
            var input = this.get("input");

            $.ajax({
                url: "/api/sitecore/UsersList/Find",
                type: "POST",
                data: { input: input, pageIndex: index },
                context: this,
                success: function (data) {
                    this.set("output", data);
                    this.GetPagerStr();
                }
            });
        },

        GetPagerStr: function () {
            $.ajax({
                url: "/api/sitecore/UsersList/GetPagerStr",
                type: "POST",
                data: {},
                context: this,
                success: function (data) {

                    this.set("pager", data);
                }
            });
        },

        setpreview: function (obj) {
            //var input = this.get("input");

            $.ajax({
                url: "/api/sitecore/UsersList/SetPreviewUser",
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
        }
    });

    var view = Sitecore.Definitions.Views.ControlView.extend({
        initialize: function (options) {
            this._super();
        }
    });

    Sitecore.Factories.createComponent("UsersList", model, view, ".sc-UsersList");
});
