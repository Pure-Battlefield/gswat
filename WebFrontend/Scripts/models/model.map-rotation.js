(function(window,$,_){
	var map_rotation = Backbone.Model.extend({
		defaults: {}
	});

	var map_list_model = Backbone.Model.extend({
		defaults: {
			name			: 'Conquest Large',
			server_name		: 'Conquest Awesomeness',
			maps			: [
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
			],
			map_count		: 0,
			min_players		: 40,
			max_players		: 50,
			activated		: true
		},

		idAttribute: "slugged_name",

		initialize: function(){
			this.on('change:maps',this.update_count,this);
			this.update_count();
		},

		update_count: function(){
			this.set({map_count:this.get('maps').length});
		},

		delete_list: function(){
			var model = this;
			/*$.ajax({
				url: '/' + model.id,
				type: 'DELETE',
				success:function(){*/
					PBF.alert({type:'success',message:'Map List deleted successfully!'});
					var collection = PBF.get({collection:{name:'map_list_collection'}});
					collection.remove(model);
				/*},
				error:function(error){
					PBC.alert({type:'error',message:error});
				}
			});*/
		},

		activate_list: function(){
			var model = this;
			/*$.ajax({
				url: '/' + model.id,
				type: 'DELETE',
				success:function(){*/
					PBF.alert({type:'success',message:'Map List activated successfully!'});
					model.set({activated:!model.get('activated')});
				/*},
				error:function(error){
					PBC.alert({type:'error',message:error});
				}
			});*/
		}
	});

	_.extend(window.GSWAT.prototype.collection_definitions, {
		map_list_collection: Backbone.Collection.extend({
			model: map_list_model
		})
	});

	_.extend(window.GSWAT.prototype.model_definitions,{map_rotation:map_rotation});
}(window,jQuery,_));