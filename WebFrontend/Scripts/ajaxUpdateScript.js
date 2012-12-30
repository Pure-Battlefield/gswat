/*
    The last known JSON parser, which is what sets the chat refresh interval.
    We need this to be traceable because when the refresh interval is changed,
    the previous interval must be killed.
 */
var parser;

// Whether or not to continuously load chat messages.  Disabled for historical chat message loading.
var loop;

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
        var timestamp = moment(new Date(parseInt(data[message].MessageTimeStamp.replace('/Date(', ''))));
        timestamp = timestamp.format('MM/DD/YYYY HH:mm:ss');

        // The channel this user is in (ALL, SQUAD#, etc.)
        var channel = data[message].MessageType.toUpperCase();

        // The speaker, or user saying the message.
        var speaker = data[message].Speaker;

        // The message body.
        var text = data[message].Text;

        // Prepare the HTML statement to be written.
        var stmt = '<p>[%ts] [%c] <strong>%s</strong>: %t</p>';

        // Rename channels to more verbose names.
        switch (channel) {
            case 'TEAM1':
                channel = 'US';
                break;
            case 'TEAM2':
                channel = 'RU';
                break;
            default:
                channel = 'SQUAD';
        }

        // Color codes for channels
        var colorCodes = {
            US: '0000ff',
            RU: 'ce2323', // ec5800
            SQUAD: '28ae00' // 00ff00
        };

        stmt = stmt.replace('%ts', timestamp)
            .replace('%c', '<span style="color:#' + colorCodes[channel] + '">' + channel + '</span>')
            .replace('%s', speaker)
            .replace('%t', text);

        // Append the prepared statement to 'content', which is the chunk of HTML data of all the messages.
        content += stmt;
    }

    // Clean out the chat div and add the new stuff.  This is significantly faster than it sounds it would be.
    $('#chatContents').empty().append(content);
}
