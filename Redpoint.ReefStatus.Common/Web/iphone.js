
function ControlerInfo() {
    var infoXmlDoc = OpenXml("command/info");
    if (infoXmlDoc != null) {
        var x = infoXmlDoc.getElementsByTagName("ControlerInfo");
        this.softwareVersion = x[0].getElementsByTagName("SoftwareVersion")[0].childNodes[0].nodeValue;
        this.model = x[0].getElementsByTagName("Model")[0].childNodes[0].nodeValue;
        this.serialNumber = x[0].getElementsByTagName("SerialNumber")[0].childNodes[0].nodeValue;
        this.alarm = x[0].getElementsByTagName("Alarm")[0].childNodes[0].nodeValue;
        this.lastUpdate = x[0].getElementsByTagName("LastUpdateString")[0].childNodes[0].nodeValue;
        this.operationMode = x[0].getElementsByTagName("OperationMode")[0].childNodes[0].nodeValue;
        this.modeId = x[0].getElementsByTagName("ModeId")[0].childNodes[0].nodeValue;

        this.hasReminders = x[0].getElementsByTagName("ReminderCount")[0].childNodes[0].nodeValue != '0';
        this.hasProbes = x[0].getElementsByTagName("ProbeCount")[0].childNodes[0].nodeValue != '0';
        this.hasLevelSensors = x[0].getElementsByTagName("LevelSensorCount")[0].childNodes[0].nodeValue != '0';
        this.hasDigitalInputs = x[0].getElementsByTagName("DigitalInputCount")[0].childNodes[0].nodeValue != '0';
        this.hasSPorts = x[0].getElementsByTagName("SPortCount")[0].childNodes[0].nodeValue != '0';
        this.hasLPorts = x[0].getElementsByTagName("LPortCount")[0].childNodes[0].nodeValue != '0';
        this.hasLights = x[0].getElementsByTagName("LightCount")[0].childNodes[0].nodeValue != '0';
        this.hasDosingPumps = x[0].getElementsByTagName("DosingPumpCount")[0].childNodes[0].nodeValue != '0';
        this.hasUserValues = x[0].getElementsByTagName("UserValueCount")[0].childNodes[0].nodeValue != '0';
    }
}


function operationMode() {
    var info = new ControlerInfo();
    return  info.modeId;
}


function updateInfo() {
    var infoText = "<fieldset class='iphonefieldset'>";
    var info = new ControlerInfo();
    if (info != null) {
        infoText += "<div class=\"row\">";
        infoText += "<label>Version:</label> <input type=\"text\" value=\"" + info.softwareVersion + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Model:</label> <input type=\"text\" value=\"" + info.model + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>S/N:</label><input type=\"text\" value=\"" + info.serialNumber + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Alarm:</label><input type=\"text\" value=\"" + info.alarm + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Mode:</label><input type=\"text\" value=\"" + info.operationMode + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Updated:</label><input type=\"text\" value=\"" + info.lastUpdate + "\" readonly = \"true\"/>";
        infoText += "</div>";
    }

    infoText += "</fieldset>";

    var unlocked = access.IsUnlocked();

    if (unlocked) {
        infoText += "<table cellpadding='3'><tr align='center'><td>";
        infoText += "<button class='whiteButton' id=\"RefreshButton\" type='button' onclick='refreshCommand()'><table><tr align='center'><td><img src=\"icons/Refresh-icon.png\"/></td></tr><tr align='center'><td>Refresh</td></tr></table></button>";
        infoText += "</td><td>";
        infoText += "<a class='whiteButton' id=\"ThunderstormButton\" type='button' href='#thunderDialog'><table><tr align='center'><td><img src=\"icons/thunder-icon.png\"/></td></tr><tr align='center'><td>Thunderstorm</td></tr></table></a>";
        infoText += "</td></tr><tr align='center'><td>";
        infoText += "<button class='whiteButton' id=\"FeedButton\" type='button' onclick='feed()'><table><tr align='center'><td><img src=\"icons/food-icon.png\"/></td></tr><tr align='center'><td>Feed Pause</td></tr></table></button>";
        infoText += "</td><td>";
        infoText += "<button class='whiteButton' id=\"MaintainanceButton\" type='button' onclick='maintainance()'><table><tr align='center'><td><img src=\"icons/wrench.png\"/></td></tr><tr align='center'><td>Maintainance</td></tr></table></button>";
        infoText += "</td></tr></table>";
    }

    if (access.LockEnabled) {
        if (unlocked) {
            infoText += "<button class= \"whiteButton\" id=\"LockButton\" type=\"button\" onclick = \"lock()\">Lock</button>";
        }
        else {
            infoText += "<a class= \"whiteButton\" id=\"redButton\" type=\"button\" href=\"#unlock\">Unlock</a>";
        }
    }

    document.getElementById('info').innerHTML = infoText;
}

