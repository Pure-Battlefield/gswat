(function(window,$,_,ich){
	_.extend(window.GSWAT.prototype.view_definitions,{
		login: Backbone.View.extend({
			id: 'login',

			render: function(){
				this.$el.html(ich.tpl_login());
			}
		})
	});
}(window,jQuery,_,ich));