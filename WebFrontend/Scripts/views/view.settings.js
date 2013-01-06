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
            },

            render: function () {
                this.$el.html(ich.tpl_settings());
                this.render_sub_views();
            },

            render_sub_views: function () {
                var scope = this;
                _.each(scope.subviews, function (view) {
                    view.render()
                    scope.$el.find('#' + view.id).replaceWith(view.el);
                });
            }
        }),

        chat_settings: Backbone.View.extend({
            events: {
                'click input:submit'    : 'submit',
                'click button.helper': 'clear_field',
                'change .input-control.switch input': 'change_switch'
            },

            id: 'chat-settings',

            initialize: function () {
                //
            },

            submit: function (event) {
                event.preventDefault();
                var val = parseInt(this.$el.find('#chat-interval-field input').val());
                if (!isNaN(val)) {
                    this.model.set({ 'interval': val });
                } else {
                    // Not a number, do something
                }
            },

            change_switch: function (event) {
                var ele = $(event.currentTarget);
                var val = {};
                val[ele.attr('data-field')] = ele.is(':checked');
                this.model.set(val);
            },

            clear_field: function (event) {
                event.preventDefault();
                $(event.currentTarget).siblings('input').val('');
            },

            render: function () {
                this.$el.html(ich.tpl_chat_settings(this.model.toJSON()));
            }
        }),

        server_settings: Backbone.View.extend({
            events: {
                'click input:submit': 'submit',
                'click button.helper': 'clear_field'
            },

            id: 'server-settings',

            initialize: function () {
                //
            },

            submit: function (event) {
                event.preventDefault();
                var values = this.$el.find('form').serializeArray();
                this.model.set(values);
            },

            clear_field: function (event) {
                event.preventDefault();
                $(event.currentTarget).siblings('input').val('');
            },

            render: function () {
                this.$el.html(ich.tpl_server_settings(this.model.toJSON()));
            }
        })
    });
}(window, jQuery, _, ich));