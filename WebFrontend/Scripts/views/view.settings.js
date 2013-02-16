(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        settings: Backbone.View.extend({
            id: 'settings',

            initialize: function () {
                this.subviews = {};
                this.subviews.server_settings_view = PBF.get({view:{name:'server_settings'},model:{name:'server_model'}});
                this.subviews.chat_settings = PBF.get({view:{name:'chat_settings'},model:{name:'chat_model'}});
            },

            render: function () {
                this.$el.html(ich.tpl_settings());
                this.render_sub_views();
                this.delegateEvents(); // TODO: Properly fix this event issue
            },

            render_sub_views: function () {
                var view = this;
                _.each(view.subviews, function (sub_view) {
                    view.$el.find('#' + sub_view.id).replaceWith(sub_view.render().el);
                    sub_view.delegateEvents(); // TODO: Properly fix this event issue
                });
            }
        }),

        chat_settings: Backbone.View.extend({
            events: {
                'click button.submit'		: 'submit',
                'change .checkbox input'	: 'change_switch'
            },

            id: 'chat-settings',

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

            render: function () {
                this.$el.html(ich.tpl_chat_settings(this.model.toJSON()));
				return this;
            }
        }),

        server_settings: Backbone.View.extend({
            events: {
                'click button.submit'    : 'submit'
            },

            id: 'server-settings',

            initialize: function () {
                this.model.bind('change:settings_success', this.update_confirm());
            },

            submit: function (event) {
                event.preventDefault();
                var form = this.$el.find('form').serializeArray();
                var values = {};
                _.each(form, function (input) {
                    values[input.name] = input.value;
                });
                this.model.update_settings(values);
                values.settings_success = 3;
                this.model.set(values, { silent: true });
            },

            update_confirm: function () {
                console.log('success',this.model.get('settings_success'));
            },

            render: function () {
                this.$el.html(ich.tpl_server_settings(this.model.toJSON()));
				return this;
            }
        })
    });
}(window, jQuery, _, ich));