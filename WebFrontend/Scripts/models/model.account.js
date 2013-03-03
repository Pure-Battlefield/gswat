(function(window,$,_){
	var account_model = Backbone.Model.extend({
		defaults: {

		}
	});

	_.extend(window.GSWAT.prototype.model_definitions,{account_model: account_model});
}(window,jQuery,_));