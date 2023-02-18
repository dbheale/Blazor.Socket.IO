using System.Text.Json;
using Microsoft.JSInterop;

namespace Blazor.Socket.IO;

public class JsCallback
{
    private readonly IJSRuntime _jsRuntime;
    private readonly DotNetObjectReference<JsCallback> _this;
    private readonly Dictionary<string, Action<string[]>?> _callbacks;
    public JsCallback(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;

        _callbacks = new Dictionary<string, Action<string[]>?>();

        _this = DotNetObjectReference.Create(this);
    }

    [JSInvokable]
    public void _JsCallback(string callbackId, string[] arguments)
    {
        if (_callbacks.TryGetValue(callbackId, out Action<string[]>? callback))
        {
            _callbacks.Remove(callbackId);
            callback?.Invoke(arguments);
        }
    }

    public async Task<JsCallbackResponse> InvokeJsAsync(string cmd, params object[] args)
    {
        var t = new TaskCompletionSource<JsCallbackResponse>();
        await _InvokeJsAsync(arguments => {
            t.TrySetResult(new JsCallbackResponse(arguments));
        }, cmd, args);
        return await t.Task;
    }

    public Task InvokeJsAsync(Action<JsCallbackResponse> callback, string cmd, params object[] args)
    {
        return _InvokeJsAsync(arguments => {
            callback(new JsCallbackResponse(arguments));
        }, cmd, args);
    }

    private async Task _InvokeJsAsync(Action<string[]>? callback, string cmd, object[] args)
    {
        string callbackId;
        do
        {
            callbackId = Guid.NewGuid().ToString();
        } while (_callbacks.ContainsKey(callbackId));

        _callbacks[callbackId] = callback;

        await _jsRuntime.InvokeVoidAsync("window._jsCallback", _this, "_JsCallback", callbackId, cmd, JsonSerializer.Serialize(args));
    }

    public async Task<JsCallbackObjectReference> InvokeAsync<T>(string identifier, params object?[]? parameters) where T : IJSObjectReference
    {
        return new JsCallbackObjectReference(await _jsRuntime.InvokeAsync<T>(identifier, parameters), _jsRuntime, _this);
    }
}

public class JsCallbackObjectReference
{
    private readonly IJSObjectReference _jsObject;
    private readonly IJSRuntime _jsRuntime;
    private readonly DotNetObjectReference<JsCallbackObjectReference> _this;
    private readonly Dictionary<string, Action<string[]>?> _callbacks;
    public JsCallbackObjectReference(IJSObjectReference jsObject, IJSRuntime jsRuntime, DotNetObjectReference<JsCallback> parent)
    {
        _jsObject = jsObject;

        _jsRuntime = jsRuntime;

        _callbacks = new Dictionary<string, Action<string[]>?>();

        _this = DotNetObjectReference.Create(this);
    }

    [JSInvokable]
    public void _JsObjectCallback(string callbackId, string[] arguments)
    {
        if (_callbacks.TryGetValue(callbackId, out Action<string[]>? callback))
        {
            callback?.Invoke(arguments);
        }
    }

    public async Task<JsCallbackResponse> BindEventHandler(string cmd, params object[] args)
    {
        var t = new TaskCompletionSource<JsCallbackResponse>();
        await _BindEventHandler(arguments => {
            t.TrySetResult(new JsCallbackResponse(arguments));
        }, cmd, args);
        return await t.Task;
    }

    public Task BindEventHandler(Action<JsCallbackResponse> callback, string cmd, params object[] args)
    {
        return _BindEventHandler(arguments => {
            callback(new JsCallbackResponse(arguments));
        }, cmd, args);
    }

    private async Task _BindEventHandler(Action<string[]>? callback, string cmd, object[] args)
    {
        string callbackId;
        do
        {
            callbackId = Guid.NewGuid().ToString();
        } while (_callbacks.ContainsKey(callbackId));

        _callbacks[callbackId] = callback;

        await _jsRuntime.InvokeVoidAsync("window._jsObjectCallback", _this, "_JsObjectCallback", callbackId, _jsObject ,cmd, JsonSerializer.Serialize(args));
    }

    public ValueTask<T> InvokeAsync<T>(string identifier, List<object> parameters) where T : IJSObjectReference
    {
        return _jsObject.InvokeAsync<T>(identifier, parameters);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        return _jsObject.InvokeAsync<TValue>(identifier, args);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        return _jsObject.InvokeAsync<TValue>(identifier, cancellationToken, args);
    }

    public ValueTask InvokeVoidAsync(string identifier, params object?[]? args)
    {
        return _jsObject.InvokeVoidAsync(identifier, args);
    }

    public ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object?[]? args)
    {
        return _jsObject.InvokeVoidAsync(identifier, cancellationToken, args);
    }

    public ValueTask DisposeAsync()
    {
        return _jsObject.DisposeAsync();
    }
}