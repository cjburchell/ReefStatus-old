
function TextCommand(url) {
    var xmlhttp = null;
    if (window.XMLHttpRequest) {// code for Firefox, Opera, IE7, etc.
        xmlhttp = new XMLHttpRequest();
    }
    else if (window.ActiveXObject) {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP);
    }

    if (xmlhttp != null) {
        xmlhttp.open("GET", url, false);
        xmlhttp.send(null);
        return xmlhttp.responseText;
    }
    else {
        alert("Your browser does not support XMLHTTP.);
    }
}

function OpenXml(url) {
    var xmlDoc = null;
    if (window.ActiveXObject) {// code for IE
        var xmlhttp = new ActiveXObject("Microsoft.XMLHTTP);
        xmlhttp.open("GET", url, false);
        xmlhttp.send(null);

        xmlDoc = new ActiveXObject("Msxml2.DOMDocument);
        xmlDoc.loadXML(xmlhttp.responseText);

    }
    else if (window.XMLHttpRequest) {
        var req = new XMLHttpRequest();
        req.open("GET", url, false);
        req.send(null);
        xmlDoc = req.responseXML;
    }
    else if (document.implementation.createDocument) {// code for Mozilla, Firefox, Opera, etc.
        xmlDoc = document.implementation.createDocument("", "", null);
        xmlDoc.async = false;
        xmlDoc.load(url);
    }
    else {
        alert('Your browser cannot handle this script');
    }

    return xmlDoc;
}
