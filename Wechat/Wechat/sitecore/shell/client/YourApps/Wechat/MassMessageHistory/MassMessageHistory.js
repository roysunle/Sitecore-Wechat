define(["sitecore"], function (Sitecore) {
    var model = Sitecore.Definitions.Models.ControlModel.extend({
        initialize: function (options) {
            this._super();

            this.set("input", "");
            this.set("output", "");
            this.set("pager", "");
            //this.getData(1);
            this.on("change:input", this.getData(1), this);
        },

        getData: function (index) {
            var input = this.get("input");
            
            $.ajax({
                url: "/api/sitecore/MassMessageHistory/GetData",
                type: "POST",
                asyn:false,
                data: { input: input, pageIndex: index },
                context: this,
                success: function (data) {
                    this.set("output", data);
                    this.GetPagerStr();
                }
            });

            
        },

        GetPagerStr: function() {
            $.ajax({
                url: "/api/sitecore/MassMessageHistory/GetPagerStr",
                type: "POST",
                data: {},
                context: this,
                success: function (data) {

                    this.set("pager", data);
                }
            });
        }

    });

    var view = Sitecore.Definitions.Views.ControlView.extend({
        initialize: function (options) {
            this._super();
        }
    });

    Sitecore.Factories.createComponent("MassMessageHistory", model, view, ".sc-MassMessageHistory");

});
