var SwarmopsJS = (function() {
    var publicSymbols = {},
        initFunctions = [];

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