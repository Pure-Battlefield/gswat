// The auto-refresh rate of chat messages in the client (in seconds).
var chatRefreshRate = 1;


/**
 * Shows a 3.5 second pop-up at the bottom of the screen, meant to be a
 * asynchronous notifier of successful settings changes.
 * 
 * @param type      The type of message, this is expected to be exact.
 * @param data      (If applicable) a piece of data to go in the message.
 */
function footerPopup(type, data, doFade) {
    doFade = typeof a !== 'undefined' ? doFade : true; //Automatically fade out message after 3500 ms if not specified
    // List of valid 'types' of messages that can be printed.
    var validTypes = {
        chatRefreshRate: 'Chat will now be refreshed every %data% seconds.',
        chatRefreshForced: 'Chat messages refreshed.',
        serverSettings: 'Connected to new server successfully at %data%.',
        loadingLogData: 'Loading Chat Log Data.'
    };

    // Construct the message itself
    var message;
    $.each(validTypes, function (nextType, nextMessage) { // Try to find the message's type.
        if (nextType === type) { // If it does exist, then set the message variable.
            message = nextMessage;
            if (nextMessage.indexOf('%data%') != -1 && data !== undefined) { // Replace dummy with live data.
                message = message.replace('%data%', data);
            }
        }
    });

    // Show the pop-up.
    $('#footerPopup').text(message).fadeIn('slow', function () {
        if (doFade) {
            return $(this).delay(3500).fadeOut('slow');
        }
    });
}


// Always select all text inside when an input is focused on.
$('input[type="text"]').click(function () {
    this.select();
});


// Toggle the pop-out settings box
function toggleSettingsBox() {
    $('#settingsBox').toggle();   // First, hide or show the box.
    $('#settingsBtn').attr('src', $('#settingsBtn').attr('src').indexOf('_') == -1 ?
        '/Images/settingsBtn_active.png' :
        '/Images/settingsBtn.png'
    );  // Now, change the image to be (un)/highlighted
}

// Toggle it whenever the settings button is clicked
$('#settingsBtn').click(function () {
    toggleSettingsBox()
});


/*
    When RConn (server) settings are switched around, we have to stop the form from
    submitting how it wants to, and use our own AJAX POST call.  
 */
$('#serverSettingsForm').submit(function (event) {
    // Prevent default form submission
    event.preventDefault();
    
    // Store the form's data into memory
    var ip = $('#ip').val(),
        port = $('#port').val(),
        passwd = $('#passwd').val(),
        oldPasswd = $('#oldPasswd').val();

    // Clear form data
    $('#ip').val("");
    $('#port').val("");
    $('#passwd').val("");
    $('#oldPasswd').val("");

    // Pack the above data into something that can be sent over JSON
    var packedData = {
        ServerIP: ip,
        ServerPort: port,
        Password: passwd,
        OldPassword: oldPasswd
    };

    // Send the AJAX POST data (as stringified JSON)
    $.ajax({
        type: 'POST',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Content-type', 'application/json')
        },
        contentType: 'application/json; charset=utf-8',
        url: '/api/values/setserverinfo',
        data: JSON.stringify(packedData),
        dataType: 'json',
        success: footerPopup('serverSettings', ip + ':' + port)
    });

    // Since a form was submitted, hide the settings box.
    toggleSettingsBox();
});


// When the chat refresh rate form is submitted, respond accordingly.
$('#refreshRateSubmit').on('click', function () {
    // Set the new refresh rate (refreshRate is declared above)
    refreshRate = $('#refreshRateInput').val();

    // Load the message queue with this new refresh rate interval.
    loadMessageQueue(refreshRate);

    // Show the pop-up footer message
    footerPopup('chatRefreshRate', refreshRate);

    // Toggle the settings box since a form was submitted
    toggleSettingsBox();
});


// When the user forces chat to be refreshed
$('#forceRefreshBtn').click(function () {
    loop = true;
    footerPopup('chatRefreshForced');
});


$('#historicalChatForm').submit(function (data) {
    // Do NOT try to continuously load messages (these are historical chat messages!)
    loop = false;

    event.preventDefault();
    var date = $('#chatDate').val();
    console.log(date);
    var splitDate = date.split("/");
    var dateTime = {
        Day: splitDate[0],
        Month: splitDate[1],
        Year: splitDate[2]
    };
    console.log(dateTime);
    footerPopup('loadingLogData', null, false);

    $.ajax({
        type: 'GET',
        url: '/api/values/getbyday/',
        dataType: 'json',
        data: dateTime,
        success: function (data) {
            parseJSON(data);
            $('#footerPopup').fadeOut('slow');
        }
    });
});