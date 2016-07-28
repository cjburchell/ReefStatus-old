
function updateInfo() {
    updateInfoText();
}

function updateInfoText() {
    var xmlDoc = OpenXml("command/info);

    var infoText = "<p><table cellpadding='3'>";
    if (xmlDoc != null) {
        var x = xmlDoc.getElementsByTagName("ControllerInfo);

        var softwareVersion = x[0].getElementsByTagName("SoftwareVersion")[0].childNodes[0].nodeValue;
        var model = x[0].getElementsByTagName("Model")[0].childNodes[0].nodeValue;
        var serialNumber = x[0].getElementsByTagName("SerialNumber")[0].childNodes[0].nodeValue;
        var alarm = x[0].getElementsByTagName("Alarm")[0].childNodes[0].nodeValue;
        var lastUpdate = x[0].getElementsByTagName("LastUpdateString")[0].childNodes[0].nodeValue;
        var operationMode = x[0].getElementsByTagName("OperationMode")[0].childNodes[0].nodeValue;
        var moonPhase = x[0].getElementsByTagName("MoonPhase")[0].childNodes[0].nodeValue;
        var latitude = x[0].getElementsByTagName("Latitude")[0].childNodes[0].nodeValue;
        var longitude = x[0].getElementsByTagName("Longitude")[0].childNodes[0].nodeValue;

        infoText += "<tr><td><b>Software Version:</b> </td><td>" + softwareVersion + "<td></tr>";
        infoText += "<tr><td><b>Model:</b></td><td>" + model + "<td></tr>";
        infoText += "<tr><td><b>Serial Number:</b></td><td>" + serialNumber + "<td></tr>";
        infoText += "<tr><td><b>Alarm:</b></td><td>" + alarm + "<td></tr>";
        infoText += "<tr><td><b>Operation Mode:</b></td><td>" + operationMode + "<td></tr>";
        infoText += "<tr><td><b>latitude:</b></td><td>" + latitude + "<td></tr>";
        infoText += "<tr><td><b>Longitude:</b></td><td>" + longitude + "<td></tr>";
        infoText += "<tr><td><b>MoonPhase:</b></td><td>" + moonPhase + "<td></tr>";
        infoText += "<tr><td><b>Last Updated:</b></td><td>" + lastUpdate + "<td></tr>";
    }
    infoText += "</table></p>";

    var unlocked = access.IsUnlocked();

    if (unlocked) {
        infoText += "<fieldset id=\"commands\"><legend></legend>";
        infoText += "<table cellpadding='3'><tr align='center'>";
        infoText += "<td><button style=\"width: 64px; height: 64px\" id=\"RefreshButton\" type=\"button\"><table><tr align='center'><td><img src=\"icons/Refresh-icon.png\"/></td></tr><tr align='center'><td>Refresh</td></tr></table></button></td>";
        infoText += "<td><button style=\"width: 64px; height: 64px\" id=\"ThunderstormButton\" type=\"button\"><table><tr align='center'><td><img src=\"icons/thunder-icon.png\"/></td></tr><tr align='center'><td>Thunderstorm</td></tr></table></button></td>";
        infoText += "</tr><tr align='center'>";
        infoText += "<td><button style=\"width: 64px; height: 64px\" id=\"FeedButton\" type=\"button\"><table><tr align='center'><td><img src=\"icons/food-icon.png\"/></td></tr><tr align='center'><td>Feed Pause</td></tr></table></button></td>";
        infoText += "<td><button style=\"width: 64px; height: 64px\" id=\"MaintainanceButton\" type=\"button\"><table><tr align='center'><td><img src=\"icons/wrench.png\"/></td></tr><tr align='center'><td>Maintainance</td></tr></table></button></td>";
        infoText += "</tr></table></fieldset></p>";
    }


    document.getElementById('info').innerHTML = infoText;

    if (unlocked) {
        var RefreshButton = new YAHOO.widget.Button("RefreshButton);
        RefreshButton.on("click", refresh);
        RefreshButton.setStyle('width', 100);

        var ThunderstormButton = new YAHOO.widget.Button("ThunderstormButton);
        ThunderstormButton.on("click", thunderstorm);
        ThunderstormButton.setStyle('width', 100);

        var FeedButton = new YAHOO.widget.Button("FeedButton);
        FeedButton.on("click", feed);
        FeedButton.setStyle('width', 100);

        var MaintainanceButton = new YAHOO.widget.Button("MaintainanceButton);
        MaintainanceButton.on("click", maintainance);
        MaintainanceButton.setStyle('width', 100);
    }

    var applyButton = new YAHOO.widget.Button("applyButton);
    applyButton.on("click", generateCustomGraph);
}

function feed() {
    TextCommand("command/feedpasue);
    updateInfoText();
}

function thunderstorm() {
    TextCommand("command/thunderstorm/duration=5);
}

function maintainance() {
    TextCommand("command/maintainance);
    updateInfoText();
}

function refresh() {
    TextCommand("command/refresh);
    updateInfoText();
}

function lock() {
    access.Lock();
    updateInfoText();
}

var customTable;

function updateCustomGraphList() {
    var xmlDoc = OpenXml("command/allvalues);
    if (xmlDoc != null) {

        var myColumnDefs = [
                        { key: "DisplayName", label: "Name", sortable: false },
                        { key: "Name", label: "NameId" }
                       ];

        myDataSource = new YAHOO.util.DataSource(xmlDoc);
        myDataSource.responseType = YAHOO.util.DataSource.TYPE_XML;
        myDataSource.responseSchema = {
            resultNode: "WebData",
            fields: ["DisplayName", "Name"]
        };

        customTable = new YAHOO.widget.DataTable("customGraphList", myColumnDefs, myDataSource, { selectionMode : "standard" });

        customTable.subscribe("rowMouseoverEvent", customTable.onEventHighlightRow);
        customTable.subscribe("rowMouseoutEvent", customTable.onEventUnhighlightRow);
        customTable.subscribe("rowClickEvent", customTable.onEventSelectRow);

        var oColumn = customTable.getColumn(1)
        customTable.hideColumn(oColumn);
    }
    
     document.getElementById('customImageWidth').value = "640";
     document.getElementById('customImageHight').value = "480";
}

function generateCustomGraph() {

    try {
        var type = "";
        var item = "";
        var selected = customTable.getSelectedRows();
        for (item in selected) {
            var oRecord = customTable.getRecord(selected[item]);
            if (type != "")
                type = type + ",";
            type = type + oRecord.getData("Name);
        }

        var startTime = document.getElementById('customStartTime').value;
        var endTime = document.getElementById('customEndTime').value;

        if (type != "" && startTime != "" && endTime != "") {

            var width = document.getElementById('customImageWidth').value;
            var hight = document.getElementById('customImageHight').value;
            if (width == "") {
                width = "640";
                document.getElementById('customImageWidth').value = width;
            }

            if (hight == "") {
                hight = "480";
                document.getElementById('customImageHight').value = hight;
            }

            document.getElementById('customGraph').innerHTML = "<img src='command/customgraph/type=" + type + "&width=" + width + "&hight=" + hight + "&start=" + startTime + "&end=" + endTime + "' />";
        }
        else {
            alert("Please select a graph to plot and a start and end time);
        }
    }
    catch (err) {
        txt = "There was an error on this page.\n\n";
        txt += "Error description: " + err.description + "\n\n";
        txt += "Click OK to continue.\n\n";
        alert(txt);
    }
}


function fillPage(type) {
    var pageLayout = "<table><tr>";
    pageLayout += "<td valign='top'><div id='" + type + "Table'></div></td>";
    pageLayout += "<td valign='top'>";
    pageLayout += "<div id='" + type + "GraphCaption'></div>";
    pageLayout += "<div id='" + type + "Graph' class='yui-navset'></div>";
    pageLayout += "</td>";
    pageLayout += "</tr></table>";
    return pageLayout;
}

var sensorTable;
var sensorGraph;

function alarmFormatter(elLiner, oRecord, oColumn, oData) {
    var alarmState = oRecord.getData("IsAlarmOn);
    if (alarmState == 'On') {
        YAHOO.util.Dom.replaceClass(elLiner.parentNode, "", "alarm);
    }
    else {
        YAHOO.util.Dom.replaceClass(elLiner.parentNode, "alarm", ");
    }

    elLiner.innerHTML = oData;
}

function updateTable(type, xmlDoc) {
    sensorGraph = type + 'Graph';
    document.getElementById(type).innerHTML = fillPage(type);
    if (xmlDoc != null) {

    // Add the custom formatter to the shortcuts 
       YAHOO.widget.DataTable.Formatter.myCustom = alarmFormatter;

        var myColumnDefs = [
                        { key: "DisplayName", label: "Name", sortable: true, formatter: "myCustom" },
                        { key: "ValueString", label: "Value", formatter: "myCustom" },
                        { key: "Name", label: "NameId" }
                       ];

        myDataSource = new YAHOO.util.DataSource(xmlDoc);
        myDataSource.responseType = YAHOO.util.DataSource.TYPE_XML;
        myDataSource.responseSchema = {
            resultNode: "WebData",
            fields: ["DisplayName", "ValueString", "Name", "IsAlarmOn"]
        };

        sensorTable = new YAHOO.widget.DataTable(type + "Table", myColumnDefs, myDataSource, { selectionMode: "single" });

        sensorTable.subscribe("rowClickEvent", updateSensorGraph);
        sensorTable.subscribe("rowMouseoverEvent", sensorTable.onEventHighlightRow);
        sensorTable.subscribe("rowMouseoutEvent", sensorTable.onEventUnhighlightRow);

        var oColumn = sensorTable.getColumn(2)
        sensorTable.hideColumn(oColumn);
        sensorTable.selectRow(sensorTable.getTrEl(0));
        updateGraph(sensorGraph, sensorTable.getRecord(sensorTable.getTrEl(0)).getData("Name"), true);
    }
}

function updateProbes() {
    updateTable("probe", OpenXml("command/probes"));
}

function updateLevelSensors() {
    updateDevicesTable("level", OpenXml("command/levelsensors"));
}

function updateSensorGraph(oArgs) {
    var elTarget = oArgs.target;
    var oRecord = sensorTable.getRecord(elTarget);
    sensorTable.unselectAllRows();
    sensorTable.selectRow(oRecord);
    updateGraph(sensorGraph, oRecord.getData("Name"), true);
}


var deviceTable;
var deviceGraph;

function updateDevicesTable(type, xmlDoc) {
    deviceGraph = type + 'Graph';
    document.getElementById(type).innerHTML = fillPage(type);
    if (xmlDoc != null) {
        YAHOO.widget.DataTable.Formatter.myCustom = alarmFormatter;


        var myColumnDefs = [
                        { key: "DisplayName", label: "Name", sortable: true, formatter: "myCustom" },
                        { key: "Mode", label: "Mode", sortable: true, formatter: "myCustom" },
                        { key: "ValueString", label: "Value", formatter: "myCustom" },
                        { key: "Name", label: "NameId" }
                       ];


        myDataSource = new YAHOO.util.DataSource(xmlDoc);
        myDataSource.responseType = YAHOO.util.DataSource.TYPE_XML;
        myDataSource.responseSchema = {
            resultNode: "WebData",
            fields: [
            { key: "DisplayName" },
            { key: "Mode" },
            { key: "ValueString" },
            { key: "Name" }, 
            { key: "IsAlarmOn" }
            ]
        };

        deviceTable = new YAHOO.widget.DataTable(type + "Table", myColumnDefs, myDataSource, { selectionMode: "single" });

        var oColumn = deviceTable.getColumn(3)
        deviceTable.hideColumn(oColumn);

        // Subscribe to events for row selection
        deviceTable.subscribe("rowClickEvent", updateDeviceGraph);
        deviceTable.subscribe("rowMouseoverEvent", deviceTable.onEventHighlightRow);
        deviceTable.subscribe("rowMouseoutEvent", deviceTable.onEventUnhighlightRow);
        
        deviceTable.selectRow(deviceTable.getTrEl(0));
        updateGraph(deviceGraph, deviceTable.getRecord(deviceTable.getTrEl(0)).getData("Name"), true);
    }
}

function updateSPorts() {
    updateDevicesTable("sport", OpenXml("command/sport"));
}

function updateLPorts() {
    updateDevicesTable("lport", OpenXml("command/lport"));
}

function updateUserValues() {
    updateTable("user", OpenXml("command/uservalues"));
}

function updateDosing() {
    updateTable("dosing", OpenXml("command/dosing"));
}

function updateLights() {
    updateTable("lights", OpenXml("command/lights"));
}

function updateDigitalInputs() {
    updateDevicesTable("digitalInputs", OpenXml("command/digitalinputs"));
}


function updateDeviceGraph(oArgs) {
    var elTarget = oArgs.target;
    var oRecord = deviceTable.getRecord(elTarget);
    deviceTable.unselectAllRows();
    deviceTable.selectRow(oRecord);
    updateGraph(deviceGraph, oRecord.getData("Name"), true);
}

function updateGraph(graphType, type, showDay) {
    document.getElementById(graphType + 'Caption').innerHTML = "<center><h3>" + type + "</h3></center>";

    document.getElementById(graphType).innerHTML = "";
    var graphTabs = new YAHOO.widget.TabView(graphType);

    var weekTab = new YAHOO.widget.Tab({
        label: 'Week',
        content: "<div id = 'graphImage" + type + "week'></div>",
        active: true
    });
    graphTabs.addTab(weekTab);
    weekTab.addListener("click", function () { updateGraphImage(type, "week); });
    updateGraphImage(type, "week);

    if (showDay) {
        var dayTab = new YAHOO.widget.Tab({
            label: 'Day',
            content: "<div id = 'graphImage" + type + "day'></div>"
        });
        graphTabs.addTab(dayTab);
        dayTab.addListener("click", function() { updateGraphImage(type, "day); });
    }

    var monthTab = new YAHOO.widget.Tab({
        label: 'Month',
        content: "<div id = 'graphImage" + type + "month'></div>"
    });
    graphTabs.addTab(monthTab);
    monthTab.addListener("click", function() { updateGraphImage(type, "month); });


    var yearTab = new YAHOO.widget.Tab({
        label: 'Year',
        content: "<div id = 'graphImage" + type + "year'></div>"
    });
    graphTabs.addTab(yearTab);
    yearTab.addListener("click", function() { updateGraphImage(type, "year); });

    var allTab = new YAHOO.widget.Tab({
        label: 'All',
        content: "<div id = 'graphImage" + type + "'></div>"
    });
    graphTabs.addTab(allTab);
    allTab.addListener("click", function() { updateGraphImage(type, "); });

    var dataTab = new YAHOO.widget.Tab({
        label: 'Data',
        content: "<div id = 'dataPointTable" + type + "'></div>"
    });

    graphTabs.addTab(dataTab);
    dataTab.addListener("click", function() { dataPointTable = updateDataPoints(type); dataPointTable.onShow() });
}

var graphToUpdate;

function updateGraphCallback(result) {

    var items = result.Data;
    var width = 640;
    var hight = 480;


    var minValue = 1000.0;
    var maxValue = 0.0;
    var data = new Array();
    for (var i in items) {
        var item = new Array();
        var time = eval(items[i].Time.replace(/\/Date\((.*?)\)\//gi, "new Date($1)"));
        var offset = time.getTimezoneOffset() * 60 * 1000;
        var utc = time.getTime();
        item[0] = utc - offset;
        item[1] = items[i].Value;
        minValue = Math.min(items[i].Value, minValue);
        maxValue = Math.max(items[i].Value, maxValue);
        data[i] = item;
    }

    document.getElementById(graphToUpdate).innerHTML = '<div id="' + graphToUpdate + 'placeholder" style="width:' + width + 'px;height:' + hight + 'px;"></div>';

    var options = {
        lines: { show: true, fill: true, fillColor: { colors: ["rgba(" + result.Colour + ", 0.1)", "rgba(" + result.Colour + ", 0.8)"]} },
        xaxis: { mode: "time" },
        grid: { backgroundColor: { colors: ["#fff", "#eee"]} },
        zoom: { interactive: true },
        pan: { interactive: true }
    };

    var plot = $.plot($("#" + graphToUpdate + "placeholder"), [{ data: data, color: "rgba(" + result.Colour + ", 0.8)"}], options);
}

function Request(command, param, callback) {
    var fullcommand;
    if (param != "") {
        fullcommand = command + "/output=json&callback=" + callback + "&" + param;
    }
    else {
        fullcommand = command + "/output=json&callback=" + callback;
    }
    var reqest = new JSONscriptRequest(fullcommand);
    reqest.buildScriptTag();
    reqest.addScriptTag();
}

function updateGraphImage(type, range) {
    if (range == "") {
        range = 'all';
        graphToUpdate = 'graphImage' + type;
    }
    else {
        graphToUpdate = 'graphImage' + type + range;
    }

    Request("command/datapoints", "type=" + type + "&range=" + range + "&graphdata=true", "updateGraphCallback);
}

var dataPointTable;

function updateDataPoints(type) {
    var url = "command/datapoints/type=" + type;

    var xmlDoc = OpenXml(url);
    if (xmlDoc != null) {

        var myColumnDefs = [
                        { key: "TimeString", label: "Time", sortable: true },
                        { key: "Value", label: "Value", sortable: true }
                       ];

        myDataSource = new YAHOO.util.DataSource(xmlDoc);
        myDataSource.responseType = YAHOO.util.DataSource.TYPE_XML;
        myDataSource.responseSchema = {
            resultNode: "DataPoint",
            fields: [
            { key: "TimeString" },
            { key: "Value" }
            ]
        };

        var myConfigs = {
            paginator: new YAHOO.widget.Paginator({
                rowsPerPage: 25
            })
        };

        return new YAHOO.widget.DataTable("dataPointTable" + type, myColumnDefs, myDataSource, myConfigs);
    }
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

    var html = "<table>";
    html += "<tr><td><label>Server:</label></td><td><input id='ServerAddress' type='url' onchange='serverAddressChanged(this.value)' value='" + localStorage.ServerAddress + "' /></td></tr>";
    html += "<tr><td><label>Port:</label></td><td><input id='ServerPort' type='number' onchange='serverPortChanged(this.value)' value='" + localStorage.ServerPort + "'/></td></tr>";
    html += "<tr><td><label>Password:</label></td><td><input id='ServerPassword' type='password' onchange='passwordChanged(this.value)' value='" + localStorage.Password + "'/></td></tr>";
    html += "</table>";

    document.getElementById('settings').innerHTML = html;
}

function serverAddressChanged(value) {
    localStorage.ServerAddress = value;
}

function serverPortChanged(value) {
    localStorage.ServerPort = value;
}

function passwordChanged(value) {
    access.Unlock(value);
}
