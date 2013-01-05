(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        chat: Backbone.View.extend({
            events: {
                //
            },

            el: '#content',

            initialize: function () {
                //
            },

            render: function () {
                this.$el.html(ich.tpl_chat());
            }
        })
    });
}(window, jQuery, _, ich));