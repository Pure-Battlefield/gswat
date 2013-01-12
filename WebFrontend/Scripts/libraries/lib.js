/* Library of small useful global functions */
_.extend(window,{
    lib: {}
});

(function (window, document, $, _, yepnope, undefined) {
    lib = function () {
        this.squads=[["SQUAD0","LONE WOLF"],["SQUAD1","ALPHA"],["SQUAD2","BRAVO"],["SQUAD3","CHARLIE"],["SQUAD4","DELTA"],["SQUAD5","ECHO"],["SQUAD6","FOXTROT"],["SQUAD7","GOLF"],["SQUAD8","HOTEL"],["SQUAD9","INDIA"],["SQUAD10","JULIET"],["SQUAD11","KILO"],["SQUAD12","LIMA"],["SQUAD13","MIKE"],["SQUAD14","NOVEMBER"],["SQUAD15","OSCAR"],["SQUAD16","PAPA"],["SQUAD17","QUEBEC"],["SQUAD18","ROMEO"],["SQUAD19","SIERRA"],["SQUAD20","TANGO"],["SQUAD21","UNIFORM"],["SQUAD22","VICTOR"],["SQUAD23","WHISKEY"],["SQUAD24","XRAY"],["SQUAD25","YANKEE"],["SQUAD26","ZULU"]];
    };

    lib.prototype = {
        parse_chat_messages: function (msgs,squads) {
            var result = '';
            if (typeof msgs == 'object') {
                if (msgs.length > 0) {
                    squads = squads || this.squads;
                    var parsed_messages = [];
                    _.each(msgs, function (message) {
                        var now = new Date(parseInt(message.MessageTimeStamp.replace('/Date(', '')));
                        var timestamp = moment(new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds()));
                        message.MessageTimeStamp = timestamp.format('MM/DD/YYYY HH:mm:ss');
                        message.Server = false;

                        // Match on Squad
                        var re1 = '.*?';	// Non-greedy match on filler
                        var re2 = '(squad)';	// Word 1
                        var re3 = '(\\d+)';	// Integer Number 1

                        var p = new RegExp(re1 + re2 + re3, ["i"]);
                        var m = p.exec(message.MessageType);
                        if (m != null) {
                            squad_name=m[1].toUpperCase()+m[2];
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
                            } else {
                                message.MessageType = 'RU';
                            }
                            parsed_messages.push(message);
                            return;
                        }

                        // Match on Server
                        var re1 = '(server)';	// Word 1
                        var p = new RegExp(re1, ["i"]);
                        var m = p.exec(message.Speaker);
                        if (m != null) {
                            message.MessageType = 'server';
                            message.Server = true;
                            parsed_messages.push(message);
                            return;
                        }

                        // Match on All
                        var re1 = '(all)';	// Word 1

                        var p = new RegExp(re1, ["i"]);
                        var m = p.exec(message.MessageType);
                        if (m != null) {
                            parsed_messages.push(message);
                            return;
                        }
                    });
                    result = { 'status': 'success', 'content': parsed_messages };
                } else {
                    result = { 'status': 'error', 'content': 'No messages found' };
                }
            } else {
                result = {'status':'error','content':'Needs to be an object'};
            }
            return result;
        }
    };
}(window, document, jQuery, _, yepnope));