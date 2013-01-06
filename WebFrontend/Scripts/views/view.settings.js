(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        settings: Backbone.View.extend({
            events: {
                //
            },

            el: '#content',

            initialize: function () {
                this.subviews = {};
                this.subviews.server_settings_view = PBF.get_view('server_settings', 'server_model');
                this.subviews.chat_settings = PBF.get_view('chat_settings', 'chat_model');
                //<div id="chat-settings"></div>
                //<div id="server-settings"></div>
            },

            render: function () {
                this.$el.html(ich.tpl_settings());
                this.render_sub_views();

                //this.$el.find(this.child_view.el).append(this.child_view.render());
            },

            render_sub_views: function () {
                var scope = this;
                _.each(scope.subviews, function (view) {
                    view.render()
                    scope.$el.find('#' + view.id).append(view.el);
                });
            }
        }),

        chat_settings: Backbone.View.extend({
            events: {
                //
            },

            id: 'chat-settings',

            initialize: function () {
                //
            },

            render: function () {
                this.$el.html(ich.tpl_chat_settings());
            }
        }),

        server_settings: Backbone.View.extend({
            events: {
                //
            },

            id: 'server-settings',

            initialize: function () {
                //
            },

            render: function () {
                this.$el.html(ich.tpl_server_settings());
            }
        })
    });
}(window, jQuery, _, ich));