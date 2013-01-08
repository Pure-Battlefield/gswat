(function(window, $){
    window.GSWAT.prototype.router = Backbone.Router.extend({
        initialize: function () {
            var header = PBF.get_view('header');
            var footer = PBF.get_view('footer');
        },

        render_home: function(){
            var files = [
                PBF.CDN + 'Scripts/models/model.server.js',
                PBF.CDN + 'Scripts/models/model.chat.js',
                PBF.CDN + 'Scripts/views/view.home.js'
            ];
            PBF.load(files, function () {
                var home = PBF.get_view('home', 'server_model');
                home.render();
            });
        },

        render_chat: function () {
            var files = [
                PBF.CDN + 'Scripts/models/model.server.js',
                PBF.CDN + 'Scripts/models/model.chat.js',
                PBF.CDN + 'Scripts/views/view.chat.js'
            ];
            PBF.load(files, function () {
                var server = PBF.get_model('server_model');
                var chat = PBF.get_view('chat', 'chat_model');
                chat.render();
            });
        },

        render_settings: function () {
            var files = [
                PBF.CDN + 'Scripts/models/model.server.js',
                PBF.CDN + 'Scripts/models/model.chat.js',
                PBF.CDN + 'Scripts/views/view.settings.js'
            ];
            PBF.load(files, function () {
                var settings = PBF.get_view('settings');
                settings.render();
            });
        },

        render_loading: function(){
            var loading = PBF.get_view('loading');
            loading.render();
        },

        render_coming_soon : function(){
            var coming_soon = PBF.get_view('coming_soon');
            coming_soon.render();
        },

        routes: {
            'home'          : 'render_home',
            'chat'          : 'render_chat',
            'settings'      : 'render_settings',
            'loading'       : 'render_loading',
            'coming-soon'   : 'render_coming_soon'
        }
    });
}(window,jQuery));