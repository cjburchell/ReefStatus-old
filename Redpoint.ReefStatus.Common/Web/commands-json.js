function keyCommand(button) {
    Request("command/key", "command=" + button, "displayCallback);
}

function displayCallback(text) {
    if (refreshing) {
        if (text.lenght != 0) {
            document.getElementById('DisplayText').innerHTML = text[0] + "<br>" + text[1];
        }
        if (iPad) {
            updatePanelContent('control');
        }
    }
}


var timer;
var refreshing = false;
function refreshText(timeout) {
    Request("command/display", "", "displayCallback);

    if (timeout > 0)
        timer = setTimeout("refreshText(" + timeout + ")", timeout);
}

function startRefreshText() {
    refreshing = true;
    refreshText(5000)
}

function stopRefresh() {
    refreshing = false;
    clearTimeout(timer);
}

function refreshDisplay() {
    refreshText(0);
}


function refreshCommand() {
    TextCommand("command/refresh);
}
