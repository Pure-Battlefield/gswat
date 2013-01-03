/*
    The last known JSON parser, which is what sets the chat refresh interval.
    We need this to be traceable because when the refresh interval is changed,
    the previous interval must be killed.
 */
var parser;

// Whether or not to continuously load chat messages.  Disabled for historical chat message loading.
var loop;

// Scroll pane API reference
var scrollPaneAPI;

// First launch
var first;

    function startup() {
        first = true;
        var scrollPane = $('#chatContents').jScrollPane({
                verticalDragMinHeight: 20
            });
        scrollPaneAPI = scrollPane.data('jsp');
        loadMessageQueue();
    }

    /**
     * Load the queue of chat messages.
     * 
     * @param interval      How often to refresh the queue.
     */
    function loadMessageQueue(interval) {
        if (interval === undefined) { // Set the interval to the default if it's not defined explicitly.
            interval = 1;
        }

        if (parser !== undefined) { // Clear the old parser interval, if it existed.
            window.clearInterval(parser);
        }

        // Since we're loading at a periodic interval, definitely need to loop loading.
        loop = true;

        getCurrentChat();

        // Set the parser to load at the interval specified.
        parser = window.setInterval(getCurrentChat, interval * 1000);
    }


    // Get all current chat messages.
    function getCurrentChat() {
        if (loop) {
            $.get('/api/values/getallmessages', function (data) {
                parseJSON(data);
            });
        }
    }


    /**
     * Highly re-used:  parse the JSON data sent by the server which details chat messages.
     *
     * @param data      Raw data sent by the server to be decoded as JSON.
     */
    function parseJSON(data) {
        // Parse the data into JSON to be decoded.
        data = JSON.parse(data);

        var content = '';
        for (var message in data) { // Loop through each chat message in the data sent
            // The timestamp hooked to the current chat message.
            var now = new Date(parseInt(data[message].MessageTimeStamp.replace('/Date(', '')));
            var timestamp = moment(new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(),  now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds()));
            timestamp = timestamp.format('MM/DD/YYYY HH:mm:ss');

            // The channel this user is in (ALL, SQUAD#, etc.)
            var channel = data[message].MessageType.toUpperCase();

            // The speaker, or user saying the message.
            var speaker = data[message].Speaker;

            // The message body.
            var text = data[message].Text;

            // Rename channels to more verbose names.
            channel = channel.replace('TEAM1', '<span style="color:#0000ff">US</span>');
            channel = channel.replace('TEAM2', '<span style="color:#ce2323">RU</span>');
            channel = channel.replace('SQUAD', ' <span style="color:#28ae00">SQUAD');
        
            // Prepare the HTML statement to be written.
            var stmt = '<p>[%ts] <strong>[%c</span>] %s</strong>: %t</p>';
            if (channel.indexOf('SQUAD') > -1) {
                stmt = '<p>[%ts] <strong>[%c</span>] %s</strong>: %t</p>';
            }
            else if (channel.substring(0, 2) == 'US') {
                stmt = '<p>[%ts] <strong>[%c</span>] %s</strong>: %t</p>';
            }
            else if (channel.substring(0, 2) == 'RU') {
                stmt = '<p>[%ts] <strong>[%c</span>] %s</strong>: %t</p>';
            }

            stmt = stmt.replace('%ts', timestamp)
                .replace('%c', channel)
                .replace('%s', speaker)
                .replace('%t', text);

            var squads = [
                ["SQUAD1", "ALPHA"],
                ["SQUAD2", "BRAVO"],
                ["SQUAD3", "CHARLIE"],
                ["SQUAD4", "DELTA"],
                ["SQUAD5", "ECHO"],
                ["SQUAD6", "FOXTROT"],
                ["SQUAD7", "GOLF"],
                ["SQUAD8", "HOTEL"],
                ["SQUAD9", "INDIA"],
                ["SQUAD10", "JULIET"],
                ["SQUAD11", "KILO"],
                ["SQUAD12", "LIMA"],
                ["SQUAD13", "MIKE"],
                ["SQUAD14", "NOVEMBER"],
                ["SQUAD15", "OSCAR"],
                ["SQUAD16", "PAPA"],
                ["SQUAD17", "QUEBEC"],
                ["SQUAD18", "ROMEO"],
                ["SQUAD19", "SIERRA"],
                ["SQUAD20", "TANGO"],
                ["SQUAD21", "UNIFORM"],
                ["SQUAD22", "VICTOR"],
                ["SQUAD23", "WHISKEY"],
                ["SQUAD24", "XRAY"],
                ["SQUAD25", "YANKEE"],
                ["SQUAD26", "ZULU"]
            ];
        
            // This isn't quite the prettiest solution, but it works
            for (var i = 0; i < squads.length; i++) {
                stmt = stmt.replace(squads[i][0], squads[i][1]);
            }

            // Append the prepared statement to 'content', which is the chunk of HTML data of all the messages.
            content += stmt;
        }

        // Clean out the chat div and add the new stuff.  This is significantly faster than it sounds it would be.
        //$('#chatContents').empty().append(content);
        var scroll = true;
        if (scrollPaneAPI.getPercentScrolledY() < 100) {
            scroll = false;
        }
        scrollPaneAPI.getContentPane().empty().append(content);
        scrollPaneAPI.reinitialise();
        if(scroll || first) {
            scrollPaneAPI.scrollToBottom();
            first = false;
        }
    }
