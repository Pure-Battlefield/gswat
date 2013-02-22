(function(window, $, _, ich) {
    _.extend(window.GSWAT.prototype.view_definitions, {
        chat: Backbone.View.extend({
            events: {
                'click #chat-force-refresh'     : 'force_refresh',
                'click .archive-btn'            : 'fetch_archive',
                'click #chat-toggle-options'    : 'quick_settings'
            },

            id: 'server-chat-page',

            className: 'page',

            initialize: function () {
                this.model.on("change:iframe_url", this.render_iframe, this);
                this.model.on("change:show_server_msgs", this.toggle_server_msgs, this);
                this.view = {};
                this.subviews = {};
                this.subviews.chat_messages = PBF.get({view:{name:'chat_messages'},model:this.model});
                this.subviews.chat_settings = PBF.get({view:{name:'chat_settings'},model:this.model});
				this.server_model = PBF.get({model:{name:'server_model'}});
				this.server_model.on('change:ServerIP',this.update_info,this);
            },

            quick_settings: function (event) {
                event.preventDefault();
				$(event.currentTarget).parent('li').toggleClass('active');
                this.$el.find('#chat-quick-settings').toggle();
				this.$el.find('#chat-date-input').datepicker();
            },

            force_refresh: function (event) {
                event.preventDefault();
                this.model.get_msgs();
            },

			update_info: function(){
				this.$el.find('#server-ip').text(this.server_model.get('ServerIP'));
			},

            render_iframe: function () {
                this.$el.find('#download-chat-iframe').prop('src',this.model.get('iframe_url'));
                this.model.set({iframe_url:''},{silent:true});
            },

            toggle_server_msgs: function () {
                this.$el.find('#chat-contents').toggleClass('hide-server-msgs');
            },

            fetch_archive: function (event) {
                event.preventDefault();
                var save = ($(event.currentTarget).attr('id') == 'save-archive')? true : false;
                var date = { archive_date: this.$el.find('#chat-date-input').val(), save_archive: save };
                if (date.archive_date != '') {
                    this.model.set(date);
					if(!save){
						this.subviews.chat_messages.$el.empty();
					}
                } else {
					PBF.alert({type:'error',title:'Error:',message:'You must enter a valid date (MM/DD/YYYY)'});
				}
            },

            render: function () {
				var data = this.model.toJSON();
				var server = this.server_model.toJSON();
				_.extend(data,server);
				this.$el.html(ich.tpl_chat(data));
                this.delegateEvents();
                this.render_sub_views();
            },

            render_sub_views: function () {
                var view = this;
                _.each(view.subviews, function (sub_view) {
                    view.$el.find('#' + sub_view.id).replaceWith(sub_view.render().el);
                    sub_view.delegateEvents();
                });
            }
        }),

        chat_messages: Backbone.View.extend({
            id: 'chat-contents-ul',

            tagName: 'ul',

            initialize: function () {
                this.model.on("change:new_msgs", this.render, this);
            },

			empty: function(){
				this.$el.empty();
			},

            render: function () {
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
}(window, jQuery, _, ich));