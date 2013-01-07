(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        chat: Backbone.View.extend({
            events: {
                'click #chat-force-refresh' : 'force_refresh'
            },

            el: '#content',

            initialize: function () {
                model = this.model;
                model.on("change", this.render_messages, this);
            },

            force_refresh: function () {
                this.model.get_msgs();
            },

            render_messages: function(){
                var view = {};
                view.messages = this.model.get('all_msgs');
                this.render(view);
            },

            render: function (data) {
                this.$el.html(ich.tpl_chat(data));
            }
        })
    });
}(window, jQuery, _, ich));