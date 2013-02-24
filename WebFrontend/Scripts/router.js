(function(window,$){
	window.GSWAT.prototype.router = Backbone.Router.extend({
		initialize: function(){
			var header = PBF.get({view:{name:'header'},model:{name:'header'}});
			var footer = PBF.get({view:{name:'footer'},model:{name:'footer'}});

			this.on('route',function(){
				header.set_active(Backbone.History.prototype.getHash(window));
			});
		},

		render_home: function(){
			var files = [
				PBF.CDN + 'Scripts/models/model.server.js',
				PBF.CDN + 'Scripts/models/model.chat.js',
				PBF.CDN + 'Scripts/views/view.home.js'
			];
			PBF.load(files,function(){
				var server = PBF.get({model:{name:'server_model'}});
				var home = PBF.get({view:{name:'home'},model:server});
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
				var chat_model = PBF.get({model:{name:'chat_model'}});
				var chat = PBF.get({view:{name:'chat'},model:chat_model});
				PBF.render(chat);
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
			var files = [
				PBF.CDN + 'Scripts/models/model.server.js',
				PBF.CDN + 'Scripts/models/model.map-rotation.js',
				PBF.CDN + 'Scripts/views/view.map-rotation.js'
			];
			PBF.load(files,function(){
				var server = PBF.get({model:{name:'server_model'}});
				var map_list_collection = PBF.get({collection:{name:'map_list_collection'}});
				var maps = [
					{
						name			: 'Conquest Large 1',
						min_players		: 40,
						max_players		: 50,
						activated		: true
					},
					{
						name			: 'Conquest Large 2',
						min_players		: 10,
						max_players		: 50,
						activated		: false
					},
					{
						name			: 'Conquest Large 3',
						min_players		: 15,
						max_players		: 64,
						activated		: false
					},
					{
						name			: 'Conquest Large 4',
						min_players		: 20,
						max_players		: 50,
						activated		: false
					},
					{
						name			: 'Conquest Large 5',
						server_name		: 'Server Name',
						min_players		: 20,
						max_players		: 64,
						activated		: false
					},
					{
						name			: 'Conquest Large 6',
						server_name		: 'Testing',
						min_players		: 0,
						max_players		: 10,
						activated		: false
					}
				];
				_.each(maps,function(map){
					map.slugged_name = PBF.slugify(map.name);
				});
				map_list_collection.add(maps);
				var map_rotation = PBF.get({view:{name:'map_rotation'},model:{name:'map_rotation'},collection:map_list_collection});
				PBF.render(map_rotation);
			});
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