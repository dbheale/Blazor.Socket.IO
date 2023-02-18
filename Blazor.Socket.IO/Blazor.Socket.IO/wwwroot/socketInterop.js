window.blazorSocket = function (server, options) {
    return io(server, options);
}

window._jsCallback = function (callbackObjectInstance, callbackMethod, callbackId, cmd, args) {
    var parts = cmd.split('.');
    var targetFunc = window;
    var parentObject = window;
    for (var i = 0; i < parts.length; i++) {
        var part = parts[i];
        if (i == 0 && part == 'window') continue;
        parentObject = targetFunc;
        targetFunc = targetFunc[part];
    }
    args = JSON.parse(args);
    args.push(function (e, d) {
        var args = [];
        for (var i in arguments) args.push(JSON.stringify(arguments[i]));
        callbackObjectInstance.invokeMethodAsync(callbackMethod, callbackId, args);
    });
    targetFunc.apply(parentObject, args);
};

window._jsObjectCallback = function (callbackObjectInstance, callbackMethod, callbackId, targetFunc, cmd, args) {
    var parts = cmd.split('.');
    var parentObject = targetFunc;
    for (var i = 0; i < parts.length; i++) {
        var part = parts[i];
        parentObject = targetFunc;
        targetFunc = targetFunc[part];
    }
    args = JSON.parse(args);
    args.push(function (e, d) {
        var args = [];
        for (var i in arguments) {
            args.push(JSON.stringify(arguments[i]));
        }
        callbackObjectInstance.invokeMethodAsync(callbackMethod, callbackId, args);
    });
    targetFunc.apply(parentObject, args);
};