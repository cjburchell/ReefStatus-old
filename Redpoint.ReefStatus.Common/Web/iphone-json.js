var iPad = false;
var cashedInfo = undefined;

function updateHomeMenuCallback(info) {

    cashedInfo = info;
    var infoText;

    if (iPad) {
        infoText = '<li><a href="#info" onclick = "updateInfo()"><img src="icons/info-icon.png" /> Information</a></li>';


        if (info.ProbeCount != 0) {
            if (info.ProbeAlarm) {
                infoText += '<li class="alarmItem"><a href="#probes" onclick = "stopRefresh()"><img src="icons/thermometer.png" /> Probes</a></li>';
            }
            else {
                infoText += '<li><a href="#probes" onclick = "stopRefresh()"><img src="icons/thermometer.png" /> Probes</a></li>';
            }
        }
        if (info.LevelSensorCount != 0) {
            if (info.LevelAlarm) {
                infoText += '<li class="alarmItem"><a href="#levelSensors" onclick = "stopRefresh()"><img src="icons/WaterBottle.png" /> Level Sensors</a></li>';
            }
            else {
                infoText += '<li><a href="#levelSensors" onclick = "stopRefresh()"><img src="icons/WaterBottle.png" /> Level Sensors</a></li>';
            }
        }
        if (info.DigitalInputCount != 0) {
            infoText += '<li><a href="#digitalinputs" onclick = "stopRefresh()"><img src="icons/green_button-32.png" /> Digtital Inputs</a></li>';
        }
        if (info.SPortCount != 0) {
            infoText += '<li><a href="#sport" onclick = "stopRefresh()"><img src="icons/powerplug.png" /> Sockets</a></li>';
        }
        if (info.LPortCount != 0) {
            infoText += '<li><a href="#lport" onclick = "stopRefresh()"><img src="icons/diagram-icon.png" /> 1-10V Ports</a></li>';
        }
        if (info.LightCount != 0) {
            infoText += '<li><a href="#lights" onclick = "stopRefresh()"><img src="icons/light.png" /> Lights</a></li>';
        }
        if (info.DosingPumpCount != 0) {
            infoText += '<li><a href="#dosing" onclick = "stopRefresh()"><img src="icons/science.png" /> Dosing</a></li>';
        }
        if (info.UserValueCount != 0) {
            infoText += '<li><a href="#user" onclick = "stopRefresh()"><img src="icons/ph.png" /> Other</a></li>';
        }
        if (info.ReminderCount != 0) {
            if (info.ReminderOverdue) {
                infoText += '<li class="warnItem"><a href="#reminders" onclick = "stopRefresh()"><img src="icons/bell.png" /> Reminders</a></li>';
            }
            else {
                infoText += '<li><a href="#reminders" onclick = "stopRefresh()"><img src="icons/bell.png" /> Reminders</a></li>';
            }
        }

        if (access.IsUnlocked()) {
            infoText += '<li><a href="#control"><img src="icons/monitor-icon.png" /> Control</a></li>';
            infoText += '<li><a href="#commands" onclick = "stopRefresh()"><img src="icons/key.png" /> Commands</a></li>';
        }

        infoText += '<li><a href="#links"><img src="icons/globe-icon.png" /> Links</a></li>';
        infoText += '<li><a href="#settings" onclick = "stopRefresh()"><img src="icons/gear-icon.png" /> Settings</a></li>';

    }
    else {
        infoText = '<li><a href="#info" onclick = "updateInfo()"><img src="icons/info-icon.png" /> Information</a></li>';

        if (info.ProbeCount != 0) {
            if (info.ProbeAlarm) {
                infoText += '<li class="alarmItem"><a href="#probes" onclick = "updateProbes()"><img src="icons/thermometer.png" /> Probes</a></li>';
            } else {
                infoText += '<li><a href="#probes" onclick = "updateProbes()"><img src="icons/thermometer.png" /> Probes</a></li>';
            }
        }
        if (info.LevelSensorCount != 0) {
            if (info.LevelAlarm) {
                infoText += '<li class="alarmItem"><a href="#levelSensors" onclick = "updateLevelSensors()"><img src="icons/WaterBottle.png" /> Level Sensors</a></li>';
            }
            else {
                infoText += '<li><a href="#levelSensors" onclick = "updateLevelSensors()"><img src="icons/WaterBottle.png" /> Level Sensors</a></li>';
            }
        }
        if (info.DigitalInputCount != 0) {
            infoText += '<li><a href="#digitalinputs" onclick = "updateDigitalinputs()"><img src="icons/green_button-32.png" /> Digtital Inputs</a></li>';
        }
        if (info.SPortCount != 0) {
            infoText += '<li><a href="#sport" onclick = "updateSPorts()"><img src="icons/powerplug.png" /> Sockets</a></li>';
        }
        if (info.LPortCount != 0) {
            infoText += '<li><a href="#lport" onclick = "updateLPorts()"><img src="icons/diagram-icon.png" /> 1-10V Ports</a></li>';
        }
        if (info.LightCount != 0) {
            infoText += '<li><a href="#lights" onclick = "updateLights()"><img src="icons/light.png" /> Lights</a></li>';
        }
        if (info.DosingPumpCount != 0) {
            infoText += '<li><a href="#dosing" onclick = "updateDosing()"><img src="icons/science.png" /> Dosing</a></li>';
        }
        if (info.UserValueCount != 0) {
            infoText += '<li><a href="#user" onclick = "updateUserData()"><img src="icons/ph.png" /> Other</a></li>';
        }
        if (info.ReminderCount != 0) {
            if (info.ReminderOverdue) {
                infoText += '<li class="warnItem" ><a href="#reminders" onclick = "updateReminders()"><img src="icons/bell.png" /> Reminders</a></li>';
            }
            else {
                infoText += '<li><a href="#reminders" onclick = "updateReminders()"><img src="icons/bell.png" /> Reminders</a></li>';
            }
        }

        if (access.IsUnlocked()) {
            infoText += '<li><a href="#control" onclick = "startRefreshText()"><img src="icons/monitor-icon.png" /> Control</a></li>';
            infoText += '<li><a href="#commands"><img src="icons/key.png" /> Commands</a></li>';
        }

        infoText += '<li><a href="#links"><img src="icons/globe-icon.png" /> Links</a></li>';
        infoText += '<li><a href="#settings" onclick = "updateSettings()"><img src="icons/gear-icon.png" /> Settings</a></li>';
    }

    document.getElementById('home').innerHTML = infoText;
}

