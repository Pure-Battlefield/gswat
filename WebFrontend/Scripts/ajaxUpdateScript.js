var parser;

function loadMessageQueue(interval)
{
    // Default is 1 second
    if (interval === undefined) {
        interval = 1;
    }

    if (parser !== undefined) { // Clear the old parser interval, if it existed.
        window.clearInterval(parser);
    }
       
    parseJSON();
    parser = window.setInterval(parseJSON, interval * 1000);
    console.log('Chat refresh rate set to ' + interval + ' seconds');
}


// Avoid having to write the same code twice
function parseJSON() {
    $.get('/api/values', function (data) {
        data = JSON.parse(data);

        var content = "";
        for (var message in data) {
            var timestamp = moment(new Date(parseInt(data[message].MessageTimeStamp.replace('/Date(', ''))));
            timestamp = timestamp.format("MM/DD/YYYY HH:mm:ss");
            var channel = data[message].MessageType.toUpperCase();
            var speaker = data[message].Speaker;
            var text = data[message].Text;

            var stmt = "<p>[%ts] [%c] <strong>%s</strong>: %t</p>";
            stmt = stmt.replace('%ts', timestamp)
                .replace('%c', channel)
                .replace('%s', speaker)
                .replace('%t', text);
            content += stmt;
        }
        $('#chatContents').empty().append(content);
    });

    console.log('Refreshing chat..');
}
