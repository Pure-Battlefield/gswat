(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        chat: Backbone.View.extend({
            events: {
                'click #chat-force-refresh'     : 'force_refresh',
                'click .archive-btn'            : 'fetch_archive',
                'click #chat-toggle-options'    : 'quick_settings'
            },

            id: 'server-chat-page',

            className: 'page',

            initialize: function () {
                this.model.on("change:update_msgs", this.render, this);
                this.model.on("change:iframe_url", this.render, this);
                this.model.on("change:show_server_msgs", this.toggle_server_msgs, this);
                this.view = {};
                this.subviews = {};
                this.subviews.chat_settings = PBF.get_view('chat_settings', this.model);
            },

            quick_settings: function (event) {
                event.preventDefault();
                this.$el.find('#chat-quick-settings').toggle();
            },

            force_refresh: function (event) {
                event.preventDefault();
                this.model.get_msgs();
            },

            render_messages: function(){
                this.view.messages = this.model.get('all_msgs');
            },

            render_iframe: function () {
                this.view.iframe_url = this.model.get('iframe_url');
                this.render(view);
            },

            toggle_server_msgs: function () {
                this.$el.find('#chat-contents').toggleClass('hide-server-msgs');
            },

            fetch_archive: function (event) {
                event.preventDefault();
                var save = ($(event.currentTarget).attr('id') == 'save-archive')? true : false;
                var date = { archive_date: this.$el.find('#chat-date-input').val(), save_archive: save };
                if (date.archive_date != '') {
                    this.model.set(date);
                }
            },

            render: function () {
                this.$el.html(ich.tpl_chat(this.model.toJSON()));
                this.delegateEvents(); // TODO: Properly fix this event issue
                this.render_sub_views();
            },

            render_sub_views: function () {
                var view = this;
                _.each(view.subviews, function (sub_view) {
                    sub_view.render();
                    view.$el.find('#' + sub_view.id).replaceWith(sub_view.el);
                    sub_view.delegateEvents(); // TODO: Properly fix this event issue
                });
            }
        })
    });
}(window, jQuery, _, ich));