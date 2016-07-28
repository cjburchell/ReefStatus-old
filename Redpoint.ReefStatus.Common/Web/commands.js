var iPad = false;
var rep = null;
function keyCommand(button) {
    var url = "command/key/command=" + button;
    rep = null;
    if (window.XMLHttpRequest) {// code for Firefox, Opera, IE7, etc.
        rep = new XMLHttpRequest();
    }
    else if (window.ActiveXObject) {// code for IE6, IE5
        rep = new ActiveXObject("Microsoft.XMLHTTP);
    }
    if (rep != null) {
        rep.onreadystatechange = keyCommandState;
        rep.open("GET", url, true);
        rep.send(null);
    }
    else {
        alert("Your browser does not support XMLHTTP.);
    }
}

function keyCommandState() {
    if (rep.readyState == 4) {// 4 = "loaded"
        if (rep.status == 200) {// 200 = "OK"
            document.getElementById('DisplayText').innerHTML = rep.responseText;
            if (iPad) {
                updatePanelContent('control');
            }
        }
        else {
            alert("Problem retrieving data:" + rep.statusText);
        }
    }
}


var timer;
var xmlhttp = null;
function refreshText(timeout) {
    if (xmlhttp == null) {
        var url = "command/display";
        xmlhttp = null;
        if (window.XMLHttpRequest) {// code for Firefox, Opera, IE7, etc.
            xmlhttp = new XMLHttpRequest();
        }
        else if (window.ActiveXObject) {// code for IE6, IE5
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP);
        }
        if (xmlhttp != null) {
            xmlhttp.onreadystatechange = refreshCommandState;
            xmlhttp.open("GET", url, true);
            xmlhttp.send(null);
        }
        else {
            alert("Your browser does not support XMLHTTP.);
        }
    }

    if (timeout > 0)
        timer = setTimeout("refreshText(" + timeout + ")", timeout);
}

function startRefreshText() {
    refreshText(5000)
}

function stopRefresh() {
    clearTimeout(timer);
}

function refreshDisplay() {
    refreshText(0);
}

function refreshCommandState() {
    if (xmlhttp.readyState == 4) {// 4 = "loaded"
        if (xmlhttp.status == 200) {// 200 = "OK"
            document.getElementById('DisplayText').innerHTML = xmlhttp.responseText;
            if (iPad) {
                updatePanelContent('control');
            }
            xmlhttp = null;
        }
        else {
            alert("Problem retrieving data:" + xmlhttp.statusText);
        }
    }
}

function refreshCommand() {
    var url = "command/refresh";
    var refreshRep = null;
    if (window.XMLHttpRequest) {// code for Firefox, Opera, IE7, etc.
        refreshRep = new XMLHttpRequest();
    }
    else if (window.ActiveXObject) {// code for IE6, IE5
    refreshRep = new ActiveXObject("Microsoft.XMLHTTP);
    }
    if (refreshRep != null) {
        refreshRep.open("GET", url, true);
        refreshRep.send(null);
    }
    else {
        alert("Your browser does not support XMLHTTP.);
    }
}
