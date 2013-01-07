(function(window, $, _, moment) {
    var chat_model = Backbone.Model.extend({
        defaults : {
            'auto_refresh': true,
            'url': '/api/values/getallmessages',
            'interval': 1,
            'fetch_interval': {},
            'show_server_msgs': false,
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
                //console.log('no server set');
            }
        },

        clear_interval: function () {
            window.clearTimeout(this._intervalFetch);
            delete this._intervalFetch;
        },

        parse_msgs: function (data) {
            //data = $.parseJSON("[{\"MessageTimeStamp\":\"\\/Date(1357539261774)\\/\",\"Speaker\":\"Scorched_Waffles\",\"Text\":\"c next\",\"MessageType\":\"team2squad5\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447615382257185\",\"Timestamp\":\"\\/Date(1357539263283)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A14%3A23.2831141Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539264148)\\/\",\"Speaker\":\"Leg0lasSn1p3r\",\"Text\":\"fuck\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447615358515247\",\"Timestamp\":\"\\/Date(1357539265683)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A14%3A25.6831141Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539266272)\\/\",\"Speaker\":\"Leg0lasSn1p3r\",\"Text\":\"man\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447615337271687\",\"Timestamp\":\"\\/Date(1357539267783)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A14%3A27.7831141Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539270240)\\/\",\"Speaker\":\"Estemachine\",\"Text\":\"fuck me\",\"MessageType\":\"team2\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447615297599214\",\"Timestamp\":\"\\/Date(1357539271683)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A14%3A31.6831141Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539272583)\\/\",\"Speaker\":\"Leg0lasSn1p3r\",\"Text\":\"bad luck\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447615274168463\",\"Timestamp\":\"\\/Date(1357539274083)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A14%3A34.0831141Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539282940)\\/\",\"Speaker\":\"MORTi86\",\"Text\":\"gg\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447615170599074\",\"Timestamp\":\"\\/Date(1357539284383)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A14%3A44.3831141Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539290733)\\/\",\"Speaker\":\"Estemachine\",\"Text\":\"gg my ass\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447615092667668\",\"Timestamp\":\"\\/Date(1357539292184)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A14%3A52.1841141Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539290873)\\/\",\"Speaker\":\"theSlime\",\"Text\":\"gg\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447615091262171\",\"Timestamp\":\"\\/Date(1357539292383)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A14%3A52.3831141Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539291701)\\/\",\"Speaker\":\"KilluminattiBSB\",\"Text\":\"gg\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447615082984845\",\"Timestamp\":\"\\/Date(1357539293182)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A14%3A53.1821141Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539293575)\\/\",\"Speaker\":\"Comby_McBeardz\",\"Text\":\"gg\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447615064240542\",\"Timestamp\":\"\\/Date(1357539295083)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A14%3A55.0831141Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539408818)\\/\",\"Speaker\":\"SirloinSandvich\",\"Text\":\"wtf are you doing?\",\"MessageType\":\"team1\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447613911818482\",\"Timestamp\":\"\\/Date(1357539410161)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A16%3A50.1615451Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539451944)\\/\",\"Speaker\":\"Server\",\"Text\":\"Admins online: destroyermaker\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447613480555726\",\"Timestamp\":\"\\/Date(1357539453164)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A17%3A33.1645451Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539452053)\\/\",\"Speaker\":\"Server\",\"Text\":\"Type !listadmins to see this list at any time.\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447613479460898\",\"Timestamp\":\"\\/Date(1357539453460)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A17%3A33.4605451Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539485433)\\/\",\"Speaker\":\"KilluminattiBSB\",\"Text\":\"go\",\"MessageType\":\"team1\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447613145667188\",\"Timestamp\":\"\\/Date(1357539486661)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A18%3A06.6615451Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539553754)\\/\",\"Speaker\":\"PurpleDerple\",\"Text\":\"dat lag\",\"MessageType\":\"team1\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447612462453874\",\"Timestamp\":\"\\/Date(1357539554374)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A19%3A14.3744556Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539561752)\\/\",\"Speaker\":\"destroyermaker\",\"Text\":\"we dont need a\",\"MessageType\":\"team2squad2\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447612382476623\",\"Timestamp\":\"\\/Date(1357539562272)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A19%3A22.2724556Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539595538)\\/\",\"Speaker\":\"Server\",\"Text\":\"Welcome to PURE BATTLEFIELD!\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447612044619747\",\"Timestamp\":\"\\/Date(1357539596074)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A19%3A56.0744556Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539595725)\\/\",\"Speaker\":\"Server\",\"Text\":\"Please type !help for server rules and other commands.\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447612042747300\",\"Timestamp\":\"\\/Date(1357539596273)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A19%3A56.2734556Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539595912)\\/\",\"Speaker\":\"Server\",\"Text\":\"We are an open gaming community; all are welcome.\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447612040870410\",\"Timestamp\":\"\\/Date(1357539596473)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A19%3A56.4734556Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539596116)\\/\",\"Speaker\":\"Server\",\"Text\":\"Join us at purebattlefield.reddit.com!\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447612038839298\",\"Timestamp\":\"\\/Date(1357539596673)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A19%3A56.6734556Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539618530)\\/\",\"Speaker\":\"KilluminattiBSB\",\"Text\":\"wtf man go..\",\"MessageType\":\"team1\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447611814695160\",\"Timestamp\":\"\\/Date(1357539619101)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A20%3A19.1014556Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539625340)\\/\",\"Speaker\":\"theSlime\",\"Text\":\"left\",\"MessageType\":\"team2squad6\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447611746593761\",\"Timestamp\":\"\\/Date(1357539625889)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A20%3A25.8894556Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539628558)\\/\",\"Speaker\":\"Server\",\"Text\":\"PURE BATTLEFIELD servers are run by the community, for the community. All players are welcome to wear the [PURE] tag!\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447611714414366\",\"Timestamp\":\"\\/Date(1357539629174)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A20%3A29.1744556Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539646037)\\/\",\"Speaker\":\"Scorched_Waffles\",\"Text\":\"ouch!\",\"MessageType\":\"team2squad3\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447611539628751\",\"Timestamp\":\"\\/Date(1357539646573)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A20%3A46.5734556Z\\u0027\\\"\"},{\"MessageTimeStamp\":\"\\/Date(1357539649098)\\/\",\"Speaker\":\"YooooJoe\",\"Text\":\"wow ty dickhead\",\"MessageType\":\"all\",\"PartitionKey\":\"20130107\",\"RowKey\":\"2520447611509012335\",\"Timestamp\":\"\\/Date(1357539649572)\\/\",\"ETag\":\"W/\\\"datetime\\u00272013-01-07T06%3A20%3A49.5724556Z\\u0027\\\"\"}]");
            //this.clear_interval();
            if (data.length > 0) {
                var squads = this.get('squads');
                var all = this.get('all_msgs');

                _.each(data, function (message) {
                    var now = new Date(parseInt(message.MessageTimeStamp.replace('/Date(', '')));
                    var timestamp = moment(new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds()));
                    message.MessageTimeStamp = timestamp.format('MM/DD/YYYY HH:mm:ss');

                    // Match on Squad
                    var re1 = '.*?';	// Non-greedy match on filler
                    var re2 = '(squad)';	// Word 1
                    var re3 = '(\\d+)';	// Integer Number 1

                    var p = new RegExp(re1 + re2 + re3, ["i"]);
                    var m = p.exec(message.MessageType);
                    if (m != null) {
                        squad_name = m[1].toUpperCase() + m[2];
                        message.SquadName = _.find(squads, function (squad) {
                            return squad[0] == squad_name;
                        })[1];
                    }

                    // Match on Team
                    var re1 = '(team)';	// Word 1
                    var re2 = '(\\d+)';	// Integer Number 1

                    var p = new RegExp(re1 + re2, ["i"]);
                    var m = p.exec(message.MessageType);
                    if (m != null) {
                        if (m[2] == "1") {
                            message.MessageType = 'US';
                            //team_1.push(message);
                        } else {
                            message.MessageType = 'RU';
                            //team_2.push(message);
                        }
                        return;
                    }


                    // Match on Server
                    var re1 = '(server)';	// Word 1
                    var p = new RegExp(re1, ["i"]);
                    var m = p.exec(message.MessageType);
                    if (m != null) {
                        //server.push(message);
                        return;
                    }

                    // Match on All
                    var re1 = '(all)';	// Word 1

                    var p = new RegExp(re1, ["i"]);
                    var m = p.exec(message.MessageType);
                    if (m != null) {
                        //all.push(message);
                        return;
                    }
                });
                var msgs = {};
                msgs.all_msgs = _.union(all, data);
                msgs.update_msgs = true;
                this.set(msgs);
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