(function(window, $){
    window.pGSWAT.prototype.router = Backbone.Router.extend({
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
            });
        },

        render_chat: function () {
            var files = [
                PBF.CDN + 'Scripts/models/model.server.js',
                PBF.CDN + 'Scripts/models/model.chat.js',
                PBF.CDN + 'Scripts/views/view.chat.js'
            ];
            PBF.load(files, function () {
                var chat = PBF.get_view('chat', 'chat_model');
            });
        },

        render_settings: function () {
            var files = [
                PBF.CDN + 'Scripts/models/model.server.js',
                PBF.CDN + 'Scripts/models/model.chat.js',
                PBF.CDN + 'Scripts/views/view.settings.js'
            ];
            PBF.load(files, function () {
                var chat_settings = PBF.get_view('chat_settings', 'chat_model');
                var server_settings = PBF.get_view('server_settings', 'server_model');
            });
        },

        render_coming_soon : function(){
            var coming_soon = PBF.get_view('coming_soon');
        },

        routes: {
            'home'       : 'render_home',
            'chat'          : 'render_chat',
            'settings'      : 'render_settings',
            'coming-soon'   : 'render_coming_soon'
        }
    });
}(window,jQuery));