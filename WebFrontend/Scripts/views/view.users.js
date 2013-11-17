(function(window,$,_,ich){
    _.extend(window.GSWAT.prototype.view_definitions,{
        users: Backbone.View.extend({
            id: 'users',

            initialize: function () {

            },
            
            render: function () {
                this.$el.html(ich.tpl_users());
            }
        })
    });
}(window, jQuery, _, ich));