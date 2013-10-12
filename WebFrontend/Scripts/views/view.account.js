(function(window,$,_,ich){
	_.extend(window.GSWAT.prototype.view_definitions,{
		login: Backbone.View.extend({
			id: 'login',

			render: function(){
				this.$el.html(ich.tpl_login());
			}
		}),

		logout: Backbone.View.extend({
			id: 'logout',

			render: function(){
				this.$el.html(ich.tpl_logout());
			}
		}),

		register: Backbone.View.extend({
			id: 'register',

			render: function(){
				this.$el.html(ich.tpl_register());
			}
		}),

		account: Backbone.View.extend({
			 id: 'account',
		    	events: {
		        	'click button.submit'         : 'submit'
			},
		    
            		submit: function(event) {
                		//TODO: account page submit function
            		},

			render: function(){
				this.$el.html(ich.tpl_account());
			}
		})
	});
}(window,jQuery,_,ich));
