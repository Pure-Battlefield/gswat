function loadMessageQueue()
{
    xmlHttpRequestMessageQueue();

    window.setInterval(function () {
        xmlHttpRequestMessageQueue();
    }, 1000 /* A.K.A. 1 second */);
}


// Avoid having to write the same code twice
// TODO Change the name of this function to something else that's not as long
function xmlHttpRequestMessageQueue() {
    // Testing purposes
    console.log('Refreshing..');

    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }

    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            document.getElementById("chatContents").innerHTML = xmlhttp.responseText;
        }
    };
    xmlhttp.open("GET", "/api/values", true);
    xmlhttp.send();
}
