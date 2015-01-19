define(["sitecore"], function (Sitecore) {
    var MaterialManagement = Sitecore.Definitions.App.extend({
        initialized: function () {
        },

        addMessage: function () {

            jQuery.ajax({
                type: "POST",
                url: "/api/sitecore/MaterialManagement/AddMessage",
                //data: { "propertyData": JSON.stringify(property) },
                success: function (data) {

                    if (data.isSucceed) {
                        //alert(data.url);
                        window.location.href = "/sitecore/client/Your Apps/Wechat/EditMixedMessage?id=" + data.url;
                    } else {
                        alert("something wrong!");
                    }

                },
                error: function () {
                    console.log("There was an error. Try again please!");

                }
            });

        },
    });

    return MaterialManagement;
});