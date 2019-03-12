// ------------------- _masterSocket functions -----------------------

var _masterSocket;
var _masterSocketHeartbeatsLost;
var _masterWatchingHeartbeat = false;
var _masterSocketLastHeartbeat = -1;
var _masterAuthenticationTicket = "";
var _masterNavigatingAway = false; // set in document.ready in master

function _masterInitializeSocket(authenticationTicket) {

    // Our first call should always be with an auth ticket; save this for later use

    if (authenticationTicket === undefined) {
        authenticationTicket = _masterAuthenticationTicket;
    } else {
        _masterAuthenticationTicket = authenticationTicket;
    }

    // If we're being called to restore communications in case of a heartbeat failure,
    // close the channel first or the server will think we're still here

    if (_masterSocket != null) {
        _masterSocket.close();
        _masterSocket = null;
    }

    var socketUrl = getMasterSocketAddress() + "?Auth=" + authenticationTicket;

    // Create instance and open socket

    _masterSocket = new WebSocket(socketUrl);

    // Set events

    _masterSocket.onopen = function(data) {
        console.log("MasterSocket.OnOpen();");

        if (_error_ClientSocketLost) {
            _error_ClientSocketLost = false;
            _master_updateMalfunctions();
        }

        // If this is the first .open, start the heartbeat watcher

        if (!_masterWatchingHeartbeat) {
            _master_watchHeartbeat();
        }
    };

    _masterSocket.onclose = function(data) {

        console.log("MasterSocket.OnClose();");
        if (!_masterNavigatingAway) // set in master page on window.beforeunload: socket will close when loading other page
        {
            _error_ClientSocketLost = true;
            _master_updateMalfunctions();

            if (!_masterWatchingHeartbeat) {
                _master_watchHeartbeat(); // begin watch heartbeat if open never happened
            }
        }
    };

    _masterSocket.onerror = function (data) {

        console.log("MasterSocket.OnError();");
        if (!_error_ClientSocketLost) {
            alertify.error("WARNING: Socket connection error - realtime updates will not be available");
            _error_ClientSocketLost = true;
            _master_updateMalfunctions();
        }
    };

    _masterSocket.onmessage = function (data) {
        
        var message = $.parseJSON(data.data);

        if (message.MessageType == "Heartbeat") {
            _masterSocketLastHeartbeat = new Date().getTime();
            if (_masterSocketHeartbeatsLost) {
                console.log(" - receiving heartbeats again");
                _masterSocketHeartbeatsLost = false;
                _master_updateMalfunctions();
            }

            if (typeof pageOnHeartbeat === "function") { // if it's defined on the page, indicating there's a handler for it
                pageOnHeartbeat('Frontend', _masterSocketLastHeartbeat);
            }

        }
        else if (message.MessageType == "BackendHeartbeat") {
            if (typeof pageOnHeartbeat === "function") { // if it's defined on the page, indicating there's a handler for it
                pageOnHeartbeat('Backend', new Date().getTime());
            }
        }
        else if (message.MessageType == "BitcoinReceived") {
            // console.log("Currency is " + message.Currency);
            // console.log(message);
            var handled = false;

            if (typeof pageOnBitcoinReceived === "function") { // if it's defined on the page indicating the page handles it
                handled = pageOnBitcoinReceived(message.Address, message.Hash, message.Satoshis, message.Cents, message.Currency);
            }

            if (!handled) {
                // Otherwise make a log note at the bottom right of the screen
                alertify.log("Bitcoin received: " + message.Currency + " " + message.CentsFormatted);
            }
        }
        else if (message.MessageType == "Malfunctions") {
            _master_updateMalfunctions (message.MalfunctionsList);
        }
        else if (message.MessageType == "AnnualProfitLossCents") {
            if (odoProfitLossToDate != undefined) // if there's a P&L odometer on the current page
            {
                if (message.Instant == "1") {
                    window.odometerOptions.duration = 1;
                }
                odoProfitLossToDate.innerHTML = (message.ProfitLossCents / 100.0) + 0.001; // update it; +0.001 needed for %.2f
                window.odometerOptions.duration = 1500;
            }
        }
        else if (message.MessageType == "SandboxUpdate") {
            if (window["odoLocalParticipation"] != undefined) { // Real ugly accessing specific page elements here, but it's temporary
                odoLocalParticipation.innerHTML = message.Local;
                odoGlobalParticipation.innerHTML = 12345678 + message.Local * 5;
                odoActiveParticipation.innerHTML = 123412 + message.Local * 5;
            }
            if (window["odoProfitLossToDate"] != undefined) {
            odoProfitLossToDate.innerHTML = (message.Profit / 100.0) + 0.001;
            }
        }
        else if (message.MessageType == "ProgressUpdate") {
            var updateFunction = window["progressUpdateCallback_" + message.Guid];

            if (updateFunction != undefined) {
                updateFunction(message.Progress);
            }
        }
    };
}

