﻿var app;
var silverlightHostID = 'silverlightControlHost';
var silverlightControlID = 'silverlightControl';

function WebSocketEx(uri, protocol, onopen, onclose, onmessage, onerror) {

    this.uri = uri;
    this.protocol = protocol;
    this.onmessage = onmessage;
    this.onopen = onopen;
    this.onclose = onclose;
    this.onerror = onerror;
    this.websocket = null;
    this.send = function (msg) {
        websocket.send(msg);
    }

    function createBridgeApp() {
        return Silverlight.createObject("ClientBin/WebSocket4Net.JsBridge.xap",
                document.getElementById(silverlightHostID),
                silverlightControlID,
                {
                    width: "0",
                    height: "0",
                    background: "white",
                    version: "4.0.60310.0",
                    autoUpgrade: true
                },
                {
                    onError: onSilverlightError,
                    onLoad: onSilverlightLoaded
                });
            }

    function createBirdgeWebSocket() {
        var slPlugin = document.getElementById('silverlightControl');
        if (slPlugin) {
            websocket = slPlugin.content.services.createObject("WebSocket");
            if (websocket) {
                websocket.onopen = function (s, e) {
                    if (onopen != null)
                        onopen();
                };
                websocket.onclose = function (s, e) {
                    if (onclose != null) {
                        onclose();
                    }
                };
                websocket.onmessage = function (s, e) {
                    if (onmessage != null) {
                        onmessage(e);
                    }
                };
                websocket.onerror = function (s, e) {
                    if (onerror != null) {
                        onerror(e);
                    }
                };

                return websocket;
            }
        }
    }

    var support = "MozWebSocket" in window ? 'MozWebSocket' : ("WebSocket" in window ? 'WebSocket' : null);

    if (support != null) {
        //using native websocket
        var ws;

        if (protocol && protocol.length > 0)
            ws = new window[support](uri, protocol);
        else
            ws = new window[support](uri);

        ws.onmessage = this.onmessage;

        ws.onopen = this.onopen;

        ws.onclose = this.onclose;

        ws.onerror = this.onerror;

        websocket = ws;

        return;
    }

    if (Silverlight && Silverlight.isInstalled()) {
        if (app == null || app == undefined) {
            window.onSilverlightLoaded = function (sender, args) {
                websocket = createBirdgeWebSocket();
                websocket.open(uri, protocol);
            }
            app = createBridgeApp();
            return;
        }

        this.websocket = createBirdgeWebSocket();
        this.websocket.open(uri, protocol);
        return;
    }

    alert("Your browser cannot support WebSocket!");
}



function onSilverlightError(sender, args) {
    var appSource = "";
    if (sender != null && sender != 0) {
        appSource = sender.getHost().Source;
    }

    var errorType = args.ErrorType;
    var iErrorCode = args.ErrorCode;

    if (errorType == "ImageError" || errorType == "MediaError") {
        return;
    }

    var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

    errMsg += "Code: " + iErrorCode + "    \n";
    errMsg += "Category: " + errorType + "       \n";
    errMsg += "Message: " + args.ErrorMessage + "     \n";

    if (errorType == "ParserError") {
        errMsg += "File: " + args.xamlFile + "     \n";
        errMsg += "Line: " + args.lineNumber + "     \n";
        errMsg += "Position: " + args.charPosition + "     \n";
    }
    else if (errorType == "RuntimeError") {
        if (args.lineNumber != 0) {
            errMsg += "Line: " + args.lineNumber + "     \n";
            errMsg += "Position: " + args.charPosition + "     \n";
        }
        errMsg += "MethodName: " + args.methodName + "     \n";
    }

    throw new Error(errMsg);
}