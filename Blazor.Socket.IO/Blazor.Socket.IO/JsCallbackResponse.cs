using System.Text.Json;

namespace Blazor.Socket.IO;

public class JsCallbackResponse
{
    public string[] arguments { get; private set; }
    public JsCallbackResponse(string[] arguments)
    {
        this.arguments = arguments;
    }
    public T? GetArg<T>(int i)
    {
        return JsonSerializer.Deserialize<T>(arguments[i]);
    }
}