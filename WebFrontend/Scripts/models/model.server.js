(function(window, $, _) {
    var server_model = Backbone.Model.extend({
        defaults : {
            server_ip: '',
            server_password: '',
            server_port: 0,
            server_old_password: ''
        },

        initialize : function () {
            //
        }
    });

    _.extend(window.GSWAT.prototype.model_definitions, { server_model: server_model });
}(window, jQuery, _));