function feed() {
    TextCommand("command/feedpasue");
}

function maintainance() {
    TextCommand("command/maintainance");
    updateInfo();
    if (iPad) {
        updatePanelContent('info');
    }
}

function thunderstorm() {
    var duration = document.getElementById('thunderstormDuration').value;
    TextCommand("command/thunderstorm/duration=" + duration);
}

function lock() {
    access.Lock();
    updateInfo();
    if (iPad) {
        updatePanelContent('info');
    }
}

function unlock() {
    access.Unlock(document.getElementById('Password').value);
    updateInfo();
    if (iPad) {
        updatePanelContent('info');
    }
}

function fillTable(sensorsXmlDoc) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Value</th><th class='last'></th></tr>";

    if (sensorsXmlDoc != null) {
        var x = sensorsXmlDoc.getElementsByTagName("WebData");

        for (i = 0; i < x.length; i++) {

            var name = x[i].getElementsByTagName("Name")[0].childNodes[0].nodeValue;
            var alarm = x[i].getElementsByTagName("IsAlarmOn")[0].childNodes[0].nodeValue;

            var even = i % 2;
            if (alarm == 'On') {
                infoText += "<tr class='alarm'>";
            } else if (even == 0) {
                infoText += "<tr class='reg'>";
            } else {
                infoText += "<tr class='alt'>";
            }
            infoText += "<td class='first'>" + x[i].getElementsByTagName("DisplayName")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td>" + x[i].getElementsByTagName("ValueString")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateMoreInfo('" + name + "','" + x[i].getElementsByTagName("Units")[0].childNodes[0].nodeValue + "')\">More</a></td>";
            infoText += "</tr>";
        }
    }
    infoText += "</table>";
    return infoText;
}

function fillLevelTable(sensorsXmlDoc) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Mode</th><th>Value</th><th class='last'></th></tr>";

    if (sensorsXmlDoc != null) {

        var x = sensorsXmlDoc.getElementsByTagName("WebData");

        for (i = 0; i < x.length; i++) {

            var name = x[i].getElementsByTagName("Name")[0].childNodes[0].nodeValue;
            var alarm = x[i].getElementsByTagName("IsAlarmOn")[0].childNodes[0].nodeValue;
            var modeValue = x[i].getElementsByTagName("ModeValue")[0].childNodes[0].nodeValue;

            var even = i % 2;
            if (alarm == 'On') {
                infoText += "<tr class='alarm'>";
            } else if (even == 0) {
                infoText += "<tr class='reg'>";
            }
            else {
                infoText += "<tr class='alt'>";
            }
            infoText += "<td class='first'>" + x[i].getElementsByTagName("DisplayName")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td>" + x[i].getElementsByTagName("Mode")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td>" + x[i].getElementsByTagName("ValueString")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateLevelMoreInfo('" + name + "','" + x[i].getElementsByTagName("Units")[0].childNodes[0].nodeValue + "','" + alarm + "','" + modeValue + "')\">More</a></td>";
            infoText += "</tr>";
        }
    }
    infoText += "</table>";
    return infoText;
}

