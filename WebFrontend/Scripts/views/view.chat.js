(function(window, $, _, ich) {
    _.extend(window.pGSWAT.prototype.view_definitions, {
        chat: Backbone.View.extend({
            events: {
                //
            },

            el: '#content',

            initialize: function () {
                this.render();
            },

            render: function () {
                this.$el.html(ich.tpl_chat());
            }
        })
    });
}(window, jQuery, _, ich));