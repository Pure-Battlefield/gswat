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
                this.$el.html(ich.header());
            }
        }),

        footer: Backbone.View.extend({
            events: {
                //
            },

            el: '#footer',

            id : 'footerWpr',

            initialize: function(){
                this.render();
            },

            render: function(){
                this.$el.html(ich.footer());
            }
        })
    });
}(window, jQuery, _, ich));