function fillDeviceTable(sensorsXmlDoc) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Mode</th><th>Value</th><th class='last'></th></tr>";

    if (sensorsXmlDoc != null) {

        var x = sensorsXmlDoc.getElementsByTagName("WebData");

        for (i = 0; i < x.length; i++) {

            var name = x[i].getElementsByTagName("Name")[0].childNodes[0].nodeValue;
            var alarm = x[i].getElementsByTagName("IsAlarmOn")[0].childNodes[0].nodeValue;

            var even = i % 2;
            if (alarm == 'On') {
                infoText += "<tr class='alarm'>";
            } else if (even == 0) {
                infoText += "<tr class='reg'>";
            } else {
                infoText += "<tr class='alt'>";
            }
            infoText += "<td class='first'>" + x[i].getElementsByTagName("DisplayName")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td>" + x[i].getElementsByTagName("Mode")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td>" + x[i].getElementsByTagName("ValueString")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateMoreInfo('" + name + "','" + x[i].getElementsByTagName("Units")[0].childNodes[0].nodeValue + "')\">More</a></td>";
            infoText += "</tr>";
        }
    }
    infoText += "</table>";
    return infoText;
}

function fillSPortTable(sensorsXmlDoc) {
    var mode = 0;
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Mode</th><th>Value</th><th class='last'></th></tr>";
    var unlocked = access.IsUnlocked();

    if (unlocked) {
        mode = operationMode();
    }

    if (sensorsXmlDoc != null) {
        var x = sensorsXmlDoc.getElementsByTagName("WebData");

        for (i = 0; i < x.length; i++) {
            var name = x[i].getElementsByTagName("Name")[0].childNodes[0].nodeValue;
            var alarm = x[i].getElementsByTagName("IsAlarmOn")[0].childNodes[0].nodeValue;
            var even = i % 2;
            if (alarm == 'On') {
                infoText += "<tr class='alarm'>";
            } else if (even == 0) {
                infoText += "<tr class='reg'>";
            }
            else {
                infoText += "<tr class='alt'>";
            }
            infoText += "<td class='first'>" + x[i].getElementsByTagName("DisplayName")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td>" + x[i].getElementsByTagName("Mode")[0].childNodes[0].nodeValue + "</td><td>";

            var value = x[i].getElementsByTagName("ValueString")[0].childNodes[0].nodeValue;

            if (mode == "5") {
                var socketenabled = 'false';
                if (value == "On") {
                    socketenabled = 'true';
                }

                infoText += "<div class='row'><div style='position:relative;top: 0px;right: 0px;width: 100px;height: 28px;border-bottom: 0px solid #000000;'  class='toggle' onclick=\"toggleSportValue('" + name + "')\" toggled='" + socketenabled + "'><span class='thumb'></span><span class='toggleOn'>ON</span><span class='toggleOff'>OFF</span></div></div>";
            }
            else {
                infoText += value;
            }

            infoText += "</td><td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateMoreInfo('" + name + "','" + x[i].getElementsByTagName("Units")[0].childNodes[0].nodeValue + "')\">More</a></td>";
            infoText += "</tr>";
        }

    }

    infoText += "</table>";
    if (unlocked) {
        infoText += "<fieldset class='iphonefieldset'>";
        var enabled = 'false';
        if (mode == "5") {
            enabled = 'true';
        }

        infoText += "<div class='row'><Label>Manual Sockets</Label><div class='toggle' onclick='changeSocketMode()' toggled='" + enabled + "'><span class='thumb'></span><span class='toggleOn'>ON</span><span class='toggleOff'>OFF</span></div></div>";

        infoText += "</fieldset>"
    }

    return infoText;
}

