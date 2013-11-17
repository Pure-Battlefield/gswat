(function(window,$,_){
    var users_model = Backbone.Model.extend({
        defaults: {},

        initialize: function () {

        }

    });
        _.extend(window.GSWAT.prototype.model_definitions,{users_model: users_model});
}(window,jQuery,_));