function _master_watchHeartbeat() {
 
    _masterWatchingHeartbeat = true;

    // Check again for a new heartbeat in ten seconds

    setTimeout(function () {
        _master_watchHeartbeat();
    }, 10000);

    // Have we lost the socket connection? If so, take this cadence opportunity to try reconnecting

    if (_error_ClientSocketLost || _masterSocketHeartbeatsLost) {
        console.log(" - trying to reinit socket");
        _masterInitializeSocket();
    }

    // If we got a heartbeat more than fifteen seconds ago, we've lost the heartbeat

    if (_masterSocketLastHeartbeat != -1 && !_masterSocketHeartbeatsLost) { // if we've received any at all
        var curTime = new Date().getTime();
        var diff = (curTime - _masterSocketLastHeartbeat) / 1000;
        if (diff > 15) {
            console.log(" - Heartbeats LOST");
            _masterSocketHeartbeatsLost = true;
            _master_updateMalfunctions();
        }
    }
}




function getMasterSocketAddress() {
    if (location.host.indexOf ("localhost") >= 0) { // Assume dev environment, go for sandbox socket
        return 'ws://sandbox.swarmops.com/ws/Front';
    } else {
        var protocol = ('https:' == document.location.protocol ? 'wss://' : 'ws://');
        return protocol + location.host + "/ws/Front";
    }
}


function _master_updateMalfunctions(issueList) {

    if (issueList === undefined) {
        issueList = JSON.parse(JSON.stringify(_master_LastServerMalfunctionsList)); // deep copy
    } else {
        _master_LastServerMalfunctionsList = JSON.parse(JSON.stringify(issueList)); // deep copy
    }

    if (issueList === undefined) {  // edge case present on early init
        issueList = [];
        _master_LastServerMalfunctionsList = [];
    }

    if (_error_ClientSocketLost) {
        issueList.push(_master_constructMalfunctionData("ClientSocket", _errorDisplay_clientSocketLost));  // defined in master
    }
    else if (_masterSocketHeartbeatsLost) {
        issueList.push(_master_constructMalfunctionData("ClientHeartbeat", _errorDisplay_clientHeartbeatLost));  // defined in master
    }

    _master_updateListBox($('#divMalfunctionsList'), issueList);
}

function _master_constructMalfunctionData(id, text, icon, link) {
    var newItem = {};
    newItem.Id = id;
    newItem.Text = text;
    newItem.Icon = icon;
    newItem.Link = link;

    return newItem;
}

var _master_LastServerMalfunctionsList = [];
var _error_ClientSocketLost = false;


// ------------------- List updating --------------------------