function fillLightTable(sensorsXmlDoc) {
    var mode = operationMode();
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Value</th><th class='last'></th></tr>";
    var isUnlocked = access.IsUnlocked();

    if (!isUnlocked) {
        mode = 0;
    }

    if (sensorsXmlDoc != null) {
        var x = sensorsXmlDoc.getElementsByTagName("WebData");

        for (i = 0; i < x.length; i++) {
            var name = x[i].getElementsByTagName("Name")[0].childNodes[0].nodeValue;
            var alarm = x[i].getElementsByTagName("IsAlarmOn")[0].childNodes[0].nodeValue;
            var even = i % 2;
            if (alarm == 'On') {
                infoText += "<tr class='alarm'>";
            } else if (even == 0) {
                infoText += "<tr class='reg'>";
            }
            else {
                infoText += "<tr class='alt'>";
            }
            infoText += "<td class='first'>" + x[i].getElementsByTagName("DisplayName")[0].childNodes[0].nodeValue + "</td><td>";

            var valuestring = x[i].getElementsByTagName("ValueString")[0].childNodes[0].nodeValue;

            if (mode == "6") {
                var value = x[i].getElementsByTagName("Value")[0].childNodes[0].nodeValue;
                infoText += "<label for='" + name + "Value' id='" + name + "ValueString'>" + valuestring + "</label>"
                infoText += "<input id='" + name + "Value' type='range' max='100' min='0' step='1' value='" + value + "' onchange='changelightValue(\"" + name + "\", this.value)'/>";
            }
            else {

                infoText += valuestring;
            }

            infoText += "</td><td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateMoreInfo('" + name + "','" + x[i].getElementsByTagName("Units")[0].childNodes[0].nodeValue + "')\">More</a></td>";
            infoText += "</tr>";
        }

    }

    infoText += "</table>";

    if (isUnlocked) {
        infoText += "<fieldset class='iphonefieldset'>";
        var enabled = 'false';
        if (mode == "6") {
            enabled = 'true';
        }

        infoText += "<div class='row'><Label>Manual Lights</Label><div class='toggle' onclick='changeLightMode()' toggled='" + enabled + "'><span class='thumb'></span><span class='toggleOn'>ON</span><span class='toggleOff'>OFF</span></div></div>";

        infoText += "</fieldset>"
    }
    return infoText;
}

function updateReminders() {
    document.getElementById('reminders').innerHTML = fillReminderTable(new Reminders());
}

function Reminder(node) {
    this.isOverdue = node.getElementsByTagName("IsOverdue")[0].childNodes[0].nodeValue == 'true';
    this.index = node.getElementsByTagName("Index")[0].childNodes[0].nodeValue;
    this.text = node.getElementsByTagName("Text")[0].childNodes[0].nodeValue;
    this.date = node.getElementsByTagName("Date")[0].childNodes[0].nodeValue;
}

function Reminders() {
    var xmlDoc = OpenXml("command/reminders");
    if (xmlDoc != null) {
        var node = xmlDoc.getElementsByTagName("Reminder");
        this.count = node.length;

        function Get(index) {
            return new Reminder(node[index]);
        }

        this.Get = Get;
    }
}

function fillReminderTable(reminders) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Date</th><th>Text</th><th class='last'></th></tr>";

    var isUnlocked = access.IsUnlocked();

    for (i = 0; i < reminders.count; i++) {

        var reminder = reminders.Get(i);

        var even = i % 2;
        if (reminder.isOverdue) {
            infoText += "<tr class='warn'>";
        } else if (even == 0) {
            infoText += "<tr class='reg'>";
        } else {
            infoText += "<tr class='alt'>";
        }

        infoText += "<td class='first'>" + reminder.date + "</td>";
        infoText += "<td>" + reminder.text + "</td>";
        if (reminder.isOverdue && isUnlocked) {
            infoText += "<td class='last' align='right'><button class= \"smallBlueButton\" onclick = \"resetReminder('" + reminder.index + "')\">Reset</a></td>";
        } else {
            infoText += "<td class='last' align='right'></td>";
        }
        infoText += "</tr>";
    }
    infoText += "</table>";
    return infoText;
}

