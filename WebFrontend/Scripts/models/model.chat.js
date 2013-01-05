(function(window, $, _) {
    var chat_model = Backbone.Model.extend({
        defaults : {
            'auto_refresh': true,
            'url': '/api/values/getallmessages',
            'interval': 1000,
            'show_server_msgs': false,
            'team_1_msgs': {},
            'team_2_msgs': {},
            'server_msgs': {}
        },

        url: this.get('url'),

        initialize: function () {
            //
        },

        change_internal: function (interval) {
        },

        refresh: function () {
            //
        },

        parse: function (data) {
            //
        }
    });

    var chat_date_model = Backbone.Model.extend({
        defaults: {},

        initialize: function () {
            //
        }
    });

    $.extend(window.GSWAT.prototype.collection_definitions, {
        chat_date_collection: Backbone.Collection.extend({
            model: chat_date_model,

            url: function (day) {
                //return url;
            },

            parse: function (data) {
                console.log(data);
                return data;
            }
        })
    });

    _.extend(window.GSWAT.prototype.model_definitions, { chat_model: chat_model });
}(window, jQuery, _));