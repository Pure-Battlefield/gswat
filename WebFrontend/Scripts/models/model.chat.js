(function(window, $, _, moment) {
    var chat_model = Backbone.Model.extend({
        defaults : {
            'auto_refresh': true,
            'url': '/api/values/getallmessages',
            'interval': 1,
            'fetch_interval': {},
            'show_server_msgs': false,
            'team_1_msgs': [],
            'team_2_msgs': [],
            'server_msgs': [],
            'public_msgs': [],
            'all_msgs': [],
            'server_set': false,
            'update_msgs':false,
            //TODO: Fetch squad definitions from server
            'squads': [["SQUAD1", "ALPHA"],["SQUAD2", "BRAVO"],["SQUAD3", "CHARLIE"],["SQUAD4", "DELTA"],["SQUAD5", "ECHO"],["SQUAD6", "FOXTROT"],["SQUAD7", "GOLF"],["SQUAD8", "HOTEL"],["SQUAD9", "INDIA"],["SQUAD10", "JULIET"],["SQUAD11", "KILO"],["SQUAD12", "LIMA"],["SQUAD13", "MIKE"],["SQUAD14", "NOVEMBER"],["SQUAD15", "OSCAR"],["SQUAD16", "PAPA"],["SQUAD17", "QUEBEC"],["SQUAD18", "ROMEO"],["SQUAD19", "SIERRA"],["SQUAD20", "TANGO"],["SQUAD21", "UNIFORM"],["SQUAD22", "VICTOR"],["SQUAD23", "WHISKEY"],["SQUAD24", "XRAY"],["SQUAD25", "YANKEE"],["SQUAD26", "ZULU"]]
        },

        get_msgs: function(){
            this.set({ 'update_msgs': false }, { silent: true });
            var model = this;
            var url = this.get('url');
            $.get(url, function (data) {
                model.parse_msgs($.parseJSON(data));
            });
        },

        initialize: function () {
            this.set_interval();
        },

        update_settings: function (settings) {

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
                console.log('no server set');
            }
        },

        clear_interval: function () {
            window.clearTimeout(this._intervalFetch);
            delete this._intervalFetch;
        },

        parse_msgs: function (data) {
            //data = $.parseJSON("[{\"MessageTimeStamp\":\"\\/Date(1357535408440)\\/\",\"Speaker\":\"alaskan_fireman\",\"Text\":\"so many in the stairwell\",\"MessageType\":\"team2\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447653915596166\",\"Timestamp\":\"\\/Date(1357535410330)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A10%3A10.3308698Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535453774)\\/\",\"Speaker\":\"Schtikaio\",\"Text\":\"BASERAPING, GG RETARD\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447653462256615\",\"Timestamp\":\"\\/Date(1357535455630)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A10%3A55.6308698Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535489063)\\/\",\"Speaker\":\"KiriONE\",\"Text\":\"we\\u0027ve gotta stop meeting like this XC!\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447653109361426\",\"Timestamp\":\"\\/Date(1357535490933)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A11%3A30.9338698Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535491500)\\/\",\"Speaker\":\"Firstnecron\",\"Text\":\"wtf\",\"MessageType\":\"team1\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447653084991508\",\"Timestamp\":\"\\/Date(1357535493430)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A11%3A33.4308698Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535502310)\\/\",\"Speaker\":\"OmNomMonst3r\",\"Text\":\"couple going for B\",\"MessageType\":\"team2\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447652976890413\",\"Timestamp\":\"\\/Date(1357535504233)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A11%3A44.2338698Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535573327)\\/\",\"Speaker\":\"CincoKilos\",\"Text\":\"need help on C\",\"MessageType\":\"team1\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447652266723697\",\"Timestamp\":\"\\/Date(1357535575534)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A12%3A55.5349474Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535658215)\\/\",\"Speaker\":\"ffejeromdiks\",\"Text\":\"good smoke use team\",\"MessageType\":\"team2\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447651417842048\",\"Timestamp\":\"\\/Date(1357535660683)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A14%3A20.6837652Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535676587)\\/\",\"Speaker\":\"Server\",\"Text\":\"Next map: Armored Shield\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447651234129515\",\"Timestamp\":\"\\/Date(1357535679084)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A14%3A39.0846051Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535676884)\\/\",\"Speaker\":\"Server\",\"Text\":\"Welcome to PURE BATTLEFIELD!\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447651231159976\",\"Timestamp\":\"\\/Date(1357535679384)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A14%3A39.3846351Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535677087)\\/\",\"Speaker\":\"Server\",\"Text\":\"Please type !help for server rules and other commands.\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447651229128625\",\"Timestamp\":\"\\/Date(1357535679585)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A14%3A39.5856552Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535677289)\\/\",\"Speaker\":\"Server\",\"Text\":\"We are an open gaming community; all are welcome.\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447651227100086\",\"Timestamp\":\"\\/Date(1357535679784)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A14%3A39.7846751Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535677493)\\/\",\"Speaker\":\"Server\",\"Text\":\"Join us at purebattlefield.reddit.com!\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447651225065906\",\"Timestamp\":\"\\/Date(1357535679984)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A14%3A39.9846951Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535694286)\\/\",\"Speaker\":\"alaskan_fireman\",\"Text\":\"nice spawn\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447651057134885\",\"Timestamp\":\"\\/Date(1357535696787)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A14%3A56.7873752Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535701878)\\/\",\"Speaker\":\"alaskan_fireman\",\"Text\":\"right up my ass\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650981210863\",\"Timestamp\":\"\\/Date(1357535704387)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A15%3A04.3871351Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535705120)\\/\",\"Speaker\":\"Firstnecron\",\"Text\":\"well we lost\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650948799730\",\"Timestamp\":\"\\/Date(1357535707587)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A15%3A07.5874551Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535707955)\\/\",\"Speaker\":\"Scorched_Waffles\",\"Text\":\"snipers piss me off\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650920444134\",\"Timestamp\":\"\\/Date(1357535710387)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A15%3A10.3877351Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535713345)\\/\",\"Speaker\":\"Scorched_Waffles\",\"Text\":\"you don\\u0027t want to piss me off\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650866548537\",\"Timestamp\":\"\\/Date(1357535715794)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A15%3A15.7942757Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535723249)\\/\",\"Speaker\":\"OmNomMonst3r\",\"Text\":\"you turn green?\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650767506585\",\"Timestamp\":\"\\/Date(1357535725693)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A15%3A25.6932655Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535727780)\\/\",\"Speaker\":\"l--coldFuSion--l\",\"Text\":\"he\\u0027s gonna HULK OUT\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650722197090\",\"Timestamp\":\"\\/Date(1357535730290)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A15%3A30.2907252Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535729404)\\/\",\"Speaker\":\"Scorched_Waffles\",\"Text\":\"lol - yeah!\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650705953966\",\"Timestamp\":\"\\/Date(1357535731889)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A15%3A31.8898851Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535748213)\\/\",\"Speaker\":\"OmNomMonst3r\",\"Text\":\"bunch going c\",\"MessageType\":\"team2\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650517865397\",\"Timestamp\":\"\\/Date(1357535750690)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A15%3A50.690765Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535776270)\\/\",\"Speaker\":\"Scorched_Waffles\",\"Text\":\"support guys piss me off\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650237297663\",\"Timestamp\":\"\\/Date(1357535778795)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A16%3A18.7955752Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535789111)\\/\",\"Speaker\":\"Fuzalert\",\"Text\":\"could u stfu\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650108885828\",\"Timestamp\":\"\\/Date(1357535791595)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A16%3A31.5958551Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535798266)\\/\",\"Speaker\":\"Firstnecron\",\"Text\":\"every guy piss me off\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447650017337868\",\"Timestamp\":\"\\/Date(1357535800796)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A16%3A40.7967751Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357535802545)\\/\",\"Speaker\":\"Scorched_Waffles\",\"Text\":\"fuzalert pisses me off\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447649974540002\",\"Timestamp\":\"\\/Date(1357535804997)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T05%3A16%3A44.9971951Z\\u0027\\\"\"}]");
            this.clear_interval();
            if (data.length > 0) {
                var squads = this.get('squad');
                var all = this.get('public_msgs');
                var team_1 = this.get('team_1_msgs');
                var team_2 = this.get('team_2_msgs');
                var server = this.get('server_msgs');
                var archive = this.get('all_msgs');
                //console.log(data);

                _.each(data, function (message) {
                    //console.log(message);
                    var now = new Date(parseInt(message.MessageTimeStamp.replace('/Date(', '')));
                    var timestamp = moment(new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds()));
                    message.MessageTimeStamp = timestamp.format('MM/DD/YYYY HH:mm:ss');

                    // Rename channels to more verbose names.
                    var team = '';
                    team = message.team_slug = message.MessageType.replace('TEAM1', 'US');
                    team = message.team_slug = message.MessageType.replace('TEAM2', 'RU');
                    var squad_name = message.MessageType.replace('SQUAD', ' SQUAD');
                    //console.log('messagetype', message.MessageType);

                    // Match on Team
                    var re1 = '(team)';	// Word 1
                    var re2 = '(\\d+)';	// Integer Number 1

                    var p = new RegExp(re1 + re2, ["i"]);
                    var m = p.exec(message.MessageType);
                    if (m != null) {
                        if (m[2] == "1") {
                            team_1.push(message);
                        } else {
                            team_2.push(message);
                        }
                        return;
                    }

                    // Match on Squad
                    var re1 = '.*?';	// Non-greedy match on filler
                    var re2 = '(squad)';	// Word 1
                    var re3 = '(\\d+)';	// Integer Number 1

                    var p = new RegExp(re1 + re2 + re3, ["i"]);
                    var m = p.exec(message.MessageType);
                    if (m != null) {
                        console.log('squad',m[1], m[2]);
                        for (var i = 0; i < squads.length; i++) {
                            squad = _.find(squad, function (squad) {
                                return squad[0] == squad_name;
                            });
                            //.replace(squads[i][0], squads[i][1]);
                        }
                    }


                    // Match on Server
                    var re1 = '(server)';	// Word 1
                    var p = new RegExp(re1, ["i"]);
                    var m = p.exec(message.MessageType);
                    if (m != null) {
                        server.push(message);
                        return;
                    }

                    // Match on All
                    var re1 = '(all)';	// Word 1

                    var p = new RegExp(re1, ["i"]);
                    var m = p.exec(message.MessageType);
                    if (m != null) {
                        all.push(message);
                        return;
                    }
                });
                var msgs = {};
                msgs.all_msgs = _.union(archive, data);
                msgs.update_msgs = true;
                this.set(msgs);
                //console.log(this.attributes);
                //return msgs;
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