function changelightValue(type, value) {
    document.getElementById(type + "ValueString").innerHTML = value + "%";
    TextCommand("command/setlight/type=" + type + "&value=" + value);
}

function startWaterChange(type) {
    TextCommand("command/startwaterchange/type=" + type);
}

function resetReminder(index) {
    TextCommand("command/resetreminder/index=" + index);
    updateReminders();
    if (iPad) {
        updatePanelContent('reminders');
    }
}

function clearLevelAlarm(type) {
    TextCommand("command/clearlevelalarm/type=" + type);
    updateLevelSensors();
    if (iPad) {
        updatePanelContent('levelSensors');
    }
}

function changeLightMode() {
    var mode = operationMode();
    var enable = "True";
    if (mode == "6") {
        enable = "False";
    }

    TextCommand("command/manuallights/enable=" + enable);

    updateLights();

    if (iPad) {
        updatePanelContent('lights');
    }
}

function changeSocketMode() {
    var mode = operationMode();
    var enable = "True";
    if (mode == "5") {
        enable = "False";
    }

    TextCommand("command/manualsockets/enable=" + enable);
    updateSPorts();

    if (iPad) {
        updatePanelContent('sport');
    }
}

function toggleSportValue(type) {
    TextCommand("command/setsocket/type=" + type);
}

function updateProbes() {
    document.getElementById('probes').innerHTML = fillTable(OpenXml("command/probes"));
}

function updateUserData() {
    document.getElementById('user').innerHTML = fillTable(OpenXml("command/uservalues"));
}

function updateLevelSensors() {
    document.getElementById('levelSensors').innerHTML = fillLevelTable(OpenXml("command/levelsensors"));
}

function updateSPorts() {
    document.getElementById('sport').innerHTML = fillSPortTable(OpenXml("command/sport"));
}

function updateLPorts() {
    document.getElementById('lport').innerHTML = fillDeviceTable(OpenXml("command/lport"));
}

function updateLights() {
    document.getElementById('lights').innerHTML = fillLightTable(OpenXml("command/lights"));
}

function updateDosing() {
    document.getElementById('dosing').innerHTML = fillTable(OpenXml("command/dosing"));
}

function updateDigitalinputs() {
    document.getElementById('digitalinputs').innerHTML = fillDeviceTable(OpenXml("command/digitalinputs"));
}

function updateGraph(graph) {
    updateGraph(graph, 'all', 'graph');
}

function updateGraph(graph, range) {
    updateGraph(graph, range, 'graph');
}

function updateGraph(graph, range, item) {

    var portraitVal = "portrait";
    var landscapeVal = "landscape";
    var currentWidth = window.innerWidth;
    var currentHeight = window.innerHeight;
    var orient = (currentWidth < currentHeight) ? portraitVal : landscapeVal;

    var width = window.innerWidth;
    var offset = 120; // titlebar offset
    var hight = window.innerHeight - offset;

    if (iPad) {
        if (orient == landscapeVal) {
            hight = 768 - offset;
            width = 1024 - 320;
        }
        else {
            hight = 1024 - offset;
            width = 768 - 320;
        }
    }

    if (item == null) {
        item = "graph";
    }

    document.getElementById(item).innerHTML = "<img src=\"command/graph/type=" + graph + "&width=" + width + "&hight=" + hight + "&range=" + range + "\" />";
}

var dataPointCount = 100;

function updateDataPoints(graph, units) {
    dataPointCount = 100;
    updateMoreDataPoints(graph, units);
}

