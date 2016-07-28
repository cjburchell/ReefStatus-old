function updateInfo() {
    var xmlDoc = OpenXml("command/info);

    var infoText = "<p><table>";
    if (xmlDoc != null) {
        var x = xmlDoc.getElementsByTagName("ControllerInfo);
        var softwareVersion = x[0].getElementsByTagName("SoftwareVersion")[0].childNodes[0].nodeValue;
        var model = x[0].getElementsByTagName("Model")[0].childNodes[0].nodeValue;
        var serialNumber = x[0].getElementsByTagName("SerialNumber")[0].childNodes[0].nodeValue;
        var alarm = x[0].getElementsByTagName("Alarm")[0].childNodes[0].nodeValue;
        var lastUpdate = x[0].getElementsByTagName("LastUpdateString")[0].childNodes[0].nodeValue;
        var mode = x[0].getElementsByTagName("OperationMode")[0].childNodes[0].nodeValue;
        var moonPhase = x[0].getElementsByTagName("MoonPhase")[0].childNodes[0].nodeValue;
        var latitude = x[0].getElementsByTagName("Latitude")[0].childNodes[0].nodeValue;
        var longitude = x[0].getElementsByTagName("Longitude")[0].childNodes[0].nodeValue;

        infoText += "<tr><td><b>Software Version:</b></td><td>" + softwareVersion + "<td></tr>";
        infoText += "<tr><td><b>Model:</b></td><td>" + model + "<td></tr>";
        infoText += "<tr><td><b>Serial Number:</b></td><td>" + serialNumber + "<td></tr>";
        infoText += "<tr><td><b>Alarm:</b></td><td>" + alarm + "<td></tr>";
        infoText += "<tr><td><b>Mode:</b></td><td>" + mode + "<td></tr>";
        infoText += "<tr><td><b>Latitude:</b></td><td>" + latitude + "<td></tr>";
        infoText += "<tr><td><b>Longitude:</b></td><td>" + longitude + "<td></tr>";
        infoText += "<tr><td><b>Moon Phase:</b></td><td>" + moonPhase + "<td></tr>";
        infoText += "<tr><td><b>Last Updated:</b></td><td>" + lastUpdate + "<td></tr>";

    }
    infoText += "</table></p>";

    var lockEnabled = access.LockEnabled;
    var unlocked = access.IsUnlocked();

    if (unlocked) {
        infoText += "<table cellpadding='3'><tr align='center'><td>";
        infoText += "<button type='button' onclick='refreshCommand()'><table><tr align='center'><td><img src=\"icons/Refresh-icon.png\"/></td></tr><tr align='center'><td>Refresh</td></tr></table></button>";
        infoText += "</td><td>";
        infoText += "<button id=\"ThunderstormButton\" type='button' onclick=\"window.location.assign('thunderstorm.html')\"><table><tr align='center'><td><img src=\"icons/thunder-icon.png\"/></td></tr><tr align='center'><td>Thunderstorm</td></tr></table></button>";
        infoText += "</td></tr><tr align='center'><td>";
        infoText += "<button id=\"FeedButton\" type='button' onclick='feed()'><table><tr align='center'><td><img src=\"icons/food-icon.png\"/></td></tr><tr align='center'><td>Feed Pause</td></tr></table></button>";
        infoText += "</td><td>";
        infoText += "<button id=\"MaintainanceButton\" type='button' onclick='maintainance()'><table><tr align='center'><td><img src=\"icons/wrench.png\"/></td></tr><tr align='center'><td>Maintainance</td></tr></table></button>";
        infoText += "</td></tr></table>";
    }

    if (lockEnabled) {
        if (unlocked) {
            infoText += "<input id=\"LockButton\" type=\"button\" value=\"Lock\" onclick = \"lock()\"/>";
        }
        else {
            infoText += "<input id=\"LockButton\" type=\"button\" value=\"Unlock\" onclick = \"window.location.assign('login.html')\"/>";
        }
    }

    document.getElementById('info').innerHTML = infoText;
}

function feed() {
    TextCommand("command/feedpasue);
}

function maintainance() {
    TextCommand("command/maintainance);
    updateInfo();
}

function thunderstorm() {
    var duration = document.getElementById('thunderstormDuration').value;
    TextCommand("command/thunderstorm/duration=" + duration);
    window.location.assign("info.html);
}

function lock() {
    access.Lock();
    updateInfo(); 
}

function unlock() {
    access.Unlock(document.getElementById('Password').value);
    window.location.assign("info.html);
}

function updateList(type) {

    if (type == "Probes") {
        fillTable(type, OpenXml("command/probes"));
    }
    else if (type == "Level_Sensors") {
        fillDeviceTable(type, OpenXml("command/levelsensors"));
    }
    else if (type == "S_Ports") {
        fillDeviceTable(type, OpenXml("command/sport"));
    }
    else if (type == "L_Ports") {
        fillDeviceTable(type, OpenXml("command/lport"));
    }
    else if (type == "Dosing") {
        fillTable(type, OpenXml("command/dosing"));
    }
    else if (type == "DigitalInputs") {
        fillDeviceTable(type, OpenXml("command/digitalinputs"));
    }
    else if (type == "Lights") {
        fillTable(type, OpenXml("command/lights"));
    }
    else if (type == "User_Data") {
        fillTable(type, OpenXml("command/uservalues"));
    }
}

function getName(type) {

    if (type == "Probes") {
        return type;
    }
    else if (type == "Level_Sensors") {
    return "Level Sensors";
    }
    else if (type == "S_Ports") {
    return "Sockets";
    }
    else if (type == "L_Ports") {
        return "1-10V Ports";
    }
    else if (type == "User_Data") {
    return "User Data";
    }
else if (type == "Dosing") {
    return "Dosing";
    }
else if (type == "Lights") {
    return "Lights";
    }
else if (type == "DigitalInputs") {
    return "Digital Inputs";
    }
}

var sensorTable;

function fillTable(type, xmlDoc) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th  class='first'>Name</th><th class='last'>Value</th></tr>";

    if (xmlDoc != null) {
        var x = xmlDoc.getElementsByTagName("WebData);
        for (i = 0; i < x.length; i++) {
            var name = x[i].getElementsByTagName("Name")[0].childNodes[0].nodeValue;
            var units = "";
            if (x[i].getElementsByTagName("Units").length != 0)
                units = x[i].getElementsByTagName("Units")[0].childNodes[0].nodeValue;

            var displayName = "";
            if (x[i].getElementsByTagName("DisplayName").length != 0)
                displayName = x[i].getElementsByTagName("DisplayName")[0].childNodes[0].nodeValue;

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
            infoText += "<td class = 'first'><a href='menu.html?type=" + type + "&name=" + name + "&units=" + units + "&displayname=" + displayName + "'>" + displayName + "</a></td>";
            infoText += "<td>" + x[i].getElementsByTagName("ValueString")[0].childNodes[0].nodeValue + "</td>";
            infoText += "</tr>";
        }
    }

    infoText += "</table>";
    document.getElementById("list").innerHTML = infoText;
}

function fillDeviceTable(type, xmlDoc) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Mode</th><th class='last'>Value</th></tr>";

    if (xmlDoc != null) {
        var x = xmlDoc.getElementsByTagName("WebData);
        for (i = 0; i < x.length; i++) {
            var name = x[i].getElementsByTagName("Name")[0].childNodes[0].nodeValue;
            var units = "";
            if (x[i].getElementsByTagName("Units").length != 0)
                units = x[i].getElementsByTagName("Units")[0].childNodes[0].nodeValue;

            var displayName = "";
            if (x[i].getElementsByTagName("DisplayName").length != 0)
                displayName = x[i].getElementsByTagName("DisplayName")[0].childNodes[0].nodeValue;

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
            infoText += "<td class = 'first'><a href='menu.html?type=" + type + "&name=" + name + "&units=" + units + "&displayname=" + displayName + "'>" + displayName + "</a></td>";
            infoText += "<td>" + x[i].getElementsByTagName("Mode")[0].childNodes[0].nodeValue + "</td>";
            infoText += "<td>" + x[i].getElementsByTagName("ValueString")[0].childNodes[0].nodeValue + "</td>";
            infoText += "</tr>";
        }
    }

    infoText += "</table>";

    
    document.getElementById("list").innerHTML = infoText;
}

function updateMoreInfo(type, graph, units, displayName) {
    var width = screen.width;
    var hight = screen.height;
    if (width > 640) {
        width = 640;
        hight = 480;
    }
    
    var infoText = "<ul>";
    infoText += "<li><a href='command/graph/type=" + graph + "&width=" + width + "&hight=" + hight + "&range=all'>Graph All</a></li>";
    infoText += "<li><a href='command/graph/type=" + graph + "&width=" + width + "&hight=" + hight + "&range=day'>Graph Day</a></li>";
    infoText += "<li><a href='command/graph/type=" + graph + "&width=" + width + "&hight=" + hight + "&range=week'>Graph Week</a></li>";
    infoText += "<li><a href='command/graph/type=" + graph + "&width=" + width + "&hight=" + hight + "&range=month'>Graph Month</a></li>";
    infoText += "<li><a href='command/graph/type=" + graph + "&width=" + width + "&hight=" + hight + "&range=year'>Graph Year</a></li>";
    infoText += "<li><a href='datapoints.html?type=" + type + "&graph=" + graph + "&units=" + units + "&displayName=" + displayName + "'>DataPoints</a></li>";
    infoText += "</ul>";
    document.getElementById('more').innerHTML = infoText;
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
        var x = xmlDoc.getElementsByTagName("DataPoint);
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
            infoText += "<input type='button' value='Show 100 More' onclick = '" + buttonCommand + "'/></td>";
            dataPointCount += 100;
        }
        document.getElementById('data').innerHTML = infoText;
    }
}

function updateInput(value) {
    return value.replace("%20", " ").replace("%C2%B0", "°);
}

