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
				this.collection.on('change', this.render, this);
				this.collection.on('remove', this.render, this);
				this.model.on('render',this.render,this);
			},

			activate: function(event){
				var button = $(event.currentTarget);
				var action = button.attr('data-action');
				var list_id = button.attr('data-id');
				var map_list = this.collection;
				switch(action){
					case 'delete':
						PBF.modal({title:'Confirm Delete',message:'Are you sure you wish to delete this map list?',button:'danger','button-text':'Delete'},function(){
							map_list.delete_list(list_id);
						});
						break;
					case 'activate':
						PBF.modal({title:'Confirm Activation',message:'Are you sure you wish to activate this map list?',button:'success','button-text':'Activate'},function(){
							map_list.activate_list(list_id);
						});
						break;
				}
			},

			render: function () {
				this.$el.html(ich.tpl_map_rotation(this.collection.toJSON()));
			}
		}),

		map_form: Backbone.View.extend({
			events: {
				'click .submit.btn'	: 'button'
			},

			initialize: function(){
				this.model.on('invalid',this.form_error,this);
			},

			button: function(event){
				event.preventDefault();
				var form = this.$el.find('form').serializeArray();
				var values = {};
				_.each(form, function (input) {
					values[input.name] = input.value;
				});
				this.model.set(values);
			},

			form_error: function(value){
				PBF.alert({type:'error',message:'There were one or more errors on the form'});
				console.log(value);
			},

			render: function(){
				this.$el.html(ich.tpl_map_form(this.model.toJSON()));
			}
		})
	});
}(window, jQuery, _, ich));