(function(window,$,_){
	var map_rotation = Backbone.Model.extend({
		defaults: {}
	});

	_.extend(window.GSWAT.prototype.model_definitions,{map_rotation:map_rotation});
}(window,jQuery,_));