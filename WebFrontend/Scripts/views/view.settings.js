(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        settings: Backbone.View.extend({
            id: 'settings',

            initialize: function () {
                this.subviews = {};
                this.subviews.server_settings_view = PBF.get({view:{name:'server_settings'},model:{name:'server_model'}});
				var chat_model = PBF.get({model:{name:'chat_model'}});
                this.subviews.chat_settings = PBF.get({view:{name:'chat_settings'},model:chat_model});
            },

            render: function () {
                this.$el.html(ich.tpl_settings());
                this.render_sub_views();
                this.delegateEvents();
            },

            render_sub_views: function () {
                var view = this;
                _.each(view.subviews, function (sub_view) {
                    view.$el.find('#' + sub_view.id).replaceWith(sub_view.render().el);
                    sub_view.delegateEvents();
                });
            }
        }),

        chat_settings: Backbone.View.extend({
            events: {
                'click button.submit'					: 'submit',
                'change #chat-auto-refresh-field input'	: 'change_switch',
                'change #toggle-message-types input'	: 'change_filters'
            },

            id: 'chat-settings',

			initialize: function(){
				this.model.on("change:auto_refresh", this.render, this);
			},

            submit: function (event) {
                event.preventDefault();
                var val = parseInt(this.$el.find('#chat-interval-field').val());
                if (!isNaN(val) && val >= 1) {
                    this.model.set({'interval':val});
					PBF.alert({type:'success',title:'Success:',message:'Interval updated!'});
                } else {
					PBF.alert({type:'error',title:'Error:',message:'Please only enter a valid number bigger or equal to 1'});
                }
            },

			change_filters: function(event){
				var inputs = this.$el.find('#toggle-message-types').find('input');
				var filters = {};
				_.each(inputs,function(input){
					input = $(input);
					filters[input.attr('data-field')] = input.is(':checked');
				});
				this.model.set({message_filters:filters});
				PBF.alert({type:'success',title:'Success:',message:'Filter updated!'});
			},

            change_switch: function (event) {
                var ele = $(event.currentTarget);
                var val = {};
                val[ele.attr('data-field')] = ele.is(':checked');
                this.model.set(val);
				PBF.alert({type:'success',title:'Success:',message:'Setting updated!'});
            },

            render: function () {
                this.$el.html(ich.tpl_chat_settings(this.model.toJSON()));
				return this;
            }
        }),

        server_settings: Backbone.View.extend({
            events: {
                'click button.submit'    : 'submit'
            },

            id: 'server-settings',

			initialize: function(){
				this.model.on('update_complete',this.toggle_submit,this);
				this.model.on('change:ServerIP',this.render,this);
			},

            submit: function (event) {
                event.preventDefault();
				if(!$(event.currentTarget).hasClass('disabled')){
					this.toggle_submit();
					var form = this.$el.find('form').serializeArray();
					var values = {};
					_.each(form, function (input) {
						values[input.name] = input.value;
					});
					this.model.set(values).trigger('submit');
				}
            },

			toggle_submit: function(){
				this.$el.find('button.submit').toggleClass('disabled');
			},

            render: function () {
                this.$el.html(ich.tpl_server_settings(this.model.toJSON()));
				return this;
            }
        })
    });
}(window, jQuery, _, ich));