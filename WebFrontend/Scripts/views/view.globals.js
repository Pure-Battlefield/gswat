(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        header: Backbone.View.extend({
            events: {
                'tap .nav-bar a': 'hide_menu',
                'tap .pull-menu': 'toggle_menu',
                'swipe .nav-bar': 'swipe_menu'
            },

            el: '#header',

            initialize: function () {
                this.render();
            },

            hide_menu: function () {
                var menu = this.$el.find('ul.menu');
                menu.hide(200);
            },

            toggle_menu: function () {
                var menu = this.$el.find('ul.menu');
                menu.css({ 'overflow': 'hidden' });
                menu.toggle(200);
            },

            swipe_menu: function (e) {
                var menu = this.$el.find('ul.menu');
                menu.css({ 'overflow': 'hidden' });
                switch (e.direction) {
                    case 'down':
                        menu.show(200);
                        break;
                }
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