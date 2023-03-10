using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Data;
using System.Reflection.Metadata;

namespace Blazor.Socket.IO;

public class BlazorSocket : ComponentBase, IAsyncDisposable
{
    private JsCallbackObjectReference? _socketRef;

    [Inject] private JsCallback? JsCallback { get; set; }

    [Parameter]
    public BlazorSocketOptions? Options { get; set; }

    [Parameter]
    public Dictionary<string, Action<string>>? EventHandlers { get; set; }
    
    [Parameter]
    public Dictionary<string, Action<string>>? OnceEventHandlers { get; set; }
    
    [Parameter, EditorRequired]
    public string? Url { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        if (JsCallback is null)
        {
            throw new ConstraintException("You must call the `.AddBlazorSocket()` service collection extension method.");
        }

        _socketRef = await JsCallback.InvokeAsync<IJSObjectReference>("blazorSocket", Url, Options);
        
        if (EventHandlers != null)
        {
            foreach (var eventHandler in EventHandlers)
            {
                await _socketRef.BindEventHandler((callback) => eventHandler.Value(callback.arguments[0]), "on",
                    eventHandler.Key);
            }
        }

        if (OnceEventHandlers != null)
        {
            foreach (var eventHandler in OnceEventHandlers)
            {
                await _socketRef.BindEventHandler((callback) => eventHandler.Value(callback.arguments[0]), "once",
                    eventHandler.Key);
            }
        }
        
        await base.OnInitializedAsync();
    }

    public Task AddEventHandler(string eventName, Action<string> eventHandler)
    {
        if (_socketRef is null)
        {
            throw new ConstraintException("You must open a connection before adding an event.");
        }

        return _socketRef.BindEventHandler((callback) => eventHandler(callback.arguments[0]), "on", eventName);
    }

    public Task AddOnceEventHandler(string eventName, Action<string> eventHandler)
    {
        if (_socketRef is null)
        {
            throw new ConstraintException("You must open a connection before adding an event.");
        }

        return _socketRef.BindEventHandler((callback) => eventHandler(callback.arguments[0]), "once", eventName);
    }

    public async Task EmitAsync(string eventName, params object?[]? args)
    {
        if (_socketRef is null)
        {
            throw new ConstraintException("You must open a connection before calling EmitAsync.");
        }
        
        await _socketRef.InvokeVoidAsync("emit", eventName, args);
    }

    public async ValueTask DisposeAsync()
    {
        if (_socketRef is not null)
        {
            await _socketRef.DisposeAsync();
        }
    }
}