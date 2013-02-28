(function(window,$,_){
	var map_rotation = Backbone.Model.extend({
		defaults: {}
	});

	var map_list_model = Backbone.Model.extend({
		defaults: {
			maps	: [],
			active	: false
		},

		idAttribute: "slugged_name",

		initialize: function(){
			this.on('change:maps',this.update_count,this);
			this.update_count();
		},

		update_count: function(){
			this.set({map_count:this.get('maps').length});
		}
	});

	_.extend(window.GSWAT.prototype.collection_definitions, {
		map_list_collection: Backbone.Collection.extend({
			model: map_list_model,

			initialize: function(){
				//TODO: Remove this once API exists
				var list = [
					{
						name	: 'Caspian Border',
						gamemode: 'Conquest Large',
						rounds	: 2
					},
					{
						name	: 'Operation Firestorm',
						gamemode: 'Conquest Large',
						rounds	: 2
					},
					{
						name	: 'Kharg Island',
						gamemode: 'Conquest Large',
						rounds	: 2
					}
				];
				var maps = [
					{
						name			: 'Conquest Large 1',
						server_name		: 'Testes',
						min_players		: 40,
						max_players		: 50,
						maps			: list,
						active			: true
					},
					{
						name			: 'Conquest Large 2',
						server_name		: 'Server for Cats',
						min_players		: 10,
						max_players		: 50,
						maps			: list,
						active			: false
					},
					{
						name			: 'Conquest Large 3',
						server_name		: 'habababab',
						min_players		: 15,
						max_players		: 64,
						maps			: list,
						active			: false
					},
					{
						name			: 'Conquest Large 4',
						server_name		: 'Serverrrr',
						min_players		: 20,
						max_players		: 50,
						maps			: list,
						active			: false
					},
					{
						name			: 'Conquest Large 5',
						server_name		: 'Server Name',
						min_players		: 20,
						max_players		: 64,
						maps			: list,
						active			: false
					},
					{
						name			: 'Conquest Large 6',
						server_name		: 'Testing',
						min_players		: 0,
						max_players		: 10,
						maps			: list,
						active			: false
					}
				];
				_.each(maps,function(map){
					map.slugged_name = PBF.slugify(map.name);
				});
				this.add(maps);
			},

			activate_list: function(id){
				//TODO: Server implementation should use .sync
				var collection = this;
				/*$.ajax({
					url: '/' + id,
					type: 'DELETE',
					success:function(){*/
						PBF.alert({type:'success',message:'Map List activated successfully!'});
						var list = collection.get(id);
						collection.each(function(model){
							model.set({active:false});
						});
						list.set({active:true});
						collection.trigger('change');
					/*},
					error:function(error){
						PBC.alert({type:'error',message:error});
					}
				});*/
			},

			delete_list: function(id){
				var collection = this;
				//TODO: Server implementation should use .sync
				/*$.ajax({
					url: '/' + id,
					type: 'DELETE',
					success:function(){*/
						PBF.alert({type:'success',message:'Map List deleted successfully!'});
						var list = collection.get(id);
						var active = list.get('active');
						collection.remove(id,{silent:true});
						if(active){
							collection.first().set({active:true});
						}
						collection.trigger('change');
					/*},
					error:function(error){
						PBC.alert({type:'error',message:error});
					}
				});*/
			}
		})
	});

	_.extend(window.GSWAT.prototype.model_definitions,{map_rotation:map_rotation,map_list_model:map_list_model});
}(window,jQuery,_));