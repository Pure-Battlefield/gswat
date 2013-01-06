(function(window, $, _, moment) {
    var chat_model = Backbone.Model.extend({
        defaults : {
            'auto_refresh': true,
            'url': '/api/values/getallmessages',
            'interval': 1000,
            'fetch_interval': {},
            'show_server_msgs': false,
            'team_1_msgs': [],
            'team_2_msgs': [],
            'server_msgs': [],
            'server_set': false,
            'squads': [["SQUAD1", "ALPHA"],["SQUAD2", "BRAVO"],["SQUAD3", "CHARLIE"],["SQUAD4", "DELTA"],["SQUAD5", "ECHO"],["SQUAD6", "FOXTROT"],["SQUAD7", "GOLF"],["SQUAD8", "HOTEL"],["SQUAD9", "INDIA"],["SQUAD10", "JULIET"],["SQUAD11", "KILO"],["SQUAD12", "LIMA"],["SQUAD13", "MIKE"],["SQUAD14", "NOVEMBER"],["SQUAD15", "OSCAR"],["SQUAD16", "PAPA"],["SQUAD17", "QUEBEC"],["SQUAD18", "ROMEO"],["SQUAD19", "SIERRA"],["SQUAD20", "TANGO"],["SQUAD21", "UNIFORM"],["SQUAD22", "VICTOR"],["SQUAD23", "WHISKEY"],["SQUAD24", "XRAY"],["SQUAD25", "YANKEE"],["SQUAD26", "ZULU"]]
        },

        url: function(){
            return this.get('url');
        },

        initialize: function () {
            this.set_interval();
        },

        set_interval: function (interval) {
            this.clear_interval();
            if(this.get('server_set')){
                interval = interval || this.get('interval');
                /*var fetch_interval = _.bind(function () {
                    window.setInterval(function () {
                        model.fetch();
                    }, interval)
                }, this);
                this.set({ 'fetch_interval': fetch_interval });*/


                var _update = _.bind(function () {
                    this._intervalFetch = window.setTimeout(_update, interval || 1000);
                }, this);

                _update();
            } else {
                console.log('no server set');
            }
        },

        clear_interval: function () {
            window.clearTimeout(this._intervalFetch);
            delete this._intervalFetch;
        },

        parse: function (data) {
            var squads = this.get('squad');
            var team_1 = this.get('team_1_msgs');
            var team_2 = this.get('team_2_msgs');
            var server = this.get('server_msgs');

            data = $.parseJSON(data);
            console.log(data);

            _.each(data, function (message) {
                var now = new Date(parseInt(message.MessageTimeStamp.replace('/Date(', '')));
                var timestamp = moment(new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds()));
                message.MessageTimeStamp = timestamp.format('MM/DD/YYYY HH:mm:ss');

                // Rename channels to more verbose names.
                var team = '';
                team = message.team_slug = MessageType.replace('TEAM1', 'US');
                team = message.team_slug = MessageType.replace('TEAM2', 'RU');
                var squad_name = MessageType.replace('SQUAD', ' SQUAD');

                // Match on Team
                var re1 = '(team)';	// Word 1
                var re2 = '(\\d+)';	// Integer Number 1

                var p = new RegExp(re1 + re2, ["i"]);
                var m = p.exec(txt);
                if (m != null) {
                    console.log(m[1], m[2]);
                }

                // Match on Squad
                var re1 = '.*?';	// Non-greedy match on filler
                var re2 = '(squad)';	// Word 1
                var re3 = '(\\d+)';	// Integer Number 1

                var p = new RegExp(re1 + re2 + re3, ["i"]);
                var m = p.exec(txt);
                if (m != null) {
                    console.log(m[1], m[2]);
                    for (var i = 0; i < squads.length; i++) {
                        squad = _.find(squad, function(squad){
                            return squad[0] == squad_name;
                        });
                            //.replace(squads[i][0], squads[i][1]);
                    }
                }


                // Match on Server
                var p = new RegExp(re1, ["i"]);
                var m = p.exec(txt);
                if (m != null) {
                    console.log(m[1]);
                }
            });
            return data;
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