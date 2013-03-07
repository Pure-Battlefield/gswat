(function(window,$,_,ich){
	_.extend(window.GSWAT.prototype.view_definitions,{
		header: Backbone.View.extend({
			events: {
				'click #logout'		: 'logout',
				'click #login'		: 'login'
			},

			el: '#header',

			initialize: function(){
				this.body = $('body');
				this.on('alert',this.trigger_alert,this);
				this.account = PBF.get({model:{name:'account_model'}});
				this.account.on('change:logged_in',this.update_login,this);
				this.render();
			},

			trigger_alert: function(data){
				var alert = PBF.get({view: {name: 'alert',reset: true},model: {name: 'alert',data: data}});
				this.$el.find('.navbar').append(alert.render().el);
			},

			set_active: function(event){
				var path = (typeof event === 'object') ? $(event.currentTarget).attr('href') : '#' + event.split('/')[0];
				this.$el.find('li').removeClass('active');
				this.$el.find('a[href=' + path + ']').parent('li').addClass('active');
			},

			logout: function(event){
				event.preventDefault();
				this.account.disconnect();
			},

			login: function(event){
				event.preventDefault();
			},

			update_login: function(){
				this.body.toggleClass('logged-in',this.account.get('logged_in'));
			},

			render: function(){
				this.$el.html(ich.tpl_header(this.model.toJSON()));
			}
		}),

		footer: Backbone.View.extend({
			el: '#footer',

			initialize: function(){
				this.on('modal',this.trigger_modal,this);
				this.render();
				this.$el.show();
			},

			trigger_modal: function(data,callback){
				var modal = PBF.get({view: {name: 'modal',reset: true},model: {name: 'modal',data: data,options: {callback: callback}}});
				this.$el.append(modal.render().el);
				modal.$modal.modal();
			},

			render: function(){
				this.$el.html(ich.tpl_footer(this.model.toJSON()));
			}
		}),

		coming_soon: Backbone.View.extend({
			id: 'coming-soon',

			render: function(){
				this.$el.html(ich.tpl_coming_soon());
			}
		}),

		loading: Backbone.View.extend({
			el: '#content',

			render: function(){
				this.$el.html(ich.tpl_loading());
			}
		}),

		alert: Backbone.View.extend({
			events: {
				'.close': 'hide_alert'
			},

			id: 'header-alert',

			className: 'hide',

			show_alert: function(){
				var view = this;
				this.error_timeout = window.setTimeout(function(){
					view.hide_alert();
				},2500);
			},

			hide_alert: function(){
				var view = this;
				clearTimeout(this.error_timeout);
				this.$el.fadeOut(300,function(){
					view.remove();
				});
			},

			render: function(){
				this.show_alert();
				this.$el.html(ich.tpl_alert(this.model.toJSON())).fadeIn(300);
				return this;
			}
		}),

		modal: Backbone.View.extend({
			events: {
				'click .btn-confirm': 'confirm'
			},

			id: 'confirm-dialogue',

			initialize: function(){
				this.render();
			},

			confirm: function(){
				var modal = this;
				this.$modal.on('hidden',function(){
					if(!_.isUndefined(modal.model.callback)){
						modal.model.callback();
					}
					modal.remove();
				});
				this.$modal.modal('hide');
			},

			render: function(){
				this.$el.html(ich.tpl_confirm_dialogue(this.model.toJSON()));
				this.$modal = this.$el.find('#modal');
				return this;
			}
		})
	});
}(window,jQuery,_,ich));