(function(window, $, _, moment) {
    var chat_model = Backbone.Model.extend({
        defaults : {
            'auto_refresh': true,
            'url': '/api/values/getallmessages',
            'interval': 1,
            'fetch_interval': {},
            'show_server_msgs': false,
            'all_msgs': [],
            'server_set': true,
            'update_msgs': false,
            'archive_date': '',
            'save_archive': false,
            'iframe_url':''
        },

        initialize: function () {
            this.set_interval();
            this.on('change:archive_date',this.get_old_msgs);
        },

        get_msgs: function () {
            this.set({ 'update_msgs': false }, { silent: true });
            var model = this;
            var url = this.get('url');
            $.get(url, function (data) {
                model.parse_msgs($.parseJSON(data));
            });
        },

        set_interval: function () {
            this.clear_interval();
            if(this.get('server_set')){
                var _update = _.bind(function () {
                    if (this.get('server_set') && this.get('auto_refresh')) {
                        this.get_msgs();
                        interval = this.get('interval') * 1000;
                        this._intervalFetch = window.setTimeout(_update, interval);
                    }
                }, this);

                _update();
            } else {
                //console.log('No server set yet');
            }
        },

        clear_interval: function () {
            window.clearTimeout(this._intervalFetch);
            delete this._intervalFetch;
        },

        parse_msgs: function (data) {
            console.log(data.length);
            if (data.length > 0) {
                var msgs = {};
                data = PBF.lib.parse_chat_messages(data);
                msgs.all_msgs = data.content;
                msgs.update_msgs = true;
                this.set(msgs);
            }
        },

        get_old_msgs: function () {
            var model = this;
            var date = model.get('archive_date').split('/');
            if (date.length === 3) {
                var msgs = {};
                date = {
                    Day: date[1],
                    Month: date[0],
                    Year: date[2]
                };
                if (model.get('save_archive')) {
                    model.set({ iframe_url: '/api/values/downloadbyday/?' + $.param(date) });
                } else {
                    model.clear_interval();
                    $.ajax({
                        type: 'GET',
                        url: '/api/values/getbyday/',
                        dataType: 'json',
                        data: date,
                        success: function (data) {
                            model.set({ 'update_msgs': false }, { silent: true });
                            model.parse_msgs($.parseJSON(data));
                        }
                    });
                }
            }
        }
    });

    var chat_date_model = Backbone.Model.extend({
        defaults: {},

        initialize: function () {
            //
        }
    });

    $.extend(window.GSWAT.prototype.collection_definitions, {
        chat_date_collection: Backbone.Collection.extend({
            model: chat_date_model,

            url: function (day) {
                //return url;
            },

            parse: function (data) {
                console.log(data);
                return data;
            }
        })
    });

    _.extend(window.GSWAT.prototype.model_definitions, { chat_model: chat_model });
}(window, jQuery, _, moment));