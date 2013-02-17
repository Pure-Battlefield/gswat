(function(window, $, _, moment) {
    var chat_model = Backbone.Model.extend({
        defaults : {
            'auto_refresh'      : true,
            'url'               : '/api/messages',
            'interval'          : 1,
            'fetch_interval'    : {},
            'show_server_msgs'  : true,
            'all_msgs'          : [],
            'server_set'        : true,
            'new_msgs'       	: {},
            'archive_date'      : '',
            'save_archive'      : false,
            'iframe_url'        : '',
            'last_fetch'        : 0
        },

        initialize: function () {
			this.set({'last_fetch':moment().subtract('minutes', 10)});
			this.set_interval();
            this.on('change:archive_date', this.get_old_msgs, this);
            this.on('change:auto_refresh', this.set_interval, this);
            this.on('change:interval', this.set_interval, this);
            this.on('change:server_set', this.set_interval, this);
        },

        get_msgs: function () {
            var model = this;
			var data = {DateTimeUnix:this.get('last_fetch').utc().valueOf(),Action:'GetFromTime'};
			var url = this.get('url');
			this.set({'new_msgs':''}, { silent: true });
            $.ajax({
                url:url,
				beforeSend: function (xhr) {
					xhr.setRequestHeader('Content-type', 'application/json')
				},
				contentType: 'application/json; charset=utf-8',
				type: 'POST',
				data: JSON.stringify(data),
				dataType: 'json',
                success: function(data){
                    model.parse_msgs(data);
                }
            });
        },

        set_interval: function () {
            this.clear_interval();
            if(this.get('server_set')){
                var _update = _.bind(function () {
                    if (this.get('server_set') && this.get('auto_refresh')) {
                        this.get_msgs();
                        var interval = this.get('interval') * 1000;
                        this._intervalFetch = window.setTimeout(_update, interval);
                    }
                }, this);
                _update();
            }
        },

        clear_interval: function () {
            window.clearTimeout(this._intervalFetch);
            delete this._intervalFetch;
        },

        parse_msgs: function (data) {
            if (data.length > 0) {
                this.set({ 'last_fetch': moment(data[data.length - 1].MessageTimeStamp) }, { silent: true });
                data = PBF.parse_chat_messages(data);
                this.get('all_msgs').push(data.content);
                this.set({new_msgs:data.content});
            }
        },

        get_old_msgs: function () {
            var model = this;
            var date = this.get('archive_date').split('/');
			var url = this.get('url');
            if (date.length === 3) {
                date = {
                    Day: date[1],
                    Month: date[0],
                    Year: date[2]
                };
                if (model.get('save_archive')) {
                    model.set({iframe_url:url + $.param(date)});
                } else {
                    model.clear_interval();
                    $.ajax({
                        type: 'POST',
						beforeSend: function (xhr) {
							xhr.setRequestHeader('Content-type', 'application/json')
						},
						contentType: 'application/json; charset=utf-8',
                        url: url,
                        dataType: 'json',
                        data: JSON.stringify(date),
                        success: function (data) {
							model.set({'update_msgs':false},{silent:true});
                            model.set({'auto_refresh':false});
                            model.parse_msgs($.parseJSON(data));
                        }
                    });
                }
            }
        }
    });

    _.extend(window.GSWAT.prototype.model_definitions, {chat_model:chat_model});
}(window, jQuery, _, moment));