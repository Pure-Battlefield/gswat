(function(window, $, _) {
    var chat_model = Backbone.Model.extend({
        defaults : {
            password: '',
            ip: '',
            port: 0
        },

        initialize : function () {
            //
        }
    });

    var chat_day_model = Backbone.Model.extend({
        defaults: {},

        initialize: function () {
            //
        }
    });

    _.extend(window.pGSWAT.prototype.model_definitions, { chat_model: chat_model });
}(window, jQuery, _));