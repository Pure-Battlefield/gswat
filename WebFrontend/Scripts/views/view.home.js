(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        home: Backbone.View.extend({
            events: {
                //
            },

            id: 'home',

            initialize: function () {
                //
            },

            render: function () {
                this.$el.html(ich.tpl_body());
            }
        })
    });
}(window, jQuery, _, ich));