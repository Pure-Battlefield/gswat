(function(window, $, _, ich) {
	_.extend(window.GSWAT.prototype.view_definitions, {
		map_rotation: Backbone.View.extend({
			events: {
				'click .btn'	: 'activate'
			},

			id: 'map-rotation',

			initialize: function () {
				this.model.on('change', this.render, this);
				this.collection.on('add', this.render, this);
				this.collection.on('remove', this.render, this);
				this.model.on('render',this.render,this);
			},

			activate: function(event){
				var button = $(event.currentTarget);
				var action = button.attr('data-action');
				var map_list = this.collection.get(button.attr('data-id'));
				switch(action){
					case 'delete':
						PBF.modal({title:'Confirm Delete',message:'Are you sure you wish to delete this map list?',button:'danger','button-text':'Delete'},function(){map_list.delete_list()});
						break;
					case 'activate':
						PBF.modal({title:'Confirm Activation',message:'Are you sure you wish to activate this map list?',button:'danger','button-text':'Activate'},function(){map_list.activate_list()});
						break;
				}
			},

			render: function () {
				this.$el.html(ich.tpl_map_rotation(this.collection.toJSON()));
			}
		})
	});
}(window, jQuery, _, ich));