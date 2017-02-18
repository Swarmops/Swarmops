// ------------------- _masterSocket functions -----------------------

var _masterSocket;
var _masterSocketHeartbeatsLost;


function _masterInitializeSocket(authenticationTicket) {
    if (_masterSocket != null) {
        _masterSocket.close();
        _masterSocket = null;
    }

    var socketUrl = getMasterSocketAddress() + "?Auth=" + authenticationTicket;

    _masterSocket = new WebSocket(socketUrl);
    _masterSocket.onopen = function(data) {
        if (_masterSocketHeartbeatsLost) {
            // Reload all edits and the edit pool. If there are discrepancies, we can probably live with them. (Detect editing?)
            //alertify.log("Backend connection restored without interruption.");

            _masterSocketHeartbeatsLost = false;
        }

        //watchHeartbeat();
    };
    _masterSocket.onclose = function(data) { /* TODO: try reconnecting */ };
    _masterSocket.onerror = function(data) { alertify.error("WARNING: Socket connection error - realtime updates will not be available"); };
    _masterSocket.onmessage = function(data) {
        
        console.log(data.data);

        var message = $.parseJSON(data.data);

        if (message.messageType == "Heartbeat") {
            alertify.log("Master socket heartbeat: " + message.Timestamp);
        } 
        else if (message.messageType == "SandboxUpdate") {
            if (odoLocalParticipation != undefined) {  // Real ugly accessing specific page elements here, but it's temporary
                odoLocalParticipation.innerHTML = message.local;
                odoGlobalParticipation.innerHTML = 12345678 + message.local * 5;
                odoActiveParticipation.innerHTML = 123412 + message.local * 5;
                odoProfitLossToDate.innerHTML = (message.profit / 100.0) + 0.001;
            }
        }
    };
}

function getMasterSocketAddress() {
    if (location.host.indexOf ("localhost") >= 0) { // Assume dev environment, go for sandbox socket
        return 'ws://sandbox.swarmops.com/ws/Front';
    } else {
        var protocol = ('https:' == document.location.protocol ? 'wss://' : 'ws://');
        return protocol + location.host + "/ws/Front";
    }
}

// ------------------- SwarmopsJS object ----------------------

var SwarmopsJS = (function () {
    var publicSymbols = {},
        initFunctions = [];


    publicSymbols.unescape = unescape;
    function unescape(data) {
        return decodeURIComponent(data);
    }



    publicSymbols.formatInteger = formatInteger;
    function formatInteger (number, callbackSuccess, callbackError) 
    {
        if (callbackSuccess === undefined) {
            alert("FormatInteger() called without callbackSuccess parameter. This is not allowed.");
            return ("[Undefined]");
        }

        var jsonData = {};
        jsonData.input = number;

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: '/Automation/Formatting.aspx/FormatInteger',
            dataType: 'json',
            data: $.toJSON(jsonData),

            success: function(data) {
                callbackSuccess(data.d);
            },

            error: function(){
                if (callbackError !== undefined) {
                    callbackError();
                } else {
                    alertify.error("AJAX error calling SwarmopsJS.FormatInteger. Are the server and network still available?");  // TODO: Loc - how?
                    callbackSuccess("[Undefined]");
                }
            }
        });

    }

    publicSymbols.formatCurrency = formatCurrency;
    function formatCurrency(number, callbackSuccess, callbackError) {
        if (callbackSuccess === undefined) {
            alert("FormatCurrency() called without callbackSuccess parameter. This is not allowed.");
            return ("[Undefined]");
        }

        var jsonData = {};
        jsonData.input = number;

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: '/Automation/Formatting.aspx/FormatCurrency',
            dataType: 'json',
            data: $.toJSON(jsonData),

            success: function (data) {
                callbackSuccess(data.d);
            },

            error: function () {
                if (callbackError !== undefined) {
                    callbackError();
                } else {
                    alertify.error("AJAX error calling SwarmopsJS.FormatCurrency. Are the server and network still available?");  // TODO: Loc - how?
                    callbackSuccess("[Undefined]");
                }
            }
        });

    }


    publicSymbols.ajaxCall = ajaxCall;
    function ajaxCall(url, params, successFunction, errorFunction) {
        $.ajax({
            type: "POST",
            url: url,
            data: $.toJSON(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                successFunction(msg.d);
            },
            error: function () {
                if (errorFunction !== undefined) {
                    errorFunction();
                } else {
                    alertify.error("There was an unspecified error calling the Swarmops server. Is the server reachable?"); // TODO: Localize
                }
            }
        });
    }

    // Below copied from http://stackoverflow.com/questions/4578424/javascript-extend-a-function, assumed to be in public domain

    publicSymbols.init = init;
    function init() {
        var funcs = initFunctions;

        initFunctions = undefined;

        for (index = 0; index < funcs.length; ++index) {
            try { funcs[index](); } catch (e) { }
        }
    }

    publicSymbols.addInitFunction = addInitFunction;

    function addInitFunction(f) {
        if (initFunctions) {
            // Init hasn't run yet, rememeber it
            initFunctions.push(f);
        } else {
            // `init` has already run, call it almost immediately
            // but *asynchronously* (so the caller never sees the
            // call synchronously)
            setTimeout(f, 0);
        }
    }

    return publicSymbols;
})();



/* Overrides for EasyUI */

/* Nav in ComboTree */

// below code from http://doc.javake.cn/jeasyui/www.jeasyui.com/forum/index.php-topic=1972.0.htm, understood to be in public domain: enables keyboard navigation

if (typeof $.fn.combotree != 'undefined') {

    (function() {
        $.extend($.fn.combotree.methods, {
            nav: function(jq, dir) {
                return jq.each(function() {
                    var opts = $(this).combotree('options');
                    var t = $(this).combotree('tree');
                    var nodes = t.tree('getChildren');
                    if (!nodes.length) {
                        return;
                    }
                    var node = t.tree('getSelected');
                    if (!node) {
                        t.tree('select', dir > 0 ? nodes[0].target : nodes[nodes.length - 1].target);
                    } else {
                        var index = 0;
                        for (var i = 0; i < nodes.length; i++) {
                            if (nodes[i].target == node.target) {
                                index = i;
                                break;
                            }
                        }
                        if (dir > 0) {
                            while (index < nodes.length - 1) {
                                index++;
                                if ($(nodes[index].target).is(':visible')) {
                                    break;
                                }
                            }
                        } else {
                            while (index > 0) {
                                index--;
                                if ($(nodes[index].target).is(':visible')) {
                                    break;
                                }
                            }
                        }
                        t.tree('select', nodes[index].target);
                    }
                    if (opts.selectOnNavigation) {
                        var node = t.tree('getSelected');
                        $(node.target).trigger('click');
                        $(this).combotree('showPanel');
                    }
                });
            }
        });
        $.extend($.fn.combotree.defaults.keyHandler, {
            up: function() {
                $(this).combotree('nav', -1);
            },
            down: function() {
                $(this).combotree('nav', 1);
            },
            enter: function() {
                var t = $(this).combotree('tree');
                var node = t.tree('getSelected');
                if (node) {
                    $(node.target).trigger('click');
                }
                $(this).combotree('hidePanel');
            }
        });
    })(jQuery);

}