function _master_updateListBox(box, listArray) {
    // list is an array of object { id, text }
    // box is a div-div-ul-li nest containing the li:s with the id
    // the function syncs the box to the list with some nice fades

    console.log("UpdateListBox();");

    var listContainer = $(box).parent().parent();
    var listElements = $(box).children();

    // If the list is empty but the box wasn't, add an "all clear" message
    // to the list of items and set timer to remove it

    // TODO: Make the "all clear" message dependent on which box we're dealing with

    if (listElements.length > 0 && listArray.length == 0) {
        listContainer.attr("allClear", "true");
        listArray.push(_master_constructMalfunctionData("AllClear", eval(listContainer.attr("allClearDisplay")), "/Images/Icons/iconshock-green-tick-128px.png"));
        setTimeout(function() { _master_removeAllClearMessage(listContainer, listElements); }, 10000);
    } else {
        listContainer.attr("allClear", "false");
    }

    // Is the box visible right now?

    var boxVisible = $(box).is(":visible");

    // Step 1: Iterate through list, build id array

    console.log(" - step 1");

    console.log(listArray);
    var idListLookup = {};
    if (listArray.length > 0) {
        listArray.forEach(function(item, index) {
            idListLookup[item.Id] = item;
        });
    }

    // Step 2: Iterate through box, change texts of matching ids,
    //         remove items that aren't in list, build id array

    console.log(" - step 2");

    var idBoxLookup = {};

    if (listElements.length > 0) {
        listElements.children().each(function (index) {
            console.log($(this));
            var elementId = $(this).attr("rel");
            console.log(elementId);
            idBoxLookup[elementId] = true;
            var testLookup = idListLookup[elementId];
            console.log(testLookup);

            if (testLookup === undefined) {
                // displayed item is NOT present in supplied list, and should be removed
                $(this).slideUp(400, function () { $(this).remove(); });
            } else {
                // displayed item is present in supplied list
                // TODO: update text
            }
        });
    }

    // Step 3: Iterate through list, add missing items

    console.log(" - step 3");

    if (listElements.length == 0)
    {
        // If there's not even an UL element (yet), add one and move the pointer to it,
        // so we can add any missing items

        $(box).append ($("<ul></ul>"));
        listElements = $(box).children();
    }


    listArray.forEach(function(item, index) {
        if (idBoxLookup[item.Id] != true) {
            var html = "<li rel='" + item.Id + "'";

            if (item.Icon !== undefined) {
                html += " style='background-image:url(" + item.Icon + ")'";
            }

            html += "> " + item.Text + "</li>";
            var newItem = $(html).hide();
            $(listElements).append(newItem);
            newItem.slideDown();
        }
    });


    // Step 4: Adjust visibility as required

    console.log(" - fixing visibility");

    var listEmpty = (!$.isArray(listArray) || !listArray.length);

    if (!boxVisible && !listEmpty) {
        listContainer.fadeIn(500).slideDown(300);
    }
    else if (boxVisible && listEmpty) {
        listContainer.fadeOut(500).slideUp(300);
    }
}


function _master_removeAllClearMessage(domIssueContainer, domIssuesList) {
    if ($(domIssueContainer).attr("allClear") == "true") {
        $(domIssueContainer).fadeOut(500).slideUp(300);
        $(domIssuesList).children().each(function() {
            $(this).slideUp(400, function() { $(this).remove(); });
        });
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


    // Preserves the "this" object into success and error functions
    publicSymbols.proxiedAjaxCall = proxiedAjaxCall;
    function proxiedAjaxCall(url, params, thisObject, successFunction, errorFunction) {
        ajaxCall(
            url,
            params,
            $.proxy(successFunction, thisObject),
            errorFunction !== undefined? $.proxy(errorFunction, thisObject) : undefined
        );
    }


    publicSymbols.fancyBoxInit = fancyBoxInit;
    function fancyBoxInit(selector) {

        if (typeof selector == 'undefined') {
            selector = '.FancyBox_Gallery';
        }

        $(selector).fancybox({
            toolbar: false,
            smallBtn: true,
            arrows: false,
            infobar: false,
            title: $(this).title,

            helpers: {
                title: {
                    position: 'bottom',
                    type: 'float'
                }
            },

            beforeShow: function (instance, current) {
                $('.zoomContainer').remove();
                this.title = $(this.element).data("caption");

                if (instance.group.length > 1) {
                    if (instance.currIndex > 0) {
                        $('a.fancybox-arrow-previous').show();
                    } else {
                        $('a.fancybox-arrow-previous').hide();
                    }

                    if (instance.currIndex < instance.group.length - 1) {
                        $('a.fancybox-arrow-next').show();
                    } else {
                        $('a.fancybox-arrow-next').hide();
                    }
                }
            },

            afterShow: function (instance, current) {
                $('.fancybox-image').elevateZoom({
                    zoomType: "lens",
                    cursor: "crosshair",
                    zoomWindowFadeIn: 200,
                    zoomWindowFadeOut: 200,
                    lensShape: "round",
                    lensSize: 300
                });
            },

            afterLoad: function (instance, current) {

                /* TODO: MAKE A RIGHT-TO-LEFT VERSION OF THIS */

                if (instance.group.length > 1 && current.$content) {
                    current.$content.append('<a data-fancybox-next class="fancybox-arrow-next button-next" href="javascript:;">→</a><a data-fancybox-prev class="fancybox-arrow-previous button-previous" href="javascript:;" style="display:none">←</a>');
                }
            },

            afterClose: function () {
                $('.zoomContainer').remove();
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