function updateMoreDataPoints(graph, units) {
    var url = "command/datapoints/type=" + graph + "&count=" + dataPointCount;
    var xmlDoc = OpenXml(url);
    if (xmlDoc != null) {
        var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Time</th><th class='last'>Value " + units + "</th></tr>";
        var x = xmlDoc.getElementsByTagName("DataPoint");
        for (i = 0; i < x.length; i++) {
            var even = i % 2;
            if (even == 0) {
                infoText += "<tr class='reg'>";
            }
            else {
                infoText += "<tr class='alt'>";
            }
            infoText += "<td class='first'>" + x[i].getElementsByTagName("TimeString")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td class='last' >" + x[i].getElementsByTagName("Value")[0].childNodes[0].nodeValue + "</td>";
            infoText += "</tr>";
        }
        infoText += "</table>";
        if (x.length == dataPointCount) {
            var buttonCommand = "updateMoreDataPoints('" + graph + "', '" + units + "')";
            infoText += "<button class= \"whiteButton\" id=\"MoreDataPoints\" type=\"button\" onclick = \"" + buttonCommand + "\">Show 100 More</button></td>";
            dataPointCount += 100;
        }
        document.getElementById('data').innerHTML = infoText;

        if (iPad) {
            updatePanelContent('data');
        }
    }
}

function updatePanelContent(item) {
    var page = document.getElementById(item);
    var contentEl = document.getElementById("iuipad-content");
    contentEl.innerHTML = page.innerHTML;
    var contentTitle = document.getElementById("contentTitle");
    if (page.title) {
        contentTitle.innerHTML = page.title;
    }
}

function updateLevelMoreInfo(graph, units, alarm, modeValue) {
    updateMoreInfo(graph, units);
    if (access.IsUnlocked()) {

        var infoText = "";
        if (alarm == 'On') {
            infoText = "<li><a href='#none' onclick='clearLevelAlarm(\"" + graph + "\")'>Clear Alarm</a></li>";
        }

        if (modeValue == '3' || modeValue == '5') {
            infoText += "<li><a href='#none' onclick='startWaterChange(\"" + graph + "\")'>Water Change</a></li>";
        }

        document.getElementById('more').innerHTML += infoText;
    }
}

function updateMoreInfo(graph, units) {
    var infoText;
    if (iPad) {

        infoText = "<li><a href=\"#graph\">Graph All</a></li>";
        infoText += "<li><a href=\"#graph_day\">Graph Day</a></li>";
        infoText += "<li><a href=\"#graph_week\">Graph Week</a></li>";
        infoText += "<li><a href=\"#graph_month\">Graph Month</a></li>";
        infoText += "<li><a href=\"#graph_year\">Graph Year</a></li>";
        infoText += "<li><a href=\"#data\">DataPoints</a></li>";

        function graphall() {
            updateGraph(graph);
        }

        function graphday() {
            updateGraph(graph, 'day', 'graph_day');
        }

        function graphweek() {
            updateGraph(graph, 'week', 'graph_week');
        }

        function graphmonth() {
            updateGraph(graph, 'month', 'graph_month');
        }

        function graphyear() {
            updateGraph(graph, 'year', 'graph_year');
        }


        function datapoints() {
            updateDataPoints(graph, units);
        }

        updateContentPane['graph'] = graphall;
        updateContentPane['graph_day'] = graphday;
        updateContentPane['graph_week'] = graphweek;
        updateContentPane['graph_month'] = graphmonth;
        updateContentPane['graph_year'] = graphyear;

        updateContentPane['data'] = datapoints;
    }
    else {
        infoText = "<li><a href=\"#graph\" onclick = \"updateGraph('" + graph + "')\">Graph All</a></li>";
        infoText += "<li><a href=\"#graph\" onclick = \"updateGraph('" + graph + "', 'day')\">Graph Day</a></li>";
        infoText += "<li><a href=\"#graph\" onclick = \"updateGraph('" + graph + "', 'week')\">Graph Week</a></li>";
        infoText += "<li><a href=\"#graph\" onclick = \"updateGraph('" + graph + "', 'month')\">Graph Month</a></li>";
        infoText += "<li><a href=\"#graph\" onclick = \"updateGraph('" + graph + "', 'year')\">Graph Year</a></li>";
        infoText += "<li><a href=\"#data\" onclick = \"updateDataPoints('" + graph + "','" + units + "')\">DataPoints</a></li>";
    }

    document.getElementById('more').innerHTML = infoText;
}