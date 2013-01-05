(function(window, $){
    window.pGSWAT.prototype.router = Backbone.Router.extend({
        initialize: function(){},

        render_main: function(){
            var files = [
                PBF.CDN + 'js/models/model.server.js',
                PBF.CDN + 'js/views/view.home.js'
            ];
            PBF.load(files,function(){
                var header = PBF.get_view('header', 'server_model');
                var featured = PBF.get_view('featured', 'server_model');
                var content = PBF.get_view('content', 'server_model');
                var footer = PBF.get_view('footer');
                PBF.event_controller.bind_events();
            });
        },

        routes: {
            'main' : 'render_main'
        }
    });
}(window,jQuery));