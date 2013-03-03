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
				var map_rotation = PBF.get({view:{name:'map_rotation'},model:{name:'map_rotation'},collection:map_list_collection});
				PBF.render(map_rotation);
			});
		},

		render_map_rotation_form: function(id){
			var files = [
				PBF.CDN + 'Scripts/models/model.server.js',
				PBF.CDN + 'Scripts/models/model.map-rotation.js',
				PBF.CDN + 'Scripts/views/view.map-rotation.js'
			];
			PBF.load(files,function(){
				var map_model = PBF.get({model:{name:'map_list_model',data:{create_new:true}}});
				if(id){
					var map_list_collection = PBF.get({collection:{name:'map_list_collection'}});
					map_model = map_list_collection.get(id);
					if(_.isUndefined(map_model)){
						window.location.hash = '#map-rotation';
						PBF.alert({type:'error',message:'Could not edit the selected playlist'})
					} else {
						map_model = map_list_collection.get(id);
						render_form();
					}
				} else {
					render_form();
				}

				function render_form(){
					var map_form = PBF.get({view:{name:'map_form'},model:map_model});
					PBF.render(map_form);
				}
			});
		},

		routes:{
			'home'                 :'render_home',
			'chat'                 :'render_chat',
			'settings'             :'render_settings',
			'loading'              :'render_loading',
			'coming-soon'          :'render_coming_soon',
			'map-rotation'         :'render_map_rotation',
			'map-rotation/new'     :'render_map_rotation_form',
			'map-rotation/edit/:id':'render_map_rotation_form'
		}
	});
}(window,jQuery));