(function(window, $, _) {
    var server_model = Backbone.Model.extend({
        defaults : {
            ip: '',
            port: 0
        },

        initialize : function () {
            //
        }
    });

    _.extend(window.pGSWAT.prototype.model_definitions, { server_model: server_model });
}(window, jQuery, _));