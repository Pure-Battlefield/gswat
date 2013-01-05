(function(window, $, _, ich) {
    _.extend(window.pGSWAT.prototype.view_definitions, {
        chat_settings: Backbone.View.extend({
            events: {
                //
            },

            el: '#content',

            initialize: function () {
                this.render();
            },

            render: function () {
                this.$el.html(ich.tpl_chat_settings());
            }
        })
    });
}(window, jQuery, _, ich));