(function(window, $, _, ich) {
    _.extend(window.pGSWAT.prototype.view_definitions, {
        header: Backbone.View.extend({
            events: {
                //
            },

            el: '#header',

            initialize: function(){
                this.render();
            },

            render: function(){
                this.$el.html(ich.tpl_header());
            }
        }),

        main: Backbone.View.extend({
            events: {
                //
            },

            el: '#content',

            initialize: function () {
                this.render();
            },

            render: function () {
                this.$el.html(ich.tpl_body());
            }
        }),

        footer: Backbone.View.extend({
            events: {
                //
            },

            el: '#footer',

            initialize: function(){
                this.render();
            },

            render: function(){
                this.$el.html(ich.tpl_footer());
            }
        })
    });
}(window, jQuery, _, ich));