(function(window, $){
    window.pGSWAT.prototype.router = Backbone.Router.extend({
        initialize: function(){},

        render_main: function(){
            var files = [
                PBF.CDN + 'Scripts/models/model.server.js',
                PBF.CDN + 'Scripts/views/view.home.js'
            ];
            PBF.load(files,function(){
                var header = PBF.get_view('header', 'server_model');
                var main = PBF.get_view('main', 'server_model');
                var footer = PBF.get_view('footer');
            });
        },

        routes: {
            'home' : 'render_main'
        }
    });
}(window,jQuery));