function updateHomeMenu() {
    Request("command/info", "", "updateHomeMenuCallback);
}

function updateSettings() {
    if (localStorage.ServerAddress == undefined) {
        localStorage.ServerAddress = "";
    }

    if (localStorage.ServerPort == undefined) {
        localStorage.ServerPort = 8081;
    }

    if (localStorage.Password == undefined) {
        localStorage.Password = '';
    }

   var html = "<fieldset class='iphonefieldset'>";
   html += "<div class='row'><label>Server:</label><input id='ServerAddress' type='url' onchange='serverAddressChanged(this.value)' value='" + localStorage.ServerAddress + "' /></div>";
   html += "<div class='row'><label>Port:</label><input id='ServerPort' type='number' onchange='serverPortChanged(this.value)' value='" + localStorage.ServerPort + "'/></div>";
   html += "</fieldset>";
   html += "<fieldset class='iphonefieldset'>";
   html += "<div class='row'><label>Password:</label><input id='ServerPassword' type='password' onchange='passwordChanged(this.value)' value='" + localStorage.Password + "'/></div>";
   html += "</fieldset>";

   document.getElementById('settings').innerHTML = html;

    if (iPad) {
        updatePanelContent('settings');
    }
}

function serverAddressChanged(value) {
    localStorage.ServerAddress = value;
    updateHomeMenu();
}

function serverPortChanged(value) {
    localStorage.ServerPort = value;
    updateHomeMenu();
}

function passwordChanged(value) {
    access.Unlock(value);
    updateHomeMenu();
}

function TextCommandCallback(result) {
    console.log("reply = " + result);
}

function getUrl() {
    if (localStorage.ServerAddress != '' && localStorage.ServerAddress != undefined ) {
        if (localStorage.ServerPort == '80' || localStorage.ServerPort == undefined) {
            return "http://" + localStorage.ServerAddress + "/";
        }
        else {
            return "http://" + localStorage.ServerAddress + ":" + localStorage.ServerPort + "/";
        }
    }

    return "";
}

function Request(command, param, callback) {
    var fullcommand = getUrl() + command + "/output=json&callback=" + callback;

    if (localStorage.ControllerAddress != '' && localStorage.ControllerAddress != undefined) {
        fullcommand += "&Controller=" + localStorage.ControllerAddress;
    }

    if (param != "") {
        fullcommand += "&" + param;
    }

    var reqest = new JSONscriptRequest(fullcommand);
    reqest.buildScriptTag();
    reqest.addScriptTag();
}

function TextCommand(command) {
    Request(command,"","TextCommandCallback);
}


function TextCommand(command, param) {
    Request(command , param, "TextCommandCallback);
}

function updateInfoDataCallback(info) {
    cashedInfo = info;
}

function updateInfoData() {
    Request("command/info", "", "updateInfoDataCallback);
}

function updateInfoCallback(info) {
    cashedInfo = info;
    var infoText = "<fieldset class='iphonefieldset'>";
    if (info != null) {
        infoText += "<div class=\"row\">";
        infoText += "<label>Version:</label> <input type=\"text\" value=\"" + info.SoftwareVersion + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Model:</label> <input type=\"text\" value=\"" + info.Model + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>S/N:</label><input type=\"text\" value=\"" + info.SerialNumber + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Alarm:</label><input type=\"text\" value=\"" + info.Alarm + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Mode:</label><input type=\"text\" value=\"" + info.OperationMode + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Latitude:</label><input type=\"text\" value=\"" + info.Latitude + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Longitude:</label><input type=\"text\" value=\"" + info.Longitude + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Moon Phase:</label><input type=\"text\" value=\"" + info.MoonPhase + "\" readonly = \"true\"/>";
        infoText += "</div>";
        infoText += "<div class=\"row\">";
        infoText += "<label>Updated:</label><input type=\"text\" value=\"" + info.LastUpdateString + "\" readonly = \"true\"/>";
        infoText += "</div>";
    }

    infoText += "</fieldset>";

    document.getElementById('info').innerHTML = infoText;

    if (iPad) {
        updatePanelContent('info');
    }
}

function updateInfo() {
    access.Update();
    Request("command/info", "", "updateInfoCallback);
}

function operationMode() {
    if (cashedInfo != undefined)
    {
        return cashedInfo.ModeId;
    }

    return 0;
}

function setOperationMode(mode) {
    if (cashedInfo != undefined) {
        cashedInfo.ModeId = mode;
    }
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
    TextCommand("command/thunderstorm", "duration=" + duration);
    window.iui.DialogCallbackOnSubmit = undefined;
}

function fillTable(items) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Value</th><th class='last'></th></tr>";

    for (i = 0; i < items.length; i++) {

        var item = items[i];

        var even = i % 2;
        if (item.IsAlarmOn != 0) {
            infoText += "<tr class='alarm'>";
        } else if (even == 0) {
            infoText += "<tr class='reg'>";
        } else {
            infoText += "<tr class='alt'>";
        }
        infoText += "<td class='first'>" + item.DisplayName + "</td>";
        infoText += "<td>" + item.ValueString + "</td>";
        infoText += "<td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateMoreInfo('" + item.Name + "','" + item.Units + "','"+item.DisplayName+"')\">More</a></td>";
        infoText += "</tr>";
    }
    infoText += "</table>";
    return infoText;
}

function fillLevelTable(items) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Mode</th><th>Value</th><th class='last'></th></tr>";

    for (i = 0; i < items.length; i++) {

        var item = items[i];

        var even = i % 2;
        if (item.IsAlarmOn != 0) {
            infoText += "<tr class='alarm'>";
        } else if (even == 0) {
            infoText += "<tr class='reg'>";
        } else {
            infoText += "<tr class='alt'>";
        }
        infoText += "<td class='first'>" + item.DisplayName + "</td>";
        infoText += "<td>" + item.Mode + "</td>";
        infoText += "<td>" + item.ValueString + "</td>";
        infoText += "<td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateLevelMoreInfo('" + item.Name + "','" + item.Units + "','" + item.IsAlarmOn + "','" + item.ModeValue + "','" + item.DisplayName + "')\">More</a></td>";
        infoText += "</tr>";
    }

    infoText += "</table>";
    return infoText;
}

function fillDeviceTable(items) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Mode</th><th>Value</th><th class='last'></th></tr>";

    for (i = 0; i < items.length; i++) {

        var item = items[i];

        var even = i % 2;
        if (item.IsAlarmOn != 0) {
            infoText += "<tr class='alarm'>";
        } else if (even == 0) {
            infoText += "<tr class='reg'>";
        } else {
            infoText += "<tr class='alt'>";
        }
        infoText += "<td class='first'>" + item.DisplayName + "</td>";
        infoText += "<td>" + item.Mode + "</td>";
        infoText += "<td>" + item.ValueString + "</td>";
        infoText += "<td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateMoreInfo('" + item.Name + "','" + item.Units + "','" + item.DisplayName + "')\">More</a></td>";
        infoText += "</tr>";
    }
    
    infoText += "</table>";
    return infoText;
}

function fillSPortTable(items) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Mode</th><th>Value</th><th class='last'></th></tr>";


    var mode = 0;
    var unlocked = access.IsUnlocked();

    if (unlocked) {
           mode = operationMode();
    }

    for (i = 0; i < items.length; i++) {
        var item = items[i];
        var name = item.Name;
        var alarm = item.IsAlarmOn != 0;
        var even = i % 2;
        if (alarm) {
            infoText += "<tr class='alarm'>";
        } else if (even == 0) {
            infoText += "<tr class='reg'>";
        }
        else {
            infoText += "<tr class='alt'>";
        }
        infoText += "<td class='first'>" + item.DisplayName + "</td>";
        infoText += "<td>" + item.Mode + "</td><td>";

        if (mode == 5) {
            var socketenabled = 'false';
            if (item.ValueString == "On") {
                socketenabled = 'true';
            }

            infoText += "<div class='row'><div style='position:relative;top: 0px;right: 0px;width: 100px;height: 28px;border-bottom: 0px solid #000000;'  class='toggle' onclick=\"toggleSportValue('" + name + "')\" toggled='" + socketenabled + "'><span class='thumb'></span><span class='toggleOn'>ON</span><span class='toggleOff'>OFF</span></div></div>";
        }
        else {
            infoText += item.ValueString;
        }

        infoText += "</td><td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateMoreInfo('" + name + "','" + item.Units + "','" + item.DisplayName + "')\">More</a></td>";
        infoText += "</tr>";
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

function fillLightTable(items) {

    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Value</th><th class='last'></th></tr>";


    var mode = 0;
    var isUnlocked = access.IsUnlocked();

    if (isUnlocked) {
        mode = operationMode();
    }

    for (i = 0; i < items.length; i++) {
        var item = items[i];
        var name = item.Name;
        var even = i % 2;
        if (item.IsAlarmOn != 0) {
            infoText += "<tr class='alarm'>";
        } else if (even == 0) {
            infoText += "<tr class='reg'>";
        }
        else {
            infoText += "<tr class='alt'>";
        }
        infoText += "<td class='first'>" + item.DisplayName + "</td><td>";

        var valuestring = item.ValueString;

        if (mode == 6) {
            var value = item.Value;
            infoText += "<label for='" + name + "Value' id='" + name + "ValueString'>" + valuestring + "</label>"
            infoText += "<input id='" + name + "Value' type='range' max='100' min='0' step='1' value='" + value + "' onchange='changelightValue(\"" + name + "\", this.value)'/>";
        }
        else {

            infoText += valuestring;
        }

        infoText += "</td><td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateMoreInfo('" + name + "','" + item.Units + "','" + item.DisplayName + "')\">More</a></td>";
        infoText += "</tr>";
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

function fillReminderTable(reminders) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Date</th><th>Text</th><th class='last'></th></tr>";

    var isUnlocked = access.IsUnlocked();

    for (i = 0; i < reminders.length; i++) {

        var reminder = reminders[i];

        var even = i % 2;
        if (reminder.IsOverdue) {
            infoText += "<tr class='warn'>";
        } else if (even == 0) {
            infoText += "<tr class='reg'>";
        } else {
            infoText += "<tr class='alt'>";
        }

        infoText += "<td class='first'>" + reminder.Date + "</td>";
        infoText += "<td>" + reminder.Text + "</td>";
        if (reminder.IsOverdue && isUnlocked) {
            infoText += "<td class='last' align='right'><button class= \"smallBlueButton\" onclick = \"resetReminder('" + reminder.Index + "')\">Reset</a></td>";
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
    TextCommand("command/setlight","type=" + type + "&value=" + value);
}

function startWaterChange(type) {
    TextCommand("command/startwaterchange", "type=" + type);
}

function resetReminder(index) {
    TextCommand("command/resetreminder", "index=" + index);
    updateReminders();
    updateHomeMenu();
}

function clearLevelAlarm(type) {
    TextCommand("command/clearlevelalarm", "type=" + type);
    updateLevelSensors();
    updateHomeMenu();
}

function changeLightMode() {
    var mode = operationMode();
    var enable = "True";
    if (mode == 6) {
        enable = "False";
        setOperationMode(0);
    }
    else {
        setOperationMode(6);
    }



    TextCommand("command/manuallights", "enable=" + enable);
    updateLights(); 
}

function changeSocketMode() {
    var mode = operationMode();
    var enable = "True";
    if (mode == 5) {
        enable = "False"; 
        setOperationMode(0);
    }
    else {
        setOperationMode(5);
    }

    TextCommand("command/manualsockets", "enable=" + enable);
    updateSPorts();
}

function toggleSportValue(type) {
    TextCommand("command/setsocket", "type=" + type);
}

function updateReminders() {
    access.Update();
    Request("command/reminders", "", "updateRemindersCallback);
}

function updateRemindersCallback(reminders) {
    document.getElementById('reminders').innerHTML = fillReminderTable(reminders);
    if (iPad) {
        updatePanelContent('reminders');
    }
}

function updateProbes() {
    Request("command/probes", "", "updateProbesCallback);
}

function updateProbesCallback(probes) {
    document.getElementById('probes').innerHTML = fillTable(probes);
    if (iPad) {
        updatePanelContent('probes');
    }
}

function fillUserTable(items) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Value</th><th class='last'></th></tr>";

    for (i = 0; i < items.length; i++) {

        var item = items[i];

        var even = i % 2;
        if (item.IsAlarmOn != 0) {
            infoText += "<tr class='alarm'>";
        } else if (even == 0) {
            infoText += "<tr class='reg'>";
        } else {
            infoText += "<tr class='alt'>";
        }
        infoText += "<td class='first'>" + item.DisplayName + "</td>";
        infoText += "<td>" + item.ValueString + "</td>";
        infoText += "<td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateUserMoreInfo('" + item.Name + "','" + item.Units + "','" + item.DisplayName + "','" + item.Value + "')\">More</a></td>";
        infoText += "</tr>";
    }
    infoText += "</table>";
    return infoText;
}

function updateUserData() {
    Request("command/uservalues", "", "updateUserDataCallback);
}

function updateUserDataCallback(items) {
    document.getElementById('user').innerHTML = fillUserTable(items);

    if (iPad) {
        updatePanelContent('user');
    }
}


function updateLevelSensors() {
    Request("command/levelsensors", "", "updateLevelSensorsCallback);
}

function updateLevelSensorsCallback(items) {
    document.getElementById('levelSensors').innerHTML = fillLevelTable(items);

    if (iPad) {
        updatePanelContent('levelSensors');
    }
}

function updateSPorts() {
    access.Update();
    updateInfoData();
    Request("command/sport", "", "updateSPortsCallback);
}

function updateSPortsCallback(items) {
    document.getElementById('sport').innerHTML = fillSPortTable(items);
    if (iPad) {
        updatePanelContent('sport');
    }
}

function updateLPorts() {
    Request("command/lport", "", "updateLPortsCallback);
}

function updateLPortsCallback(items) {
    document.getElementById('lport').innerHTML = fillDeviceTable(items);

    if (iPad) {
        updatePanelContent('lport');
    }
}

function updateLights() {
    access.Update();
    updateInfoData();
    Request("command/lights", "", "updateLightsCallback);
}

function updateLightsCallback(items) {
    document.getElementById('lights').innerHTML = fillLightTable(items);
    if (iPad) {
        updatePanelContent('lights');
    }
}

function fillDosingTable(items) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Name</th><th>Value</th><th class='last'></th></tr>";

    for (i = 0; i < items.length; i++) {

        var item = items[i];

        var even = i % 2;
        if (item.IsAlarmOn != 0) {
            infoText += "<tr class='alarm'>";
        } else if (even == 0) {
            infoText += "<tr class='reg'>";
        } else {
            infoText += "<tr class='alt'>";
        }
        infoText += "<td class='first'>" + item.DisplayName + "</td>";
        infoText += "<td>" + item.ValueString + "</td>";
        infoText += "<td class='last' align='right'><a class= \"smallBlueButton\" href=\"#more\" onclick = \"updateDosingMoreInfo('" + item.Name + "','" + item.Units + "','" + item.DisplayName + "','" + item.Rate + "','" + item.PerDay + "')\">More</a></td>";
        infoText += "</tr>";
    }
    infoText += "</table>";
    return infoText;
}

function updateDosing() {
    Request("command/dosing", "", "updateDosingCallback);
}


function updateDosingCallback(items) {
    document.getElementById('dosing').innerHTML = fillDosingTable(items);

    if (iPad) {
        updatePanelContent('dosing');
    }
}

function updateDigitalinputs() {
    Request("command/digitalinputs", "", "updateDigitalinputsCallback);
}

function updateDigitalinputsCallback(items) {
    document.getElementById('digitalinputs').innerHTML = fillDeviceTable(items);

    if (iPad) {
        updatePanelContent('digitalinputs');
    }
}

var graphToUpdate;

function updateGraphCallback(result) {

    var items = result.Data;
    var portraitVal = "portrait";
    var landscapeVal = "landscape";
    var currentWidth = window.innerWidth;
    var currentHeight = window.innerHeight;
    var orient = (currentWidth < currentHeight) ? portraitVal : landscapeVal;

    var width = window.innerWidth - 20;
    var offset = 120; // titlebar offset
    var hight = window.innerHeight - offset;

    if (iPad) {
        if (orient == landscapeVal) {
            hight = 768 - offset;
            width = 1024 - 340;
        }
        else {
            hight = (1024 - offset) - 100;
            width = 768 - 340;
        }
    }

    var minValue = 1000.0;
    var maxValue = 0.0;
    var data = new Array();
    for (var i in items) {
        item = new Array();
        var time = eval(items[i].Time.replace(/\/Date\((.*?)\)\//gi, "new Date($1)"));
        var offset = time.getTimezoneOffset() * 60 * 1000;
        var utc = time.getTime();
        item[0] = utc - offset;
        item[1] = items[i].Value;
        minValue = Math.min(items[i].Value, minValue);
        maxValue = Math.max(items[i].Value, maxValue);
        data[i] = item;
    }


    if (iPad) {
        updatePanelContent(graphToUpdate);
        document.getElementById("iuipad-content").innerHTML = '<div id="placeholder" style="width:' + width + 'px;height:' + hight + 'px;"></div>';
    }
    else {
        document.getElementById(graphToUpdate).innerHTML = '<div id="placeholder" style="width:' + width + 'px;height:' + hight + 'px;"></div>';
    }

    var options = {
        lines: { show: true, fill: true, fillColor: { colors: ["rgba(" + result.Colour + ", 0.1)", "rgba(" + result.Colour + ", 0.8)"]} },
        xaxis: { mode: "time" },
        grid: { backgroundColor: { colors: ["#fff", "#eee"]} },
        zoom: { interactive: true },
        pan: { interactive: true }
    };

    var plot = $.plot($("#placeholder"), [{ data: data, color: "rgba(" + result.Colour + ", 0.8)"}], options);
}

function updateGraph(graph, range, item, displayname) {

    if (item == null) {
        item = "graph";
    }

    graphToUpdate = item;
    document.getElementById(item).title = displayname;
    Request("command/datapoints", "type=" + graph + "&range=" + range + "&graphdata=true", "updateGraphCallback);

    //document.getElementById(item).innerHTML = "<img src=\"" + getUrl() +"command/graph/type=" + graph + "&width=" + width + "&hight=" + hight + "&range=" + range + "\" />";
}

var dataPointCount = 100;

function updateDataPoints(graph, units) {
    dataPointCount = 100;
    updateMoreDataPoints(graph, units);
}

function updateMoreDataPointsCallback(points) {
    var infoText = "<table class='itable' width='100%' border='0' cellspacing='0' cellpadding='3'><tr class='header'><th class='first'>Time</th><th class='last'>Value " + datapointsCurrentUnits + "</th></tr>";
        for (i = 0; i < points.length; i++) {
            var even = i % 2;
            if (even == 0) {
                infoText += "<tr class='reg'>";
            }
            else {
                infoText += "<tr class='alt'>";
            }
            infoText += "<td class='first'>" + points[i].TimeString + "</td>";
            infoText += "<td class='last' >" + points[i].Value + "</td>";
            infoText += "</tr>";
        }
        infoText += "</table>";
        if (points.length == dataPointCount) {
            var buttonCommand = "updateMoreDataPoints('" + datapointsCurrentGraph + "', '" + datapointsCurrentUnits + "')";
            infoText += "<button class= \"whiteButton\" id=\"MoreDataPoints\" type=\"button\" onclick = \"" + buttonCommand + "\">Show 100 More</button></td>";
            dataPointCount += 100;
        }
        document.getElementById('data').innerHTML = infoText;

        if (iPad) {
            updatePanelContent('data');
        }
}

var datapointsCurrentGraph = "";
var datapointsCurrentUnits = "";

function updateMoreDataPoints(graph, units) {
    datapointsCurrentGraph = graph;
    datapointsCurrentUnits = units;

    Request("command/datapoints", "type=" + graph + "&count=" + dataPointCount, "updateMoreDataPointsCallback);
}

function updatePanelContent(item) {
    var page = document.getElementById(item);
    var contentEl = document.getElementById("iuipad-content);
    contentEl.innerHTML = page.innerHTML;
    var contentTitle = document.getElementById("contentTitle);
    if (page.title) {
        contentTitle.innerHTML = page.title;
    }
}

function updateLevelMoreInfo(graph, units, alarm, modeValue, displayName) {
    updateMoreInfo(graph, units, displayName);
    if (access.IsUnlocked()) {

        var infoText = "";
        if (alarm != 0) {
            infoText = "<li><a href='#none' onclick='clearLevelAlarm(\"" + graph + "\")'>Clear Alarm</a></li>";
        }

        if (modeValue == '3' || modeValue == '5') {
            infoText += "<li><a href='#none' onclick='startWaterChange(\"" + graph + "\")'>Water Change</a></li>";
        }

        document.getElementById('more').innerHTML += infoText;
    }
}

function updateUserMoreInfo(graph, units, displayName, value) {
    updateMoreInfo(graph, units, displayName);
    if (access.IsUnlocked()) {

        if (iPad) {
            infoText += "<li><a href=\"#uservalueupdate\">Update Value</a></li>";

            function updateUserValueCallback() {
                userValueUpdate(graph, value, units);
            }

            updateContentPane['uservalueupdate'] = updateUserValueCallback;
        }
        else {
            infoText = "<li><a href='#uservalueupdate' onclick='userValueUpdate(\"" + graph + "\", \"" + value + "\",\"" + units + "\")'>Update Value</a></li>";
        }

        document.getElementById('more').innerHTML += infoText;
    }
}

function userValueUpdate(graph, value, units) {
    var html = "<fieldset class='iphonefieldset'>";
    html += "<div class='row'><label>Value (" + units + "):</label><input id='UserValue' type='number' onchange='updateUserValue(this.value,\"" + graph + "\" )' value='" + value + "' /></div>";
    html += "</fieldset>";;

    document.getElementById('uservalueupdate').innerHTML = html;

    if (iPad) {
        updatePanelContent('uservalueupdate');
    }
}

function updateUserValue(value, graph ) {
    TextCommand("command/updateuservalue", "type=" + graph + "&value=" + value);
}

function updateDosingMoreInfo(graph, units, displayName, rate, perDay) {
    updateMoreInfo(graph, units, displayName);
    if (access.IsUnlocked()) {

        var infoText = "";
        if (iPad) {
            infoText += "<li><a href=\"#updatedousingrate\">Update Rate</a></li>";

            function updateDosingCallback() {
                dosingRateUpdate(graph, rate, perDay, units);
            }

            updateContentPane['updatedousingrate'] = updateDosingCallback;
        }
        else {
            infoText = "<li><a href='#updatedousingrate' onclick='dosingUpdate(\"" + graph + "\", \"" + rate + "\",\"" + perDay + "\")'>Update Rate</a></li>";
        }

        document.getElementById('more').innerHTML += infoText;
    }
}

function dosingUpdate(graph, rate, perday) {
    var html = "<fieldset class='iphonefieldset'>";
    html += "<div class='row'><label>Rate:</label><input id='DousingRate' type='number' onchange='updateDosingValue(this.value,\"" + perday + "\" ,\"" + graph + "\" )' value='" + rate + "' /></div>";
    html += "<div class='row'><label>Per Day:</label><input id='DosingPerDay' type='number' onchange='updateDosingValue(\"" + rate + "\" ,this.value,\"" + graph + "\" )' value='" + perday + "' /></div>";
    html += "</fieldset>"; ;

    document.getElementById('updatedousingrate').innerHTML = html;

    if (iPad) {
        updatePanelContent('updatedousingrate');
    }
}

function updateDosingValue(rate, perday, graph) {
    TextCommand("command/updatedousingvalue", "type=" + graph + "&rate=" + rate + "&perday=" + perday);
    dosingUpdate(graph, rate, perday);
}

function updateMoreInfo(graph, units, displayName) {
    var infoText;
    if (iPad) {

        infoText = "<li><a href=\"#graph\">Graph All</a></li>";
        infoText += "<li><a href=\"#graph_day\">Graph Day</a></li>";
        infoText += "<li><a href=\"#graph_week\">Graph Week</a></li>";
        infoText += "<li><a href=\"#graph_month\">Graph Month</a></li>";
        infoText += "<li><a href=\"#graph_year\">Graph Year</a></li>";
        infoText += "<li><a href=\"#data\">DataPoints</a></li>";

        function graphall() {
            updateGraph(graph, 'all', 'graph', displayName);
        }

        function graphday() {
            updateGraph(graph, 'day', 'graph_day', displayName);
        }

        function graphweek() {
            updateGraph(graph, 'week', 'graph_week', displayName);
        }

        function graphmonth() {
            updateGraph(graph, 'month', 'graph_month', displayName);
        }

        function graphyear() {
            updateGraph(graph, 'year', 'graph_year', displayName);
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
        infoText = "<li><a href=\"#graph\" onclick = \"updateGraph('" + graph +  "', 'all','graph','" + displayName + "')\">Graph All</a></li>";
        infoText += "<li><a href=\"#graph\" onclick = \"updateGraph('" + graph + "', 'day', 'graph','" + displayName + "')\">Graph Day</a></li>";
        infoText += "<li><a href=\"#graph\" onclick = \"updateGraph('" + graph + "', 'week', 'graph','" + displayName + "')\">Graph Week</a></li>";
        infoText += "<li><a href=\"#graph\" onclick = \"updateGraph('" + graph + "', 'month', 'graph','" + displayName + "')\">Graph Month</a></li>";
        infoText += "<li><a href=\"#graph\" onclick = \"updateGraph('" + graph + "', 'year', 'graph','" + displayName + "')\">Graph Year</a></li>";
        infoText += "<li><a href=\"#data\" onclick = \"updateDataPoints('" + graph + "','" + units + "')\">DataPoints</a></li>";
    }

    document.getElementById('more').innerHTML = infoText;
}