(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        chat: Backbone.View.extend({
            events: {
                'click #chat-force-refresh': 'force_refresh',
                'click .archive-btn': 'fetch_archive'
            },

            className: 'page',

            initialize: function () {
                this.model.on("change:update_msgs", this.render_messages, this);
                this.model.on("change:iframe_url", this.render_iframe, this);
            },

            force_refresh: function () {
                this.model.get_msgs();
            },

            render_messages: function(view){
                view = view || {};
                view.messages = this.model.get('all_msgs');
                this.render(view);
            },

            render_iframe: function () {
                var view = {}
                view.iframe_url = this.model.get('iframe_url');
                this.render(view);
            },

            fetch_archive: function (event) {
                event.preventDefault();
                var save = ($(event.currentTarget).attr('id') == 'save-archive')? true : false;
                var date = { archive_date: this.$el.find('#chat-date-input').val(), save_archive: save };
                if (date.archive_date != '') {
                    this.model.set(date);
                }
            },

            render: function (data) {
                this.$el.html(ich.tpl_chat(data));
            }
        })
    });
}(window, jQuery, _, ich));