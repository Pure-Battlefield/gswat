(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        header: Backbone.View.extend({
            events: {
                //
            },

            el: '#header',

            initialize: function(){
                this.render();
            },

            render: function () {
                this.$el.html(ich.tpl_header());
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
        }),

        coming_soon: Backbone.View.extend({
            events: {
                //
            },

            el: '#content',

            initialize: function () {
                //
            },

            render: function () {
                this.$el.html(ich.tpl_coming_soon());
            }
        }),

        loading: Backbone.View.extend({
            events: {
                //
            },

            el: '#content',

            initialize: function () {
                //
            },

            render: function () {
                this.$el.html(ich.tpl_loading());
            }
        })
    });
}(window, jQuery, _, ich));