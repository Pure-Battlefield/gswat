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
            },

            quick_settings: function (event) {
                event.preventDefault();
                this.$el.find('#chat-quick-settings').toggle();
            },

            force_refresh: function (event) {
                event.preventDefault();
                this.model.get_msgs();
            },

            render_iframe: function () {
                this.render();
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
                }
            },

            render: function () {
                this.$el.html(ich.tpl_chat(this.model.toJSON()));
                this.delegateEvents(); // TODO: Properly fix this event issue
                this.render_sub_views();
            },

            render_sub_views: function () {
                var view = this;
                _.each(view.subviews, function (sub_view) {
                    view.$el.find('#' + sub_view.id).replaceWith(sub_view.render().el);
                    sub_view.delegateEvents(); // TODO: Properly fix this event issue
                });
            }
        }),

        chat_messages: Backbone.View.extend({
			events: {
				'scroll #chat-contents'	: 'scroll'
			},

            id: 'chat-contents-ul',

            tagName: 'ul',

            initialize: function () {
				this.do_scroll = true;
                this.model.on("change:new_msgs", this.render, this);
            },

			//TODO: Fix the scrolling
			scroll: function(){
				var ele = document.getElementById('chat-contents');
				if(ele.scrollTop != ele.scrollHeight){
					this.do_scroll = false;
				}
			},

            render: function () {
                this.$el.append(ich.tpl_chat_messages({all_msgs:this.model.get('new_msgs')}));
				if(this.do_scroll){
					var ele = $("chat-contents");
					ele.prop('scrollTop',ele.prop('scrollHeight'));
				}
				return this;
            }
        })
    });
}(window, jQuery, _, ich));