(function(window,$,_){
	var map_rotation = Backbone.Model.extend({
		defaults: {}
	});

	var map_list_model = Backbone.Model.extend({
		defaults: {
			Maps  : [],
			active: false
		},

		idAttribute: "slugged_name",

		initialize: function(){
			console.log(this.toJSON());
			this.on('change:Maps',this.update_count,this);
			this.update_count();
		},

		validate: {
			Name      : {
				required: true
			},
			MaxPlayers: {
				type: "number",
				min : 0,
				max : 64
			},
			MinPlayers: {
				type: "number",
				min : 0,
				max : 64
			}
		},

		update_count: function(){
			this.set({MapCount: this.get('Maps').length});
		}
	});

	_.extend(window.GSWAT.prototype.collection_definitions,{
		map_list_collection: Backbone.Collection.extend({
			model: map_list_model,

			initialize: function(){
				//TODO: Remove this once API exists
				var list = [
					{
						Name    : 'Caspian Border',
						Gamemode: 'Conquest Large',
						Rounds  : 2
					},
					{
						Name    : 'Operation Firestorm',
						Gamemode: 'Conquest Large',
						Rounds  : 2
					},
					{
						Name    : 'Kharg Island',
						Gamemode: 'Conquest Large',
						Rounds  : 2
					}
				];
				var maps = [
					{
						Name      : 'Conquest Large 1',
						ServerName: 'Testes',
						MinPlayers: 40,
						MaxPlayers: 50,
						Maps      : list,
						active    : true
					},
					{
						Name      : 'Conquest Large 2',
						ServerName: 'Server for Cats',
						MinPlayers: 10,
						MaxPlayers: 50,
						Maps      : list,
						active    : false
					},
					{
						Name      : 'Conquest Large 3',
						ServerName: 'habababab',
						MinPlayers: 15,
						MaxPlayers: 64,
						Maps      : list,
						active    : false
					},
					{
						Name      : 'Conquest Large 4',
						ServerName: 'Serverrrr',
						MinPlayers: 20,
						MaxPlayers: 50,
						Maps      : list,
						active    : false
					},
					{
						Name      : 'Conquest Large 5',
						ServerName: 'Server Name',
						MinPlayers: 20,
						MaxPlayers: 64,
						Maps      : list,
						active    : false
					},
					{
						Name      : 'Conquest Large 6',
						ServerName: 'Testing',
						MinPlayers: 0,
						MaxPlayers: 10,
						Maps      : list,
						active    : false
					}
				];
				_.each(maps,function(map){
					map.slugged_name = PBF.slugify(map.Name);
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
				PBF.alert({type: 'success',message: 'Map List activated successfully!'});
				var list = collection.get(id);
				collection.each(function(model){
					model.set({active: false});
				});
				list.set({active: true});
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
				PBF.alert({type: 'success',message: 'Map List deleted successfully!'});
				var list = collection.get(id);
				var active = list.get('active');
				collection.remove(id,{silent: true});
				if(active){
					collection.first().set({active: true});
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

	_.extend(window.GSWAT.prototype.model_definitions,{map_rotation: map_rotation,map_list_model: map_list_model});
}(window,jQuery,_));