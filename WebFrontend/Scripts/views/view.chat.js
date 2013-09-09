(function(window,$,_,ich){
	_.extend(window.GSWAT.prototype.view_definitions,{
		chat: Backbone.View.extend({
			events: {
				'click #chat-force-refresh' : 'force_refresh',
				'click .archive-btn'        : 'fetch_archive',
				'click #chat-toggle-options': 'quick_settings',
				'click #chatbox-submit-btn': 'send_serverchat'
			},

			id: 'server-chat-page',

			className: 'page',

			initialize: function(){
				this.model.on("change:iframe_url",this.render_iframe,this);
				this.model.on("change:message_filters",this.toggle_filters,this);
				this.view = {};
				this.subviews = {};
				this.subviews.chat_messages = PBF.get({view: {name: 'chat_messages'},model: this.model});
				this.subviews.chat_settings = PBF.get({view: {name: 'chat_settings'},model: this.model});
				this.server_model = PBF.get({model: {name: 'server_model'}});
				this.server_model.on('change:ServerIP',this.update_info,this);
			},

			quick_settings: function(event){
				event.preventDefault();
				$(event.currentTarget).parent('li').toggleClass('active');
				this.$el.find('#chat-quick-settings').toggle();
				this.$el.find('#chat-date-input').datepicker();
			},

			force_refresh: function(event){
				event.preventDefault();
				this.model.get_msgs();
			},

			update_info: function(){
				this.$el.find('#server-ip').text(this.server_model.get('ServerIP'));
			},

			render_iframe: function(){
				this.$el.find('#download-chat-iframe').prop('src',this.model.get('iframe_url'));
				this.model.set({iframe_url: ''},{silent: true});
			},

			toggle_filters: function(){
				var filters = this.model.get('message_filters');
				var chat = this.$el.find('#chat-contents');
				var scroll = false;
				if(chat.prop('offsetHeight') + chat.prop('scrollTop') == chat.prop('scrollHeight')){
					scroll = true;
				}
				//TODO: Optimize this more
				chat.toggleClass('hide-server-msgs',!filters.show_server_msgs);
				chat.toggleClass('hide-global-msgs',!filters.show_global_msgs);
				chat.toggleClass('hide-ru-msgs',!filters.show_ru_msgs);
				chat.toggleClass('hide-us-msgs',!filters.show_us_msgs);
				chat.toggleClass('hide-squad-msgs',!filters.show_squad_msgs);
				if(scroll){
					chat.scrollTop(chat.prop('scrollHeight'));
				}
			},

			fetch_archive: function(event){
				event.preventDefault();
				var save = ($(event.currentTarget).attr('id') == 'save-archive') ? true : false;
				var date = { archive_date: this.$el.find('#chat-date-input').val(),save_archive: save };
				if(date.archive_date != ''){
					this.model.set(date);
					if(!save){
						this.subviews.chat_messages.$el.empty();
					}
				} else {
					PBF.alert({type: 'error',title: 'Error:',message: 'You must enter a valid date (MM/DD/YYYY)'});
				}
			},

			render: function(){
				var data = this.model.toJSON();
				var server = this.server_model.toJSON();
				_.extend(data,server);
				this.$el.html(ich.tpl_chat(data));
				this.delegateEvents();
				this.render_sub_views();
			    
			    // Bind enter key event to chatbox
				this.$el.find("#chatbox-msgfield").keyup(function (event) {
				    if (event.keyCode == 13) {
				        $("#chatbox-submit-btn").click();
				    }
				});
			},

			render_sub_views: function(){
				var view = this;
				_.each(view.subviews,function(sub_view){
					view.$el.find('#' + sub_view.id).replaceWith(sub_view.render().el);
					sub_view.delegateEvents();
				});
			},
			send_serverchat: function (event) {
			    var msg = this.$el.find('#chatbox-msgfield').val();
			    if (msg !== "") {
			        this.model.serverchat(msg, "", "", "", "");
			        // clear chatbox-field
			        this.$el.find('#chatbox-msgfield').val("");
			    }
			}
		}),

		chat_messages: Backbone.View.extend({
			id: 'chat-contents-ul',

			tagName: 'ul',

			initialize: function(){
				this.model.on("change:new_msgs",this.render,this);
			},

			empty: function(){
				this.$el.empty();
			},

			render: function(){
				var ele = $("#chat-contents");
				if(ele.prop('offsetHeight') + ele.prop('scrollTop') == ele.prop('scrollHeight')){
					this.append_message();
					ele.scrollTop(ele.prop('scrollHeight'));
				} else {
					this.append_message();
				}
				return this;
			},

			append_message: function(messages){
				messages = messages || this.model.get('new_msgs');
				if(messages.length > 0 && _.isArray(messages)){
					this.$el.append(ich.tpl_chat_messages(messages));
				}
			}
		})
	});
}(window,jQuery,_,ich));