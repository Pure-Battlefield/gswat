(function(window,$,_,moment){
	var chat_model = Backbone.Model.extend({
		defaults: {
			auto_refresh   : true,
			url            : '/api/messages/',
			interval       : 1,
			fetch_interval : {},
			message_filters: {
				show_server_msgs: true,
				show_global_msgs: true,
				show_ru_msgs    : true,
				show_us_msgs    : true,
				show_squad_msgs : true
			},
			all_msgs       : [],
			server_set     : true,
			new_msgs       : {},
			archive_date   : '',
			save_archive   : false,
			iframe_url     : '',
			last_fetch     : 0
		},

		initialize: function(){
			this.set({'last_fetch': moment().subtract('minutes',10)});
			this.set_interval();
			this.on('change:archive_date',this.get_old_msgs,this);
			this.on('change:auto_refresh',this.set_interval,this);
			this.on('change:interval',this.set_interval,this);
			this.on('change:server_set',this.set_interval,this);
		},

		get_msgs: function(){
			var model = this;
			var data = {DateTimeUnix: this.get('last_fetch').utc().valueOf(),Action: 'GetFromTime'};
			var url = this.get('url');
			this.set({'new_msgs': ''},{ silent: true });
			$.ajax({
				url     : url,
				type    : 'GET',
				data    : data,
				dataType: 'json',
				success : function(data){
					model.parse_msgs(data);
				}
			});
		},

		set_interval: function(){
			this.clear_interval();
			var model = this;
			if(this.get('server_set')){
				var interval = this.get('interval') * 1000;
				var _update = function(){
					if(model.get('server_set') && model.get('auto_refresh')){
						model.get_msgs();
					} else {
						model.clear_interval();
					}
				};
				window.fetchChat = window.setInterval(_update,interval);
			}
		},

		clear_interval: function(){
			window.clearInterval(window.fetchChat);
			delete window.fetchChat;
		},

		parse_msgs: function(data){
			if(data.length > 0){
				this.set({last_fetch: moment(data[data.length - 1].MessageTimeStamp)},{silent: true});
				data = PBF.parse_chat_messages(data);
				this.get('all_msgs').push(data.content);
				this.set({new_msgs: data.content});
			}
		},

		get_old_msgs: function(){
			var model = this;
			var date = moment(this.get('archive_date'),'MM/DD/YYYY').valueOf();
			var data = {DateTimeUnix: date};
			var url = this.get('url');
			if(date){
				PBF.alert({type: 'info',title: 'Fetching:',message: 'Please wait'});
				if(model.get('save_archive')){
					data.Action = 'DownloadByDay';
					model.set({iframe_url: url + $.param(data)});
				} else {
					data.Action = 'GetByDay';
					this.set({update_msgs: false,new_msgs: '',all_msgs: []},{silent: true});
					this.set({auto_refresh: false});
					this.clear_interval();
					$.ajax({
						type    : 'GET',
						url     : url,
						dataType: 'json',
						data    : data,
						success : function(data){
							model.parse_msgs(data);
							PBF.alert({type: 'success',title: 'Success!',message: 'Messages fetched'});
						},
						error   : function(error){
							PBF.alert({type: 'error',title: 'An error occurred:',message: error.responseText});
						}
					});
				}
			}
		}
	});

	_.extend(window.GSWAT.prototype.model_definitions,{chat_model: chat_model});
}(window,jQuery,_,moment));