(function(window,$){
	window.GSWAT.prototype.router = Backbone.Router.extend({
		initialize: function(){
			var header = PBF.get({view:{name:'header'}});
			var footer = PBF.get({view:{name:'footer'}});
		},

		render_home: function(){
			var files = [
				PBF.CDN + 'Scripts/models/model.server.js',
				PBF.CDN + 'Scripts/models/model.chat.js',
				PBF.CDN + 'Scripts/views/view.home.js'
			];
			PBF.load(files,function(){
				var home = PBF.get({view:{name:'home'},model:{name:'server_model'}});
				PBF.render(home);
			});
		},

		render_chat: function(){
			var files = [
				PBF.CDN + 'Scripts/models/model.server.js',
				PBF.CDN + 'Scripts/models/model.chat.js',
				PBF.CDN + 'Scripts/views/view.settings.js',
				PBF.CDN + 'Scripts/views/view.chat.js'
			];
			PBF.load(files,function(){
				var server = PBF.get({model:{name:'server_model'}});
				var chat = PBF.get({view:{name:'chat'},model:{name:'chat_model'}});
				PBF.render(chat);
				//$(PBF.main_ele).html(chat.$el);
			});
		},

		render_settings: function(){
			var files = [
				PBF.CDN + 'Scripts/models/model.server.js',
				PBF.CDN + 'Scripts/models/model.chat.js',
				PBF.CDN + 'Scripts/views/view.settings.js'
			];
			PBF.load(files,function(){
				var settings = PBF.get({view:{name:'settings'}});
				PBF.render(settings);
			});
		},

		render_loading: function(){
			var loading = PBF.get({view:{name:'loading'}});
			PBF.render(loading);
		},

		render_coming_soon: function(){
			var coming_soon = PBF.get({view:{name:'coming_soon'}});
			PBF.render(coming_soon);
		},

		render_map_rotation: function(){
			var map_rotation = PBG.get({view:{name:'map_rotation'}});
			PBF.render(map_rotation);
		},

		routes: {
			'home'			: 'render_home',
			'chat'			: 'render_chat',
			'settings'		: 'render_settings',
			'loading'		: 'render_loading',
			'coming-soon'	: 'render_coming_soon',
			'map-rotation'	: 'render_map_rotation'
		}
	});
}(window,jQuery));